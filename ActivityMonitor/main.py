"""Entry point for the Activity Monitor desktop application."""

from activity_monitor import ActivityMonitorApp


def main() -> None:
    app = ActivityMonitorApp()
    app.run()


if __name__ == "__main__":
    main()
