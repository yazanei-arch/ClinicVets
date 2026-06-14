"""
Activity Monitor — Tkinter desktop app that logs keyboard and mouse activity
only while the user interacts inside this application window.
"""

from __future__ import annotations

import tkinter as tk
from datetime import datetime
from pathlib import Path
from tkinter import scrolledtext, ttk


class EventLogger:
    """Persists timestamped events to disk and feeds the on-screen log."""

    def __init__(self, log_file: Path) -> None:
        self._log_file = log_file
        self._log_file.parent.mkdir(parents=True, exist_ok=True)
        if not self._log_file.exists():
            self._log_file.write_text("", encoding="utf-8")

    @staticmethod
    def timestamp() -> str:
        return datetime.now().strftime("%Y-%m-%d %H:%M:%S")

    def format_line(self, message: str) -> str:
        return f"[{self.timestamp()}] {message}"

    def append(self, message: str, text_widget: scrolledtext.ScrolledText) -> str:
        line = self.format_line(message)
        text_widget.configure(state=tk.NORMAL)
        text_widget.insert(tk.END, line + "\n")
        text_widget.see(tk.END)
        text_widget.configure(state=tk.DISABLED)
        self._write_file(line)
        return line

    def _write_file(self, line: str) -> None:
        with self._log_file.open("a", encoding="utf-8") as handle:
            handle.write(line + "\n")

    def clear(self, text_widget: scrolledtext.ScrolledText) -> None:
        text_widget.configure(state=tk.NORMAL)
        text_widget.delete("1.0", tk.END)
        text_widget.configure(state=tk.DISABLED)
        self._log_file.write_text("", encoding="utf-8")


