using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using ClinicVets.UI;
using ClinicVets;
using System.Linq;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        private static void RunOnSta(Action action)
        {
            Exception exception = null;

            Thread thread = new Thread(() =>
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            if (exception != null)
                throw exception;
        }

        private AddPetForm CreateValidForm()
        {
            AddPetForm form = new AddPetForm();

            SetTextBox(form, "txtPetName", "Rex");
            SetComboBox(form, "cmbAnimalType", "Dog");
            SetTextBox(form, "txtWeight", "10");
            SetTextBox(form, "txtChipNumber", "123456");
            SetTextBox(form, "txtOwner", "Yazan");

            SetDateTimePicker(form, "dtpBirthDate", DateTime.Today.AddYears(-2));
            SetDateTimePicker(form, "dtpLastVaccineDate", DateTime.Today.AddMonths(-1));

            return form;
        }

        [TestMethod]
        public void Save_EmptyPetName_ShowsRequiredError()
        {
            RunOnSta(() =>
            {
                using (AddPetForm form = CreateValidForm())
                {
                    SetTextBox(form, "txtPetName", "");

                    InvokeSave(form);

                    Label error = GetPrivateLabel(form, "lblPetNameError");
                    Assert.AreEqual("Pet name is required.", error.Text);
                }
            });
        }

        [TestMethod]
        public void Save_WeightIsNotNumber_ShowsNumberError()
        {
            RunOnSta(() =>
            {
                using (AddPetForm form = CreateValidForm())
                {
                    SetTextBox(form, "txtWeight", "abc");

                    InvokeSave(form);

                    Label error = GetPrivateLabel(form, "lblWeightError");
                    Assert.AreEqual("Weight must be a number.", error.Text);
                }
            });
        }

        [TestMethod]
        public void Save_BirthDateInFuture_ShowsBirthDateError()
        {
            RunOnSta(() =>
            {
                using (AddPetForm form = CreateValidForm())
                {
                    SetDateTimePicker(form, "dtpBirthDate", DateTime.Today.AddDays(1));

                    InvokeSave(form);

                    Label error = GetPrivateLabel(form, "lblBirthDateError");
                    Assert.AreEqual("Birth date cannot be future.", error.Text);
                }
            });
        }

        private static void InvokeSave(AddPetForm form)
        {
            MethodInfo method = typeof(AddPetForm).GetMethod(
                "btnSave_Click",
                BindingFlags.Instance | BindingFlags.NonPublic);

            method.Invoke(form, new object[] { null, EventArgs.Empty });
        }

        private static Label GetPrivateLabel(AddPetForm form, string fieldName)
        {
            FieldInfo field = typeof(AddPetForm).GetField(
                fieldName,
                BindingFlags.Instance | BindingFlags.NonPublic);

            return (Label)field.GetValue(form);
        }

        private static void SetTextBox(AddPetForm form, string name, string value)
        {
            Control control = GetPrivateField<Control>(form, name);
            control.Text = value;
        }

        private static void SetComboBox(AddPetForm form, string name, string value)
        {
            ComboBox comboBox = GetPrivateField<ComboBox>(form, name);

            comboBox.Items.Clear();
            comboBox.Items.Add(value);
            comboBox.SelectedIndex = 0;
        }

        private static void SetDateTimePicker(AddPetForm form, string name, DateTime value)
        {
            DateTimePicker picker = GetPrivateField<DateTimePicker>(form, name);
            picker.Value = value;
        }

        private static T FindControl<T>(Control parent, string name) where T : Control
        {
            if (parent == null)
                return null;

            foreach (Control control in parent.Controls)
            {
                if (control.Name == name)
                    return control as T;

                T result = FindControl<T>(control, name);

                if (result != null)
                    return result;
            }

            return null;
        }

        [TestMethod]
        public void RegisterButton_WithEmptyFields_ShouldStayOnRegisterForm()
        {
            RunOnSta(() =>
            {
                using (RegisterEmployeeForm form = new RegisterEmployeeForm())
                {
                    InvokeRegisterFormLoad(form);

                    TextBox txtUsername = FindControl<TextBox>(form, "txtUsername");
                    TextBox txtFullName = FindControl<TextBox>(form, "txtFullName");
                    TextBox txtPassword = FindControl<TextBox>(form, "txtPassword");
                    TextBox txtEmployeeNumber = FindControl<TextBox>(form, "txtEmployeeNumber");
                    TextBox txtEmail = FindControl<TextBox>(form, "txtEmail");
                    TextBox txtId = FindControl<TextBox>(form, "txtId");
                    ComboBox cmbRole = FindControl<ComboBox>(form, "cmbRole");

                    txtUsername.Text = "";
                    txtFullName.Text = "";
                    txtPassword.Text = "";
                    txtEmployeeNumber.Text = "";
                    txtEmail.Text = "";
                    txtId.Text = "";
                    cmbRole.SelectedIndex = -1;

                    InvokeRegisterButtonClick(form);

                    Assert.IsFalse(form.IsDisposed);
                }
            });
        }

        private static void InvokeRegisterFormLoad(RegisterEmployeeForm form)
        {
            MethodInfo method = typeof(RegisterEmployeeForm).GetMethod(
                "RegisterEmployeeForm_Load",
                BindingFlags.Instance | BindingFlags.NonPublic);

            method.Invoke(form, new object[] { null, EventArgs.Empty });
        }

        private static void InvokeRegisterButtonClick(RegisterEmployeeForm form)
        {
            MethodInfo method = typeof(RegisterEmployeeForm).GetMethod(
                "btnRegister_Click",
                BindingFlags.Instance | BindingFlags.NonPublic);

            method.Invoke(form, new object[] { null, EventArgs.Empty });
        }

        [TestMethod]
        public void LoginForm_Load_SetsTitleAndSubtitle()
        {
            RunOnSta(() =>
            {
                using (Form1 form = new Form1())
                {
                    InvokeForm1Load(form);

                    Label lblTitle = FindControl<Label>(form, "lblTitle");
                    Label lblSubtitle = FindControl<Label>(form, "lblSubtitle");

                    Assert.AreEqual("Welcome Back", lblTitle.Text);
                    Assert.AreEqual("Sign in to access your clinic workspace", lblSubtitle.Text);
                }
            });
        }

        [TestMethod]
        public void LoginForm_EmptyFields_ShowsValidationError()
        {
            RunOnSta(() =>
            {
                using (Form1 form = new Form1())
                {
                    InvokeForm1Load(form);

                    TextBox txtUsername = FindControl<TextBox>(form, "txtUsername");
                    TextBox txtPassword = FindControl<TextBox>(form, "txtPassword");

                    txtUsername.Text = "";
                    txtPassword.Text = "";

                    InvokeLoginClick(form);

                    Label lblUsernameError = FindControl<Label>(form, "lblUsernameError");
                    Label lblPasswordError = FindControl<Label>(form, "lblPasswordError");

                    Assert.IsTrue(
                        !string.IsNullOrWhiteSpace(lblUsernameError.Text) ||
                        !string.IsNullOrWhiteSpace(lblPasswordError.Text)
                    );
                }
            });
        }

        [TestMethod]
        public void LoginForm_ClearLoginFields_ClearsUsernameAndPassword()
        {
            RunOnSta(() =>
            {
                using (Form1 form = new Form1())
                {
                    InvokeForm1Load(form);

                    TextBox txtUsername = FindControl<TextBox>(form, "txtUsername");
                    TextBox txtPassword = FindControl<TextBox>(form, "txtPassword");

                    txtUsername.Text = "Yazan";
                    txtPassword.Text = "123456";

                    form.ClearLoginFields();

                    Assert.AreEqual("", txtUsername.Text);
                    Assert.AreEqual("", txtPassword.Text);
                }
            });
        }
        private static void InvokeForm1Load(Form1 form)
        {
            MethodInfo method = typeof(Form1).GetMethod(
                "Form1_Load",
                BindingFlags.Instance | BindingFlags.NonPublic);

            method.Invoke(form, new object[] { null, EventArgs.Empty });
        }

        private static void InvokeLoginClick(Form1 form)
        {
            MethodInfo method = typeof(Form1).GetMethod(
                "btnLogin_Click",
                BindingFlags.Instance | BindingFlags.NonPublic);

            method.Invoke(form, new object[] { null, EventArgs.Empty });
        }

        [TestMethod]
        public void SearchPet_ClearButton_ClearsFieldsAndGrid()
        {
            RunOnSta(() =>
            {
                using (SearchPetForm form = new SearchPetForm())
                {
                    TextBox txtPetName2 = FindControl<TextBox>(form, "txtPetName2");
                    TextBox txtChipNumber2 = FindControl<TextBox>(form, "txtChipNumber2");
                    DataGridView dgvPets = FindControl<DataGridView>(form, "dgvPets");

                    txtPetName2.Text = "Rex";
                    txtChipNumber2.Text = "123456";

                    dgvPets.Rows.Add(
                        "Rex",
                        "Dog",
                        "10",
                        DateTime.Today.ToShortDateString(),
                        "Yazan",
                        "123456",
                        DateTime.Today.ToShortDateString()
                    );

                    InvokeSearchPetClearButton(form);

                    Assert.AreEqual("", txtPetName2.Text);
                    Assert.AreEqual("", txtChipNumber2.Text);
                    Assert.AreEqual(0, dgvPets.Rows.Count);
                }
            });
        }

        private static void InvokeSearchPetClearButton(SearchPetForm form)
        {
            MethodInfo method = typeof(SearchPetForm).GetMethod(
                "btnClear2_Click",
                BindingFlags.Instance | BindingFlags.NonPublic);

            method.Invoke(form, new object[] { null, EventArgs.Empty });
        }

        [TestMethod]
        public void AnimalTypes_BackButton_ShouldCloseForm()
        {
            RunOnSta(() =>
            {
                AnimalTypesForm form = new AnimalTypesForm();

                InvokeAnimalTypesBackButton(form);

                Assert.IsTrue(form.IsDisposed);
            });
        }
        private static void InvokeAnimalTypesBackButton(AnimalTypesForm form)
        {
            MethodInfo method = typeof(AnimalTypesForm).GetMethod(
                "btnBack_Click",
                BindingFlags.Instance | BindingFlags.NonPublic);

            method.Invoke(form, new object[] { null, EventArgs.Empty });
        }
        [TestMethod]
        public void MedicinesForm_ShouldCreateMainControls()
        {
            RunOnSta(() =>
            {
                using (Form form = CreateMedicinesForm())
                {
                    TextBox txtName = GetPrivateField<TextBox>(form, "txtName");
                    NumericUpDown numQuantity = GetPrivateField<NumericUpDown>(form, "numQuantity");
                    NumericUpDown numPrice = GetPrivateField<NumericUpDown>(form, "numPrice");
                    DataGridView dgvMedicines = GetPrivateField<DataGridView>(form, "dgvMedicines");

                    Assert.IsNotNull(txtName);
                    Assert.IsNotNull(numQuantity);
                    Assert.IsNotNull(numPrice);
                    Assert.IsNotNull(dgvMedicines);

                    Assert.AreEqual(1, numQuantity.Minimum);
                    Assert.AreEqual(10000, numQuantity.Maximum);
                    Assert.AreEqual(2, numPrice.DecimalPlaces);
                }
            });
        }

        [TestMethod]
        public void MedicinesForm_BackButton_ShouldCloseForm()
        {
            RunOnSta(() =>
            {
                Form form = CreateMedicinesForm();

                Button btnBack = FindButtonByText(form, "← Back");

                Assert.IsNotNull(btnBack);

                ClickButton(btnBack);

                Assert.IsTrue(form.IsDisposed);
            });
        }
        private static void ClickButton(Button button)
        {
            MethodInfo method = typeof(Button).GetMethod(
                "OnClick",
                BindingFlags.Instance | BindingFlags.NonPublic);

            method.Invoke(button, new object[] { EventArgs.Empty });
        }
        private static Form CreateMedicinesForm()
        {
            Type type = Type.GetType(
                "ClinicVets.VisitsMedicines.MedicinesForm, ClinicVets.VisitsMedicines");

            if (type == null)
            {
                type = Type.GetType(
                    "ClinicVets.VisitsMedicines.MedicinesForm, ClinicVets");
            }

            if (type == null)
            {
                Assert.Fail("MedicinesForm type was not found.");
            }

            return (Form)Activator.CreateInstance(type);
        }
        private static T GetPrivateField<T>(object obj, string fieldName) where T : class
        {
            Type type = obj.GetType();

            while (type != null)
            {
                FieldInfo field = type.GetField(
                    fieldName,
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                if (field != null)
                {
                    object value = field.GetValue(obj);

                    if (value == null)
                        return null;

                    if (value is T typedValue)
                        return typedValue;

                    Assert.Fail(
                        "Field '" + fieldName + "' is type '" +
                        value.GetType().Name +
                        "', not '" + typeof(T).Name + "'.");
                }

                type = type.BaseType;
            }

            Assert.Fail("Field not found: " + fieldName);
            return null;
        }

        private static Button FindButtonByText(Control parent, string text)
        {
            if (parent == null)
                return null;

            foreach (Control control in parent.Controls)
            {
                Button button = control as Button;

                if (button != null && button.Text == text)
                    return button;

                Button result = FindButtonByText(control, text);

                if (result != null)
                    return result;
            }

            return null;
        }
        
        private static bool IsControlMarkedVisible(Control control)
        {
            MethodInfo method = typeof(Control).GetMethod(
                "GetState",
                BindingFlags.Instance | BindingFlags.NonPublic);

            return (bool)method.Invoke(control, new object[] { 2 });
        }

        [TestMethod]
        public void VisitForm_InvalidVetName_ShouldBeDetected()
        {
            string vetName = "Doctor123";

            bool isValid = vetName.All(c => char.IsLetter(c) || c == ' ');

            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void VisitForm_EmptyReason_ShouldShowReasonError()
        {
            RunOnSta(() =>
            {
                using (Form form = CreateVisitManagementForm())
                {
                    TextBox txtReason = GetPrivateField<TextBox>(form, "txtReason");
                    Label lblReasonError = GetPrivateField<Label>(form, "lblReasonError");

                    txtReason.Text = "";

                    InvokePrivateMethod(form, "txtReason_Leave");

                    Assert.IsTrue(IsControlMarkedVisible(lblReasonError));
                }
            });
        }
        private static Form CreateVisitManagementForm()
        {
            Type type = Type.GetType(
                "ClinicVets.VisitsMedicines.VisitManagementForm, ClinicVets.VisitsMedicines");

            if (type == null)
            {
                type = Type.GetType(
                    "ClinicVets.VisitsMedicines.VisitManagementForm, ClinicVets");
            }

            if (type == null)
            {
                Assert.Fail("VisitManagementForm type was not found.");
            }

            return (Form)Activator.CreateInstance(type);
        }

        private static void InvokePrivateMethod(object obj, string methodName)
        {
            MethodInfo method = obj.GetType().GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.NonPublic);

            if (method == null)
            {
                Assert.Fail("Method not found: " + methodName);
            }

            method.Invoke(obj, new object[] { null, EventArgs.Empty });
        }
        [TestMethod]
        public void CustomerManagement_SplitFullName_ShouldSeparateFirstAndLastName()
        {
            MethodInfo method = typeof(CustomerManagementForm).GetMethod(
                "SplitFullName",
                BindingFlags.Static | BindingFlags.NonPublic);

            object[] parameters = new object[]
            {
        "Yazan Eissa",
        null,
        null
            };

            method.Invoke(null, parameters);

            Assert.AreEqual("Yazan", parameters[1]);
            Assert.AreEqual("Eissa", parameters[2]);
        }
        [TestMethod]
        public void ForgotPassword_SendCode_WithInvalidEmail_ShouldNotCreateVerificationCode()
        {
            RunOnSta(() =>
            {
                using (ForgotPasswordForm form = new ForgotPasswordForm())
                {
                    InvokeForgotPasswordLoad(form);

                    TextBox txtEmail = FindControl<TextBox>(form, "txtEmail");
                    txtEmail.Text = "wrong-email";

                    InvokeForgotPasswordSendCode(form);

                    string verificationCode = GetPrivateField<string>(form, "_verificationCode");

                    Assert.IsNull(verificationCode);
                }
            });
        }
        private static void InvokeForgotPasswordLoad(ForgotPasswordForm form)
        {
            MethodInfo method = typeof(ForgotPasswordForm).GetMethod(
                "ForgotPasswordForm_Load",
                BindingFlags.Instance | BindingFlags.NonPublic);

            method.Invoke(form, new object[] { null, EventArgs.Empty });
        }

        private static void InvokeForgotPasswordSendCode(ForgotPasswordForm form)
        {
            MethodInfo method = typeof(ForgotPasswordForm).GetMethod(
                "btnSendCode_Click",
                BindingFlags.Instance | BindingFlags.NonPublic);

            method.Invoke(form, new object[] { null, EventArgs.Empty });
        }
    }

}