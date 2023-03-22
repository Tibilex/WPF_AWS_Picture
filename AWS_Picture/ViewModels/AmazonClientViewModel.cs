using Amazon.Rekognition.Model;
using Amazon.S3.Model;
using Amazon.S3;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using AWS_Picture.Models;
using Microsoft.Win32;
using AWS_Picture.Commands;

namespace AWS_Picture.ViewModels
{
    public class AmazonClientViewModel : ViewModelBase
    {
        #region Fields

        private OpenFileDialog _openfiledialog;
        private Services _services;
        public Services Services { get; set; }

        #endregion

        #region Commands

        public CommandResult? UploadFileButton { get; private set; }
        public CommandResult? ShowAllFilesButton { get; private set; }
        public CommandResult? DeleteFileButton { get; private set; }
        public CommandResult? FacesDetectButton { get; private set; }

        #endregion

        public AmazonClientViewModel()
        {
            _openfiledialog = new OpenFileDialog();
            Services = new Services();

            ShowDataInDisplay();
            CommandsLoad();
        }

        private void CommandsLoad()
        {
            UploadFileButton = new CommandResult(UploadFile);
            DeleteFileButton = new CommandResult(DeleteFile);
            ShowAllFilesButton = new CommandResult(ShowDataInDisplay);
            FacesDetectButton = new CommandResult(FaceDetect);
        }

        public void UploadFile()
        {
            Services.UploadFile();
            ShowDataInDisplay();
        }

        public void DeleteFile()
        {
            Services.DeleteFile();
            ShowDataInDisplay();
        }

        private void ShowDataInDisplay()
        {
            Services.ShowData();
        }

        public void FaceDetect()
        {
            Services.FaceDetect();
        }
    }
}