class InAppInputMonitor:
    """
    Registers Tkinter bindings that fire only for events inside the host window.
    Does not use OS-level hooks or global listeners.
    """

    MOTION_MIN_INTERVAL_MS = 80
    MOTION_MIN_PIXELS = 8

    def __init__(self, root: tk.Tk, on_event) -> None:
        self._root = root
        self._on_event = on_event
        self._active = False
        self._last_motion_time = 0
        self._last_motion_pos: tuple[int, int] | None = None
        self._bind_ids: list[str] = []

    def start(self) -> None:
        if self._active:
            return
        self._active = True
        self._last_motion_pos = None
        if self._bind_ids:
            return
        sequences = (
            "<KeyPress>",
            "<KeyRelease>",
            "<Button-1>",
            "<Button-2>",
            "<Button-3>",
            "<ButtonRelease-1>",
            "<ButtonRelease-2>",
            "<ButtonRelease-3>",
            "<Double-Button-1>",
            "<Motion>",
            "<MouseWheel>",
        )
        for sequence in sequences:
            self._root.bind_all(sequence, self._dispatch, add="+")
            self._bind_ids.append(sequence)

    def stop(self) -> None:
        self._active = False

    def _dispatch(self, event: tk.Event) -> None:
        if not self._active:
            return
        if not self._belongs_to_app(event):
            return
        if event.type == tk.EventType.KeyPress or event.type == tk.EventType.KeyRelease:
            if self._is_password_field(event.widget):
                return
            self._on_event(self._format_key_event(event))
            return
        if event.type == tk.EventType.Motion:
            self._handle_motion(event)
            return
        if event.type in (
            tk.EventType.ButtonPress,
            tk.EventType.ButtonRelease,
            tk.EventType.DoubleButton,
        ):
            self._on_event(self._format_button_event(event))
            return
        if event.type == tk.EventType.MouseWheel:
            self._on_event(self._format_wheel_event(event))

    def _belongs_to_app(self, event: tk.Event) -> bool:
        widget = event.widget
        if widget is None:
            return False
        try:
            top = widget.winfo_toplevel()
        except tk.TclError:
            return False
        if top != self._root:
            return False
        if event.type == tk.EventType.Motion or event.type in (
            tk.EventType.ButtonPress,
            tk.EventType.ButtonRelease,
            tk.EventType.DoubleButton,
            tk.EventType.MouseWheel,
        ):
            return self._pointer_inside_root(event)
        return self._widget_in_app(widget)

    @staticmethod
    def _widget_in_app(widget: tk.Misc) -> bool:
        current = widget
        while current is not None:
            if isinstance(current, tk.Tk):
                return True
            try:
                current = current.master
            except (AttributeError, tk.TclError):
                return False
        return False

    def _pointer_inside_root(self, event: tk.Event) -> bool:
        try:
            rx = self._root.winfo_rootx()
            ry = self._root.winfo_rooty()
            rw = self._root.winfo_width()
            rh = self._root.winfo_height()
        except tk.TclError:
            return False
        x = event.x_root
        y = event.y_root
        return rx <= x < rx + rw and ry <= y < ry + rh

    @staticmethod
    def _is_password_field(widget: tk.Misc) -> bool:
        if not isinstance(widget, (tk.Entry, ttk.Entry)):
            return False
        try:
            show = widget.cget("show")
            return bool(show) and show != ""
        except tk.TclError:
            return False

    def _handle_motion(self, event: tk.Event) -> None:
        now = int(self._root.tk.call("clock", "milliseconds"))
        pos = (event.x_root, event.y_root)
        if self._last_motion_pos is not None:
            dx = abs(pos[0] - self._last_motion_pos[0])
            dy = abs(pos[1] - self._last_motion_pos[1])
            if (
                now - self._last_motion_time < self.MOTION_MIN_INTERVAL_MS
                and dx < self.MOTION_MIN_PIXELS
                and dy < self.MOTION_MIN_PIXELS
            ):
                return
        self._last_motion_time = now
        self._last_motion_pos = pos
        self._on_event(
            f"Mouse move at window ({event.x}, {event.y}) "
            f"screen ({event.x_root}, {event.y_root}) on {event.widget.winfo_class()}"
        )

    def _format_key_event(self, event: tk.Event) -> str:
        action = "Key press" if event.type == tk.EventType.KeyPress else "Key release"
        keysym = event.keysym or "?"
        char = event.char
        if char and char.isprintable() and keysym not in ("Return", "Tab", "BackSpace"):
            detail = f"'{char}' (keysym={keysym})"
        else:
            detail = f"keysym={keysym}"
        widget_name = event.widget.winfo_class()
        return f"{action}: {detail} on {widget_name}"

    def _format_button_event(self, event: tk.Event) -> str:
        if event.type == tk.EventType.DoubleButton:
            action = "Double-click"
        elif event.type == tk.EventType.ButtonPress:
            action = "Mouse press"
        else:
            action = "Mouse release"
        button = getattr(event, "num", "?")
        return (
            f"{action}: button {button} at window ({event.x}, {event.y}) "
            f"on {event.widget.winfo_class()}"
        )

    def _format_wheel_event(self, event: tk.Event) -> str:
        direction = "up" if event.delta > 0 else "down"
        return (
            f"Mouse wheel {direction} at window ({event.x}, {event.y}) "
            f"on {event.widget.winfo_class()}"
        )


