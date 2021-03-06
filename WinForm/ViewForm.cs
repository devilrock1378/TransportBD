﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Model;

namespace TransportBD
{
    public partial class ViewForm : Form
    {
        private string _filePath;
        private List<ITransport> _transportList;
        private readonly MessageService _messageServices = new MessageService();
        private bool _pointFixer;
        private RecentFilesSubMenu _recentFiles = new RecentFilesSubMenu();

        public ViewForm(string[] args)
        {
            InitializeComponent();
            _pointFixer = true;
            ItemsEnable(false);
            comboBoxSearchFuelType.Visible = false;
            if (args.Length > 0)
            {
                try
                {
                    _transportList = Serialization.Deserialize(args[0]);
                }
                catch (ArgumentException)
                {
                    _messageServices.ShowError("Error");
                }
                iTransportBindingSource.DataSource = _transportList;
                _pointFixer = true;
                ItemsEnable(true);
            }

        }

        /// <summary>
        /// Фиксатор изменений
        /// </summary>
        /// <param name="pointFixer"></param>
        private void PointFixer(bool pointFixer)
        {
            if (pointFixer == false)
            {
                this.Text = _filePath.Substring(_filePath.LastIndexOf("\\") + 1) + "* - TransportBD";
            }
            else
            {
                this.Text = _filePath.Substring(_filePath.LastIndexOf("\\") + 1) + " - TransportBD";
            }
        }

