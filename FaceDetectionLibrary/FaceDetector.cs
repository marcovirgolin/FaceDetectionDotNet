using DlibDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using Dlib = DlibDotNet.Dlib;

// Source: https://medium.com/machinelearningadvantage/detect-faces-with-c-and-dlib-in-only-40-lines-of-code-f0bb1b929133
namespace FaceDetectionLibrary
{
    public class FaceDetector
    {

        private ShapePredictor facialLandmarkPredictor;

        public FaceDetector(string serializedFacialLandmarkPredictor) {
            this.facialLandmarkPredictor = ShapePredictor.Deserialize(serializedFacialLandmarkPredictor);
        }

        public void WriteImageToFilePath(Array2D<RgbPixel> img, string filepath)
        {
            Dlib.SaveJpeg(img, filepath);
        }

        public Rectangle[] DetectFacesBoundingBoxes(Array2D<RgbPixel> img)
        {
            Rectangle[] facesBoundingBoxes;

            // Set up and apply face detector
            using (var fd = Dlib.GetFrontalFaceDetector())
            {
                facesBoundingBoxes = fd.Operator(img);
            }

            return facesBoundingBoxes;
        }

        public Point[] DetectFacialLandmarks(Array2D<RgbPixel> img, Rectangle faceBoundingBox)
        {
            var shapes = this.facialLandmarkPredictor.Detect(img, faceBoundingBox);

            var numParts = shapes.Parts;
            var landmarkPoints = new List<Point>();
            for (uint i = 0; i < numParts; i++)
            {
                var point = shapes.GetPart(i);
                landmarkPoints.Add(point);
            }
            return landmarkPoints.ToArray();
        }
    }
}