class ActivityMonitorApp:
    """Main Tkinter application shell and control wiring."""

    APP_TITLE = "Activity Monitor"
    LOG_RELATIVE_PATH = Path("logs") / "Log.txt"

    def __init__(self) -> None:
        self._root = tk.Tk()
        self._root.title(self.APP_TITLE)
        self._root.minsize(720, 520)
        self._root.geometry("860x600")
        self._apply_theme()

        log_path = Path(__file__).resolve().parent / self.LOG_RELATIVE_PATH
        self._logger = EventLogger(log_path)
        self._monitor = InAppInputMonitor(self._root, self._record_event)
        self._logging = False

        self._status_var = tk.StringVar(value="Stopped")
        self._build_ui()
        self._root.protocol("WM_DELETE_WINDOW", self._on_close)

    def _apply_theme(self) -> None:
        self._colors = {
            "bg": "#f0f4f8",
            "card": "#ffffff",
            "accent": "#2563eb",
            "accent_hover": "#1d4ed8",
            "danger": "#dc2626",
            "text": "#1e293b",
            "muted": "#64748b",
            "running": "#059669",
            "stopped": "#64748b",
        }
        self._root.configure(bg=self._colors["bg"])
        style = ttk.Style()
        if "vista" in style.theme_names():
            style.theme_use("vista")
        elif "clam" in style.theme_names():
            style.theme_use("clam")
        style.configure("TFrame", background=self._colors["bg"])
        style.configure("Card.TFrame", background=self._colors["card"])
        style.configure(
            "Title.TLabel",
            background=self._colors["bg"],
            foreground=self._colors["text"],
            font=("Segoe UI", 18, "bold"),
        )
        style.configure(
            "Subtitle.TLabel",
            background=self._colors["bg"],
            foreground=self._colors["muted"],
            font=("Segoe UI", 10),
        )
        style.configure(
            "Status.TLabel",
            background=self._colors["card"],
            font=("Segoe UI", 11, "bold"),
        )
        style.configure("TButton", font=("Segoe UI", 10), padding=(12, 8))

    def _build_ui(self) -> None:
        outer = ttk.Frame(self._root, padding=20)
        outer.pack(fill=tk.BOTH, expand=True)

        ttk.Label(outer, text=self.APP_TITLE, style="Title.TLabel").pack(anchor=tk.W)
        ttk.Label(
            outer,
            text="Logs keyboard and mouse activity inside this window only.",
            style="Subtitle.TLabel",
        ).pack(anchor=tk.W, pady=(4, 16))

        card = ttk.Frame(outer, style="Card.TFrame", padding=16)
        card.pack(fill=tk.BOTH, expand=True)

        toolbar = ttk.Frame(card, style="Card.TFrame")
        toolbar.pack(fill=tk.X, pady=(0, 12))

        self._btn_start = ttk.Button(toolbar, text="Start Logging", command=self.start_logging)
        self._btn_start.pack(side=tk.LEFT, padx=(0, 8))

        self._btn_stop = ttk.Button(
            toolbar, text="Stop Logging", command=self.stop_logging, state=tk.DISABLED
        )
        self._btn_stop.pack(side=tk.LEFT, padx=(0, 8))

        self._btn_clear = ttk.Button(toolbar, text="Clear Log", command=self.clear_log)
        self._btn_clear.pack(side=tk.LEFT)

        status_frame = ttk.Frame(toolbar, style="Card.TFrame")
        status_frame.pack(side=tk.RIGHT)
        ttk.Label(status_frame, text="Status:", style="Status.TLabel").pack(side=tk.LEFT, padx=(0, 6))
        self._status_label = ttk.Label(
            status_frame,
            textvariable=self._status_var,
            style="Status.TLabel",
            foreground=self._colors["stopped"],
        )
        self._status_label.pack(side=tk.LEFT)

        log_frame = ttk.Frame(card, style="Card.TFrame")
        log_frame.pack(fill=tk.BOTH, expand=True)

        self._log_text = scrolledtext.ScrolledText(
            log_frame,
            wrap=tk.WORD,
            font=("Consolas", 10),
            state=tk.DISABLED,
            bg="#0f172a",
            fg="#e2e8f0",
            insertbackground="#e2e8f0",
            relief=tk.FLAT,
            padx=10,
            pady=10,
        )
        self._log_text.pack(fill=tk.BOTH, expand=True)

        footer = ttk.Label(
            card,
            text=f"Events are saved to: {self.LOG_RELATIVE_PATH}",
            style="Subtitle.TLabel",
            background=self._colors["card"],
        )
        footer.pack(anchor=tk.W, pady=(10, 0))

        self._logger.append("Activity Monitor ready. Click Start Logging to begin.", self._log_text)

    def _record_event(self, message: str) -> None:
        if self._logging:
            self._logger.append(message, self._log_text)

    def start_logging(self) -> None:
        if self._logging:
            return
        self._logging = True
        self._monitor.start()
        self._status_var.set("Running")
        self._status_label.configure(foreground=self._colors["running"])
        self._btn_start.configure(state=tk.DISABLED)
        self._btn_stop.configure(state=tk.NORMAL)
        self._logger.append("--- Logging started (in-app only) ---", self._log_text)

    def stop_logging(self) -> None:
        if not self._logging:
            return
        self._logging = False
        self._monitor.stop()
        self._status_var.set("Stopped")
        self._status_label.configure(foreground=self._colors["stopped"])
        self._btn_start.configure(state=tk.NORMAL)
        self._btn_stop.configure(state=tk.DISABLED)
        self._logger.append("--- Logging stopped ---", self._log_text)

    def clear_log(self) -> None:
        self._logger.clear(self._log_text)
        self._logger.append("Log cleared.", self._log_text)

    def _on_close(self) -> None:
        self.stop_logging()
        self._root.destroy()

    def run(self) -> None:
        self._root.mainloop()


if __name__ == "__main__":
    ActivityMonitorApp().run()