        /// <summary>
        ///  Открытие файла (Меню)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog { Filter = "Файлы|*.tdb|Все файлы|*.*" };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                if (ofd.FileName != null)
                {
                    _filePath = ofd.FileName;
                    LoadRecentFiles(_filePath);
                    _transportList = Serialization.Deserialize(_filePath);
                    iTransportBindingSource.DataSource = _transportList;
                    PointFixer(true);
                    ItemsEnable(true);
                }
            }
        }

        /// <summary>
        /// Загрузка 'Недавних файлов' в лист
        /// </summary>
        /// <param name="filePath"></param>
        private void LoadRecentFiles(string filePath)
        {
            _recentFiles.AddRecentFiles(filePath);

            if (_recentFiles.RecentFilesList().Count > 0)
            {
                recentFilesToolStripMenuItem.DropDownItems.Clear();
                AddRecentFilesToMenu();
            }
        }

        /// <summary>
        /// Добавление 'Недавних файлов' в подменю
        /// </summary>
        private void AddRecentFilesToMenu()
        {
            for (var i = 0; i < _recentFiles.RecentFilesList().Count; i++)
            {
                recentFilesToolStripMenuItem.DropDownItems.Add(Path.GetFileName(_recentFiles.RecentFilesList()[i]));
                recentFilesToolStripMenuItem.DropDownItems[i].Click += OnClick;
            }
        }

        /// <summary>
        /// Открытие файла из подменю 'Недавние файлы'
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void OnClick(object sender, EventArgs eventArgs)
        {
            var index = recentFilesToolStripMenuItem.DropDownItems.IndexOf((ToolStripDropDownItem) sender);
            _filePath = _recentFiles.RecentFilesList()[index];
            _transportList = Serialization.Deserialize(_filePath);
            iTransportBindingSource.DataSource = _transportList;
            PointFixer(true);
            LoadRecentFiles(_recentFiles.RecentFilesList()[index]);
        }

        /// <summary>
        /// Сохранение (Меню)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save();
        }

        /// <summary>
        /// Изменить (Меню)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void changeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Change();
        }

        /// <summary>
        /// Сохранить как (Меню)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAss();
        }

        /// <summary>
        /// Добавить (Меню)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Add();
        }

        /// <summary>
        /// Выход (Меню)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Новый файл (Меню)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void createToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _filePath = null;
            iTransportBindingSource.DataSource = _transportList = new List<ITransport>();
            var sfd = new SaveFileDialog();
            sfd.Filter = "Файлы|*.tdb|Все файлы|*.*";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                _filePath = sfd.FileName;
                Serialization.Serialize(_filePath, _transportList);
                PointFixer(_pointFixer = true);
            }
            ItemsEnable(true);
        }

        /// <summary>
        /// Удалить (Меню)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Delete();
        }

        /// <summary>
        /// О программе (Меню)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var aboutForm = new AboutForm();
            aboutForm.ShowDialog();
        }

        /// <summary>
        /// Руководство (Меню)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var helpForm = new HelpForm();
            helpForm.ShowDialog();
        }

        /// <summary>
        /// Закрытие формы (Меню)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewForm_FormClosing(object sender, FormClosingEventArgs e)
        {

            if (_pointFixer != true)
            {
                DialogResult dialogResult = MessageBox.Show(
                    "Сохранить изменения ?", "Предупреждение",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Exclamation);

                switch (dialogResult)
                {
                    case DialogResult.Yes:
                        Save();
                        break;
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        break;
                }
            }
        }

        /// <summary>
        /// Добавить (Кнопка)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            Add();
        }

        /// <summary>
        /// Изменить (Кнопка)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonChange_Click(object sender, EventArgs e)
        {
            Change();
        }

        /// <summary>
        /// Удалить (Кнопка)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonDelete_Click(object sender, EventArgs e)
        {
            Delete();
        }

        /// <summary>
        /// Поиск (Кнопка)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSearch_Click(object sender, EventArgs e)
        {
            if (_transportList != null)
            {
                if (comboBoxSearch.SelectedIndex != -1)
                {
                    string searchLine = textBoxSearch.Text;
                    switch (comboBoxSearch.SelectedItem.ToString())
                    {
                        case "Марка":
                            {
                                iTransportBindingSource.DataSource = _transportList.FindAll(
                                transport => transport.Mark == searchLine);
                                break;
                            }
                        case "Скорость":
                            {
                                iTransportBindingSource.DataSource = _transportList.FindAll(
                                transport => transport.Speed.ToString() == searchLine);
                                break;
                            }
                        case "Степень износа":
                            {
                                iTransportBindingSource.DataSource = _transportList.FindAll(
                                transport => transport.Wear.ToString() == searchLine);
                                break;
                            }
                        case "Объем бака":
                            {
                                iTransportBindingSource.DataSource = _transportList.FindAll(
                                transport => transport.CurrentVolume.ToString() == searchLine);
                                break;
                            }
                        case "Тип топлива":
                            {
                                iTransportBindingSource.DataSource = _transportList.FindAll(
                                transport => transport.FuelType.ToString() == (string)comboBoxSearchFuelType.SelectedItem);
                                break;
                            }
                        case "Расход топлива":
                            {
                                iTransportBindingSource.DataSource = _transportList.FindAll(
                                transport => transport.FuelConsumption.ToString() == searchLine);
                                break;
                            }
                    }
                }
                else
                {
                    _messageServices.ShowExclamation("Не выбрано поле поиска.");
                }
            }
            else
            {
                _messageServices.ShowExclamation("База данных пуста.");
            }
        }

        /// <summary>
        /// Метод Сохранения
        /// </summary>
        private void Save()
        {
            if (_filePath != null)
            {
                Serialization.Serialize(_filePath, _transportList);
                _messageServices.ShowMessage("Сохранение прошло успешно.");
                PointFixer(_pointFixer = true);
            }
            else
            {
                var sfd = new SaveFileDialog();
                sfd.Filter = "Файлы|*.tdb|Все файлы|*.*";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    _filePath = sfd.FileName;
                    Serialization.Serialize(_filePath, _transportList);
                    _messageServices.ShowMessage("Сохранение прошло успешно.");
                    PointFixer(_pointFixer = true);
                }
            }
        }

        /// <summary>
        /// Метод Сохранить как...
        /// </summary>
        private void SaveAss()
        {
            var sfd = new SaveFileDialog();
            sfd.Filter = "Файлы|*.tdb|Все файлы|*.*";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                _filePath = sfd.FileName;
                Serialization.Serialize(_filePath, _transportList);
                _messageServices.ShowMessage("Сохранение прошло успешно.");
                PointFixer(_pointFixer = true);
            }
        }

        /// <summary>
        /// Метод Изменений
        /// </summary>
        private void Change()
        {
            if (iTransportBindingSource.Current != null)
            {
                var cof = new CreateObjectForm();
                var index = iTransportBindingSource.IndexOf(iTransportBindingSource.Current);
                cof.Transport = (ITransport)iTransportBindingSource.Current;

                if (cof.ShowDialog() == DialogResult.OK)
                {
                    iTransportBindingSource.RemoveAt(index);
                    PointFixer(_pointFixer = false);
                    iTransportBindingSource.Add(cof.Transport);
                }
            }
            else
            {
                _messageServices.ShowError("Не выбрана запись. Выберите запись и повторите попытку.");
            }
        }

        /// <summary>
        /// Метод Добавления
        /// </summary>
        private void Add()
        {
            var cof = new CreateObjectForm();
            if (cof.ShowDialog() == DialogResult.OK)
            {
                PointFixer(_pointFixer = false);
                iTransportBindingSource.Add(cof.Transport);
            }
        }

        /// <summary>
        /// Метод Удаления
        /// </summary>
        private void Delete()
        {
            if (iTransportBindingSource.Current != null)
            {
                iTransportBindingSource.Remove(iTransportBindingSource.Current);
                PointFixer(_pointFixer = false);
            }
            else
            {
                _messageServices.ShowError("Не выбрана запись. Выберите запись и повторите попытку.");
            }
        }

        /// <summary>
        /// Отображение контролов на форме
        /// </summary>
        /// <param name="itemsEnable"></param>
        private void ItemsEnable(bool itemsEnable)
        {
            buttonAdd.Enabled = itemsEnable;
            buttonChange.Enabled = itemsEnable;
            buttonDelete.Enabled = itemsEnable;
            buttonReset.Enabled = itemsEnable;
            buttonSearch.Enabled = itemsEnable;
            comboBoxSearch.Enabled = itemsEnable;
            textBoxSearch.Enabled = itemsEnable;
            textBoxDistance.Enabled = itemsEnable;
            label2.Enabled = itemsEnable;
            label4.Enabled = itemsEnable;
            buttonTest.Enabled = itemsEnable;
            saveAsToolStripMenuItem.Enabled = itemsEnable;
            saveToolStripMenuItem.Enabled = itemsEnable;
            editToolStripMenuItem.Enabled = itemsEnable;
            transportControl.ReadOnly = true;
        }

        /// <summary>
        /// Обновить (Меню)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            iTransportBindingSource.DataSource = _transportList;
        }

        /// <summary>
        /// Обновить (Кнопка)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonReset_Click(object sender, EventArgs e)
        {
            iTransportBindingSource.DataSource = _transportList;
        }

        /// <summary>
        /// Дополнительная информация
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridContent_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            transportControl.ReadOnly = true;
            transportControl.Transport = (ITransport)iTransportBindingSource.Current;
        }

        private void comboBoxSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBoxSearchFuelType.Visible = (string)comboBoxSearch.SelectedItem == "Тип топлива";
        }

        /// <summary>
        /// Проверка на проезд.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonTest_Click(object sender, EventArgs e)
        {
            var transport = (ITransport) iTransportBindingSource.Current;
            if (textBoxDistance.Text != string.Empty)
            {
                if (transport.IsCanBeOvercomeDistance(Convert.ToDouble(textBoxDistance.Text)))
                {
                    _messageServices.ShowMessage("Проезд возможен.");
                }
                else
                {
                    _messageServices.ShowMessage("Проезд невозможен.");
                }
            }
            else 
            _messageServices.ShowExclamation("Поле 'Дистанция' не может быть пустым!");
        }
    }
}
