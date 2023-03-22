﻿using CS6232_G2.Controller;
using CS6232_G2.Model;
using CS6232_G2.View;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CS6232_G2.UserControls
{
    public partial class SearchPatientUserControl : UserControl
    {
        private readonly SearchController _searchController;
        private readonly AppointmentController _appointmentController;
        private readonly PatientController _patientController;
        private Appointment _appointment;
        private List<Appointment> patients;
        private DateTime dob;

        public SearchPatientUserControl()
        {
            InitializeComponent();
            _searchController = new SearchController();
            _appointmentController = new AppointmentController();
            _patientController = new PatientController();
            patients = new List<Appointment>();
        }

        private void firstLastNameRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            firstNameTextBox.Enabled = true;
            lastNameTextBox.Enabled = true;
            dateTimePicker.Enabled = false;
            searchButton.Enabled = true;
            clearButton.Enabled = true;
            appointmentDataGridView.DataSource=null;
            clear();
        }

        private void dobRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            dateTimePicker.Enabled = true;
            firstNameTextBox.Enabled = false;
            lastNameTextBox.Enabled = false;
            searchButton.Enabled = true;
            clearButton.Enabled = true;
            appointmentDataGridView.DataSource=null;
            clear();
        }

        private void dobLastnameradioButton_CheckedChanged(object sender, EventArgs e)
        {
            dateTimePicker.Enabled = true;
            lastNameTextBox.Enabled = true;
            firstNameTextBox.Enabled = false;
            searchButton.Enabled = true;
            clearButton.Enabled = true;
            appointmentDataGridView.DataSource=null;
            clear();
        }
        private void clear()
        {
            firstNameTextBox.Text = "";
            lastNameTextBox.Text= "";
            dateTimePicker.Value= DateTime.Now;
        }
        private void clearButton_Click(object sender, EventArgs e)
        {
            clear();
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            string fname = firstNameTextBox.Text;
            string lname = lastNameTextBox.Text;

            try
            {
                if (firstLastNameRadioButton.Checked)
                {
                    if (string.IsNullOrEmpty(fname) || string.IsNullOrEmpty(lname))
                    {
                        MessageBox.Show("Please enter  patient first and last name", "Patient name", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    patients = _searchController.GetPatientByFirstAndLastName(fname, lname);
                    if (patients.Count == 0)
                    {
                        MessageBox.Show("No patient found with the name");
                    }

                    appointmentDataGridView.DataSource = patients;
                }
                else if (dobLastnameradioButton.Checked)
                {
                    if (string.IsNullOrEmpty(lname))
                    {
                        MessageBox.Show("Please enter  patient last name", "Patient name", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    DateTime dob = Convert.ToDateTime(dateTimePicker.Value).Date;
                    patients = _searchController.GetPatientsByDOBAndLastName(dob, lname);

                    if (patients.Count == 0)
                    {
                        MessageBox.Show("No patient found with the date");
                    }
                    appointmentDataGridView.DataSource = patients;
                }
                else if (dobRadioButton.Checked)
                {
                    DateTime dob = Convert.ToDateTime(dateTimePicker.Value).Date;
                    patients = _searchController.GetPatientsByDOB(dob);

                    if (patients.Count == 0)
                    {
                        MessageBox.Show("No patient found with the date");
                    }
                    appointmentDataGridView.DataSource = patients;
                }

                patientname();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().ToString());
            }
        }
        private void patientname()
        {
            appointmentDataGridView.DataSource = patients;

            if (appointmentDataGridView.SelectedRows.Count > 0)
            {
                _appointment = _appointmentController.GetAppointmentById(_appointment.AppointmentId);
                ///patientLinkLabel , patient name to be visible to edit
            }
        }

        private void patientLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (EditPatientForm _editForm = new EditPatientForm())
            {
                _patientController.SetPatientToEdit(_patientController.GetPatientByPatientId(int.Parse(appointmentDataGridView.SelectedCells[0].Value.ToString())));
                Hide();
                _editForm.ShowDialog();
            }    
        }
    }
}