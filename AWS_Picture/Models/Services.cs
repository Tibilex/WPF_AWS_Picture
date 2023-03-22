using Amazon.S3.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Amazon.S3;
using AWS_Picture.ViewModels;
using Amazon.Rekognition.Model;
using Amazon.Rekognition;
using System.Net;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Amazon.S3.Transfer;
using System.Drawing;
using Color = System.Drawing.Color;
using System.Reflection;

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
        private System.Windows.Controls.Image _image;

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
            get => _image;
            set
            {
                _image = value;
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
                using (var client = new WebClient())
                {
                    BitmapImage bitmap = ConvertByteToImage(client.DownloadData("https://" + _amazonClientModel.GetBucketname + ".s3.us-east-2.amazonaws.com/" + SelectedS3Object.Key));
                    System.Windows.Controls.Image img = new();
                    img.Source = bitmap;
                    img.Width = bitmap.Width;
                    img.Height = bitmap.Height;
                    ImageSource = img;

                    foreach (var item in FaceDetails)
                    {
                        Rectangle((double)(item.BoundingBox.Left * img.Width), (double)(item.BoundingBox.Top * img.Height), (int)(item.BoundingBox.Width * img.Width), (int)(item.BoundingBox.Height * img.Height), img);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private BitmapImage ConvertByteToImage(byte[] imgArray)
        {
            using (MemoryStream memoryStreams = new MemoryStream(imgArray))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = memoryStreams;
                image.EndInit();
                return image;
            }
        }
        private void Rectangle(double x, double y, int width, int height, System.Windows.Controls.Image img)
        {
            try
            {

                Rect rect = new Rect(0, 0, img.Width, img.Height);
                DrawingVisual visual = new DrawingVisual();
                
                using (DrawingContext dc = visual.RenderOpen())
                {
                    dc.DrawImage(img.Source, rect);
                    Rect faceRectangle = new Rect(x, y, width + 10, height + 10);
                    dc.DrawRectangle(null, new Pen(getRandomFrameColor(), 4), faceRectangle);
                }

                RenderTargetBitmap rtb = new RenderTargetBitmap((int)rect.Width, (int)rect.Height, 96, 96, PixelFormats.Default);
                rtb.Render(visual);
                img.Source = rtb;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private Brush getRandomFrameColor()
        {
            Random randomColor = new();
            Brush frameColorResult = Brushes.Transparent;
            Type brushesType = typeof(Brushes);
            PropertyInfo[] properties = brushesType.GetProperties();

            int random = randomColor.Next(properties.Length);
            frameColorResult = (Brush)properties[random].GetValue(null, null);
            return frameColorResult;
        }
    }
}
