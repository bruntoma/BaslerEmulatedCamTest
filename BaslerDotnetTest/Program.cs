using Basler.Pylon;

namespace BaslerDotnetTest;

class Program
{
    static void Main(string[] args)
    {
        Environment.SetEnvironmentVariable("PYLON_CAMEMU", "1");
        // Create a camera object that selects the first camera device found.
        // More constructors are available for selecting a specific camera device.
        using (Camera camera = new Camera())
        {
            // Open the connection to the camera device.
            camera.Open();
            

            // Start grabbing.
            camera.Parameters[PLCamera.AcquisitionMode].SetValue(PLCamera.AcquisitionMode.Continuous);
            camera.StreamGrabber.Start(GrabStrategy.LatestImages, GrabLoop.ProvidedByUser);

            // Grab a number of images.
            for (int i = 0; i < 10; ++i)
            {
                // Wait for an image and then retrieve it. A timeout of 5000 ms is used.
                IGrabResult grabResult = camera.StreamGrabber.RetrieveResult(5000, TimeoutHandling.ThrowException);
                using (grabResult)
                {
                    // Image grabbed successfully?
                    if (grabResult.GrabSucceeded)
                    {
                        // Access the image data.
                        Console.WriteLine("SizeX: {0}", grabResult.Width);
                        Console.WriteLine("SizeY: {0}", grabResult.Height);
                        byte[] buffer = grabResult.PixelData as byte[];
                        ImagePersistence.Save(ImageFileFormat.Png, "file.png", grabResult);
                        
                        Console.WriteLine("Gray value of first pixel: {0}", buffer[0]);
                        Console.WriteLine("");
                    }
                    else
                    {
                        Console.WriteLine("Error: {0} {1}", grabResult.ErrorCode, grabResult.ErrorDescription);
                    }
                }
            }

            // Stop grabbing.
            camera.StreamGrabber.Stop();
            
            // Close the connection to the camera device.
            camera.Close();
        }
    }
}