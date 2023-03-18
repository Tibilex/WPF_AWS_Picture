using Amazon.S3.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Amazon.S3;
using AWS_Picture.ViewModels;
using Amazon.Rekognition.Model;
using Amazon.Rekognition;
using System.Net;
using System.Windows.Documents;
using Amazon.S3.Transfer;
using System.Windows.Media.Imaging;

namespace AWS_Picture.Models
{
    
    public class Services : ViewModelBase
    {

        private AmazonClientModel _amazonClientModel;
        private IAmazonS3 _amazonS3Cliient;
        private AmazonRekognitionClient _rekognitionClient;
        private ListObjectsResponse _listObjectsResponse;
        private List<FaceDetail> _faceDetails;
        private Amazon.S3.Model.S3Object _selectedS3Object;
        private TransferUtility _fileTransferUtility;
        private System.Windows.Controls.Image Image;

        public ListObjectsResponse ListObjectsResponse
        {
            get => _listObjectsResponse; 
            set
            {
                _listObjectsResponse = value;
                OnPropertyChanged("ListObjectsResponse");
            }
        }
        
        public List<FaceDetail> FaceDetails
        {
            get => _faceDetails; 
            set
            {
                _faceDetails = value;
                OnPropertyChanged("FaceDetails");
            }
        }

        public Amazon.S3.Model.S3Object SelectedS3Object
        {
            get => _selectedS3Object; 
            set
            {
                _selectedS3Object = value;
                OnPropertyChanged("SelectedS3Object");
            }
        }
        public System.Windows.Controls.Image ImageSource
        {
            get => Image;
            set
            {
                Image = value;
                OnPropertyChanged("ImageSource");
            }
        }
        public Services()
        {
            _amazonClientModel = new();
            _amazonS3Cliient = new AmazonS3Client(_amazonClientModel.GetAccessKey, _amazonClientModel.GetSecretKey,
                _amazonClientModel.Region);
            _rekognitionClient = new AmazonRekognitionClient(_amazonClientModel.GetAccessKey,
                _amazonClientModel.GetSecretKey,
                _amazonClientModel.Region);
            _fileTransferUtility = new TransferUtility(_amazonClientModel.GetAccessKey, _amazonClientModel.GetSecretKey,
                _amazonClientModel.Region);
        }
        public async void UploadFile()
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog().Value)
                {
                    var putRequest = new PutObjectRequest
                    {
                        BucketName = _amazonClientModel.GetBucketname,
                        Key = Path.GetFileName(openFileDialog.FileName),
                        FilePath = openFileDialog.FileName,
                        ContentType = "text/plain"
                    };

                    putRequest.Metadata.Add("x-amz-meta-title", "someTitle");
                    PutObjectResponse response = _amazonS3Cliient.PutObjectAsync(putRequest).Result;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public async void DeleteFile()
        {
            try
            {
                await _amazonS3Cliient.DeleteObjectAsync(_amazonClientModel.GetBucketname, SelectedS3Object.Key);
                ShowData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        public async void ShowData()
        {
            try
            {
                ListObjectsResponse = await _amazonS3Cliient.ListObjectsAsync(_amazonClientModel.GetBucketname);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public async void FaceDetect()
        {
            try
            {
                DetectFacesRequest detectlabelsRequest = new DetectFacesRequest()
                {
                    Image = new Image()
                    {
                        S3Object = new Amazon.Rekognition.Model.S3Object()
                            { Name = SelectedS3Object.Key, Bucket = _amazonClientModel.GetBucketname },
                    },
                    Attributes = new List<String>() { "ALL" }
                };
                DetectFacesResponse detectLabelsResponse = await _rekognitionClient.DetectFacesAsync(detectlabelsRequest);
                FaceDetails = detectLabelsResponse.FaceDetails;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void ShowImage()
        {
            _fileTransferUtility.Download(Environment.CurrentDirectory, _amazonClientModel.GetBucketname, SelectedS3Object.Key);
            BitmapImage src = new BitmapImage();
            src.BeginInit();
            src.UriSource = new Uri(SelectedS3Object.Key, UriKind.Relative);
            src.CacheOption = BitmapCacheOption.OnLoad;
            src.EndInit();

            ImageSource = new System.Windows.Controls.Image();
            ImageSource.Source = src;
        }
    }
}
