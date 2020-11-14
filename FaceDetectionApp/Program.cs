using System;
using System.Linq;

using DlibDotNet;
using Dlib = DlibDotNet.Dlib;
using FaceDetectionLibrary;

/// <summary>
/// This program is an example of usage of the facial detector. In particular, we will identify the faces in an image, and 
/// draw some landmarks.
/// 
/// Requires DlibDotNet (you can get it with NuGet).
/// </summary>
class Program
{
    static string imgFilePath = "[PATH_TO_YOUR_PIC]";
    static string facialLandmarksSerializedPredictor = "[PATH_TO_THIS_FOLDER]/Resources/shape_predictor_68_face_landmarks.dat";

    static void Main(string[] args)
    {
        // Read image in
        var img = Dlib.LoadImage<RgbPixel>(imgFilePath);

        // Let's detect faces and draw rectangles around them
        FaceDetector faceDetector = new FaceDetector(facialLandmarksSerializedPredictor);
        Rectangle[] facesBoundingBoxes = faceDetector.DetectFacesBoundingBoxes(img);
        
        foreach (var bb in facesBoundingBoxes)
        {
            Dlib.DrawRectangle(img, bb, color: new RgbPixel(0, 0, 255), thickness: 3);
        }

        // Draw eyes bounding box for subject (i.e., largest) face
        if (facesBoundingBoxes.Length > 0)
        {
            // Example code if you wish to do this only on the largest face
            /*Rectangle subjectFaceBoundingBox = new Rectangle(0, 0);
            foreach (var bb in facesBoundingBoxes)
            {
                if (bb.Area > subjectFaceBoundingBox.Area)
                {
                    subjectFaceBoundingBox = bb;
                }
            }*/

            // Here we do it on all faces
            foreach(var subjectFaceBoundingBox in facesBoundingBoxes) { 

                // Next, obtain facial landmarks
                var landmarks = faceDetector.DetectFacialLandmarks(img, subjectFaceBoundingBox);
                // We also draw them
                foreach (Point p in landmarks)
                {
                    Dlib.DrawRectangle(img, new Rectangle(p), color: new RgbPixel(255, 0, 0), thickness: 3);
                }

                // Now draw bounding box around the eyes
                var topLeft = new Point(landmarks[FacialLandmarks.RIGHT_EYEBROW].X, landmarks[FacialLandmarks.RIGHT_EYEBROW].Y);
                var bottomRight = new Point(landmarks[FacialLandmarks.LEFT_EYEBROW].X, landmarks[FacialLandmarks.UPPER_NOSE].Y);
                var eyesBoundingBox = new Rectangle(topLeft, bottomRight);
                Dlib.DrawRectangle(img, eyesBoundingBox, color: new RgbPixel(0, 255, 0), thickness: 3);

            }
        }

        // Create output file path (for later)
        string outFilePath;
        var tmpStrArray = imgFilePath.Split('/');
        var extension = tmpStrArray[tmpStrArray.Length - 1].Split('.')[1];
            outFilePath = String.Join('/', tmpStrArray.SkipLast(1).ToArray()) + '/' +
                tmpStrArray[tmpStrArray.Length - 1].Replace("."+extension, "_out."+extension);
        Console.WriteLine(outFilePath);

        // Write img
        faceDetector.WriteImageToFilePath(img, outFilePath);
    }
}
