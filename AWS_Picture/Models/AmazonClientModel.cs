using Amazon.Rekognition;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Win32;
using System.Windows;
using System;
using System.IO;

namespace AWS_Picture.Models
{
    public class AmazonClientModel
    {
        private const string AccessKey = "AKIATTQYODVYN3CBMGWB";
        private const string SecretKey = "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";
        static Amazon.RegionEndpoint regionEndpoint = Amazon.RegionEndpoint.USEast2;
        public const string BucketName = "testbucketinstep";

        public string GetAccessKey
        {
            get => AccessKey;
        }

        public string GetSecretKey
        {
            get => SecretKey;
        }

        public string GetBucketname
        {
            get => BucketName;
        }

        public Amazon.RegionEndpoint Region
        {
            get => regionEndpoint;
        }
    }
}
