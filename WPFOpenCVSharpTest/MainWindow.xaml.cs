using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OpenCvSharp;

namespace WPFOpenCVSharpTest
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        List<Win_NewImage> listImage = new List<Win_NewImage>();
        DateTime timeStart = new DateTime();
        public int _nSelWin = -1;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void bn_Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fdlg = new OpenFileDialog();
            fdlg.Filter = "Image (*.bmp, *.png, *.jpg, *.tiff)|*.bmp; *.png; *.jpg; *.tiff";
            if (fdlg.ShowDialog() == true)
            {
                int id = listImage.Count;
                string strFile = fdlg.FileName;
                string strTitle = strFile.Substring(strFile.LastIndexOf("\\") + 1);
                Mat temp = Cv2.ImRead(strFile, ImreadModes.Unchanged);
                fn_NewImage(temp, strTitle);
                fn_WriteLog($"Image Open : {fdlg.FileName}");
            }
        }

        public void fn_ActivatedWindow(Win_NewImage win)
        {
            for (int i = 0; i < listImage.Count; i++)
            {
                if (listImage[i] == win)
                {
                    _nSelWin = i;
                    break;
                }
            }
        }
        public void fn_DestoryWindow(Win_NewImage win)
        {
            for (int i = 0; i < listImage.Count; i++)
            {
                if (listImage[i] == win)
                {
                    listImage.RemoveAt(i);
                    break;
                }
            }
            _nSelWin = listImage.Count - 1;
        }

        private void fn_WriteLog(string strLog)
        {
            listLog.Items.Add($"[{DateTime.Now:HH:mm:ss:fff}] {strLog}");
            listLog.ScrollIntoView(listLog.Items[listLog.Items.Count - 1]);
        }
       

        private void bn_Zoom_In(object sender, RoutedEventArgs e)
        {
            if (listImage.Count > 0)
            {
                double scale = listImage[_nSelWin].fn_GetScale();
                listImage[_nSelWin].fn_Zoom(scale + 0.1);
            }
        }

        private void bn_Zoom_Out(object sender, RoutedEventArgs e)
        {
            if (listImage.Count > 0)
            {
                double scale = listImage[_nSelWin].fn_GetScale();
                listImage[_nSelWin].fn_Zoom(scale - 0.1);
            }
        }

        private void bn_Zoom_1x(object sender, RoutedEventArgs e)
        {
            if (listImage.Count > 0)
            {
                listImage[_nSelWin].fn_Zoom(1.0);
            }
        }

        private void bn_Save_Click(object sender, RoutedEventArgs e)
        {
            if (listImage.Count > 0)
            {
                SaveFileDialog fdlg = new SaveFileDialog();
                fdlg.Filter = "Image (*.bmp, *.png, *.jpg, *.tiff)|*.bmp; *.png; *.jpg; *.tiff";
                fdlg.FileName = DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
                if (fdlg.ShowDialog() == true)
                {
                    Cv2.ImWrite(fdlg.FileName, listImage[_nSelWin].fn_GetImage());
                    fn_WriteLog($"Image Save : {fdlg.FileName}");
                }
            }
        }

        private void bn_Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public void fn_NewImage(Mat matImg, string strTitle = "Image")
        {
            listImage.Add(new Win_NewImage(this));
            listImage[listImage.Count - 1].fn_SetImage(matImg);
            listImage[listImage.Count - 1].Title = strTitle;
            listImage[listImage.Count - 1].Show();
        }

        private void bn_GrayScale_Click(object sender, RoutedEventArgs e)
        {
            if (listImage.Count > 0)
            {
                Mat matSrc = listImage[_nSelWin].fn_GetImage();
                Mat matDst = Mat.Zeros(matSrc.Size(), MatType.CV_8UC1);
                int nWidth = matSrc.Cols;
                int nHeight = matSrc.Rows;
                if (matSrc.Channels() == 3)
                {
                    timeStart = DateTime.Now;
                    Cv2.CvtColor(matSrc, matDst, ColorConversionCodes.BGR2GRAY);
                    fn_WriteLog($"[GrayScale] Color(BGR) to Gray ({(DateTime.Now - timeStart).TotalMilliseconds} ms)");
                    listImage[_nSelWin].fn_SetImage(matDst);
                }
                else
                    fn_WriteLog($"[GrayScale] Not Color Image.");
            }
        }

        private void bn_Inverse_Click(object sender, RoutedEventArgs e)
        {
            if (listImage.Count > 0)
            {
                Mat matSrc = listImage[_nSelWin].fn_GetImage();
                Mat matDst = new Mat();
                timeStart = DateTime.Now;

                Cv2.BitwiseNot(matSrc, matDst);
                
                listImage[_nSelWin].fn_SetImage(matDst);
                fn_WriteLog($"Inverse ({(DateTime.Now - timeStart).TotalMilliseconds} ms)");
            }
        }

        private void bn_LogicalSum_Click(object sender, RoutedEventArgs e)
        {
            if (listImage.Count > 0)
            {
                SubWindow.Win_LogicalSum win = new SubWindow.Win_LogicalSum(listImage);
                if (win.ShowDialog() == true)
                {
                    int nMode = win.nMode;
                    int nIdx1 = win.nSrc1;
                    int nIdx2 = win.nSrc2;
                    Mat matSrc1 = listImage[nIdx1].fn_GetImage();
                    Mat matSrc2 = listImage[nIdx2].fn_GetImage();
                    Mat matDst = new Mat();
                    timeStart = DateTime.Now;
                    switch (nMode)
                    {
                        case 0:
                            Cv2.Add(matSrc1, matSrc2, matDst);
                            break;
                        case 1:
                            Cv2.Subtract(matSrc1, matSrc2, matDst);
                            break;
                        case 2:
                            //Cv2.Average(matSrc1, matSrc2, matDst);
                            break;
                        case 3:
                            //Cv2.Differential()
                            Cv2.Absdiff(matSrc1, matSrc2, matDst);
                            break;
                        case 4:
                            Cv2.BitwiseAnd(matSrc1, matSrc2, matDst);
                            break;
                        case 5:
                            Cv2.BitwiseOr(matSrc1, matSrc2, matDst);
                            break;
                    }
                    fn_WriteLog($"[Logical Sum] {listImage[nIdx1].Title} + {listImage[nIdx2].Title} : {nMode} ({(DateTime.Now - timeStart).TotalMilliseconds} ms)");
                    fn_NewImage(matDst, $"Logical Sum {nMode}");
                }
            }
        }


        private void bn_Resize_Click(object sender, RoutedEventArgs e)
        {
            if (listImage.Count > 0)
            {
                SubWindow.Win_Resize win = new SubWindow.Win_Resize();
                string strTitle = listImage[_nSelWin].Title;
                Mat matSrc = listImage[_nSelWin].fn_GetImage();
                Mat matDst = new Mat();
                win.tb_Width.Text = matSrc.Cols.ToString();
                win.tb_Height.Text = matSrc.Rows.ToString();
                if (win.ShowDialog() == true)
                {
                    int width = 0;
                    int height = 0;
                    int.TryParse(win.tb_Width.Text, out width);
                    int.TryParse(win.tb_Height.Text, out height);
                    timeStart = DateTime.Now;
                    Cv2.Resize(matSrc, matDst, new OpenCvSharp.Size(width, height));
                    fn_WriteLog($"[Resize] {strTitle} ({(DateTime.Now - timeStart).TotalMilliseconds} ms)");
                    fn_NewImage(matDst, $"{strTitle}_Resize");
                }
            }
        }

        private void bn_Rotate_Click(object sender, RoutedEventArgs e)
        {
            if (listImage.Count > 0)
            {
                SubWindow.Win_Rotate win = new SubWindow.Win_Rotate();
                if (win.ShowDialog() == true)
                {
                    string strTitle = listImage[_nSelWin].Title;
                    Mat matSrc = listImage[_nSelWin].fn_GetImage();
                    Mat matDst = new Mat();
                    double dAngle = 0.0;
                    double.TryParse(win.tb_Angle.Text, out dAngle);
                    int width = matSrc.Cols;
                    int height = matSrc.Rows;
                    timeStart = DateTime.Now;
                    Mat matMatrix = Cv2.GetRotationMatrix2D(new Point2f(width / 2f, height / 2f), dAngle, 1.0);
                    Cv2.WarpAffine(matSrc, matDst, matMatrix, new OpenCvSharp.Size(width, height));
                    fn_WriteLog($"[Rotate] {strTitle} ({(DateTime.Now - timeStart).TotalMilliseconds} ms)");
                    fn_NewImage(matDst, $"{strTitle}_Rotate");
                }
            }
        }

        private void bn_Gaussian_Click(object sender, RoutedEventArgs e)
        {
            if (listImage.Count > 0)
            {
                SubWindow.Win_Gaussian win = new SubWindow.Win_Gaussian();
                if (win.ShowDialog() == true)
                {
                    string strTitle = listImage[_nSelWin].Title;
                    Mat matSrc = listImage[_nSelWin].fn_GetImage();
                    Mat matDst = new Mat();
                    double dSigma = 0.0;
                    double.TryParse(win.tb_Sigma.Text, out dSigma);
                    int width = matSrc.Cols;
                    int height = matSrc.Rows;
                    timeStart = DateTime.Now;

                    Cv2.GaussianBlur(matSrc, matDst, new OpenCvSharp.Size(3, 3), dSigma);

                    fn_WriteLog($"[Gaussian] {strTitle} ({(DateTime.Now - timeStart).TotalMilliseconds} ms)");
                    fn_NewImage(matDst, $"{strTitle}_Gaussian");
                }
            }
        }

        private void bn_Canny_Click(object sender, RoutedEventArgs e)
        {
            if (listImage.Count > 0)
            {
                SubWindow.Win_Canny win = new SubWindow.Win_Canny();
                if (win.ShowDialog() == true)
                {
                    string strTitle = listImage[_nSelWin].Title;
                    Mat matSrc = listImage[_nSelWin].fn_GetImage();
                    Mat matDst = new Mat();
                    double dSigma = 0.0;
                    double dLowTh = 0.0;
                    double dHighTh = 0.0;
                    double.TryParse(win.tb_Sigma.Text, out dSigma);
                    double.TryParse(win.tb_LowTh.Text, out dLowTh);
                    double.TryParse(win.tb_HighTh.Text, out dHighTh);
                    int width = matSrc.Cols;
                    int height = matSrc.Rows;
                    timeStart = DateTime.Now;

                    Cv2.GaussianBlur(matSrc, matDst, new OpenCvSharp.Size(3, 3), dSigma);
                    Cv2.Canny(matDst, matDst, dLowTh, dHighTh);

                    fn_WriteLog($"[Canny] {strTitle} ({(DateTime.Now - timeStart).TotalMilliseconds} ms)");
                    fn_NewImage(matDst, $"{strTitle}_Canny");
                }
            }
        }

        private void bn_Threshold_Click(object sender, RoutedEventArgs e)
        {
            if (listImage.Count > 0)
            {
                SubWindow.Win_Threshold win = new SubWindow.Win_Threshold();
                if (win.ShowDialog() == true)
                {
                    string strTitle = listImage[_nSelWin].Title;
                    Mat matSrc = listImage[_nSelWin].fn_GetImage();
                    Mat matDst = new Mat();
                    int nThreshold = 0;
                    int.TryParse(win.tb_Threshold.Text, out nThreshold);
                    int width = matSrc.Cols;
                    int height = matSrc.Rows;
                    timeStart = DateTime.Now;

                    Cv2.Threshold(matSrc, matDst, nThreshold, 255, ThresholdTypes.Binary);

                    fn_WriteLog($"[Threshold] {strTitle} ({(DateTime.Now - timeStart).TotalMilliseconds} ms)");
                    fn_NewImage(matDst, $"{strTitle}_Threshold");
                }
            }
        }

        private void bn_Blob_Click(object sender, RoutedEventArgs e)
        {
            if (listImage.Count > 0)
            {
                string strTitle = listImage[_nSelWin].Title;
                Mat matSrc = listImage[_nSelWin].fn_GetImage();
                if (matSrc.Channels() == 1)
                {
                    Mat matDst = matSrc.Clone();
                    Mat matLabel = new Mat();
                    Mat matStats = new Mat();
                    Mat matCentroids = new Mat();
                    int width = matSrc.Cols;
                    int height = matSrc.Rows;
                    timeStart = DateTime.Now;

                    int NumofLabels = Cv2.ConnectedComponentsWithStats(matSrc, matLabel, matStats, matCentroids);

                    unsafe
                    {
                        byte* pDst = matDst.DataPointer;
                        int* pLabel = (int*)matLabel.DataPointer;
                        int* pStats = (int*)matStats.DataPointer;
                        int* pCentroids = (int*)matCentroids.DataPointer;

                        //---------------------------------------------------------------------------
                        // Label 색칠하기.
                        //for (int stepY = 0; stepY < matLabel.Rows; stepY++)
                        //{
                        //    for (int stepX = 0; stepX < matLabel.Cols; stepX++)
                        //    {
                        //        if (pLabel[stepY * matLabel.Cols + stepX] > 0)
                        //        {
                        //            pDst[stepY * matLabel.Cols + stepX] = 128;
                        //        }
                        //    }
                        //}

                        for(int i = 0; i < NumofLabels; i++)
                        {
                            //pStats[i];
                            Cv2.Rectangle(matDst, new OpenCvSharp.Rect(pStats[i * 5], pStats[(i * 5) + 1], pStats[(i * 5) + 2], pStats[(i * 5) + 3]),new Scalar(128));
                        }
                    }

                    fn_WriteLog($"[Blob] {strTitle} ({(DateTime.Now - timeStart).TotalMilliseconds} ms)");
                    fn_NewImage(matDst, $"{strTitle}_Blob");
                }
                else
                    fn_WriteLog($"[Blob] {strTitle} is Color.");
            }
        }

        private void bn_Erode_Click(object sender, RoutedEventArgs e)
        {
            if (listImage.Count > 0)
            {
                string strTitle = listImage[_nSelWin].Title;
                Mat matSrc = listImage[_nSelWin].fn_GetImage();
                Mat matDst = new Mat();
                int width = matSrc.Cols;
                int height = matSrc.Rows;
                timeStart = DateTime.Now;

                Cv2.Erode(matSrc, matDst, new Mat());

                fn_WriteLog($"[Erode] {strTitle} ({(DateTime.Now - timeStart).TotalMilliseconds} ms)");
                fn_NewImage(matDst, $"{strTitle}_Erode");
            }
        }

        private void bn_Dilate_Click(object sender, RoutedEventArgs e)
        {
            if (listImage.Count > 0)
            {
                string strTitle = listImage[_nSelWin].Title;
                Mat matSrc = listImage[_nSelWin].fn_GetImage();
                Mat matDst = new Mat();
                int width = matSrc.Cols;
                int height = matSrc.Rows;
                timeStart = DateTime.Now;

                Cv2.Dilate(matSrc, matDst, new Mat());

                fn_WriteLog($"[Dilate] {strTitle} ({(DateTime.Now - timeStart).TotalMilliseconds} ms)");
                fn_NewImage(matDst, $"{strTitle}_Dilate");
            }
        }

        private void bn_Opening_Click(object sender, RoutedEventArgs e)
        {
            if (listImage.Count > 0)
            {
                string strTitle = listImage[_nSelWin].Title;
                Mat matSrc = listImage[_nSelWin].fn_GetImage();
                Mat matDst = new Mat();
                int width = matSrc.Cols;
                int height = matSrc.Rows;
                timeStart = DateTime.Now;

                Cv2.Erode(matSrc, matDst, new Mat());
                Cv2.Dilate(matDst, matDst, new Mat());

                fn_WriteLog($"[Opening] {strTitle} ({(DateTime.Now - timeStart).TotalMilliseconds} ms)");
                fn_NewImage(matDst, $"{strTitle}_Opening");
            }
        }

        private void bn_Closing_Click(object sender, RoutedEventArgs e)
        {
            if (listImage.Count > 0)
            {
                string strTitle = listImage[_nSelWin].Title;
                Mat matSrc = listImage[_nSelWin].fn_GetImage();
                Mat matDst = new Mat();
                int width = matSrc.Cols;
                int height = matSrc.Rows;
                timeStart = DateTime.Now;

                Cv2.Dilate(matSrc, matDst, new Mat());
                Cv2.Erode(matDst, matDst, new Mat());

                fn_WriteLog($"[Closing] {strTitle} ({(DateTime.Now - timeStart).TotalMilliseconds} ms)");
                fn_NewImage(matDst, $"{strTitle}_Closing");
            }
        }

        private void bn_Match_Click(object sender, RoutedEventArgs e)
        {
            if (listImage.Count > 0)
            {
                SubWindow.Win_Matching win = new SubWindow.Win_Matching(listImage);
                if (win.ShowDialog() == true)
                {
                    int mode = win.cb_Mode.SelectedIndex;
                    int idxSrc = win.cb_Src.SelectedIndex;
                    int idxTmpl = win.cb_Tmpl.SelectedIndex;
                    string strTitle = listImage[_nSelWin].Title;
                    Mat matSrc = listImage[idxSrc].fn_GetImage();
                    Mat matTmpl = listImage[idxTmpl].fn_GetImage();
                    Mat matDst = new Mat();
                    int width = matSrc.Cols;
                    int height = matSrc.Rows;
                    timeStart = DateTime.Now;

                    if(mode == 0)// Template
                    {
                        Mat matResult = new Mat();
                        Cv2.MatchTemplate(matSrc, matTmpl, matResult, TemplateMatchModes.SqDiffNormed);

                        OpenCvSharp.Point matchLoc = new OpenCvSharp.Point();
                        unsafe
                        {
                            float* pData = (float*)matResult.DataPointer;
                            float fMin = 1.0f;
                            for (int stepY = 0; stepY < matResult.Rows; stepY++)
                            {
                                for (int stepX = 0; stepX < matResult.Cols; stepX++)
                                {
                                    if(fMin >= pData[stepY * matResult.Cols +stepX])
                                    {
                                        fMin = pData[stepY * matResult.Cols + stepX];
                                        matchLoc.X = stepX;
                                        matchLoc.Y = stepY;
                                    }
                                }
                            }
                        }


                        matDst = matSrc.Clone();
                        Cv2.CvtColor(matDst, matDst, ColorConversionCodes.GRAY2BGR);

                        Cv2.Rectangle(matDst, new OpenCvSharp.Rect(matchLoc.X, matchLoc.Y, matTmpl.Cols, matTmpl.Rows), new Scalar(0,255,0));

                    }
                    else if(mode == 1)// SIFT
                    {
                        OpenCvSharp.Features2D.SIFT detector = OpenCvSharp.Features2D.SIFT.Create();
                        KeyPoint[] keypoint1, keypoint2;
                        Mat matDescriptor1 = new Mat();
                        Mat matDescriptor2 = new Mat();
                        detector.DetectAndCompute(matTmpl, new Mat(), out keypoint1, matDescriptor1);
                        detector.DetectAndCompute(matSrc, new Mat(), out keypoint2, matDescriptor2);
                        BFMatcher matcher = new BFMatcher();
                        DMatch[] dMatches = matcher.Match(matDescriptor1, matDescriptor2);
                        if (dMatches.Length > 0)
                        {
                            int GOOD = Math.Min(50, (int)(dMatches.Length * 0.1));
                            DMatch[] dGood = new DMatch[GOOD];
                            for (int step = 0; step < GOOD; step++)
                            {
                                dGood[step] = new DMatch();
                                dGood[step] = dMatches[step];
                            }

                            Cv2.DrawMatches(matTmpl, keypoint1, matSrc, keypoint2, dGood, matDst, Scalar.All(-1), Scalar.All(-1), new List<byte>(), DrawMatchesFlags.NotDrawSinglePoints);
                        }
                    }
                    else if(mode == 2)// SURF
                    {
                        OpenCvSharp.XFeatures2D.SURF detector = OpenCvSharp.XFeatures2D.SURF.Create(800);
                        KeyPoint[] keypoint1, keypoint2;
                        Mat matDescriptor1 = new Mat();
                        Mat matDescriptor2 = new Mat();
                        detector.DetectAndCompute(matTmpl, new Mat(), out keypoint1, matDescriptor1);
                        detector.DetectAndCompute(matSrc, new Mat(), out keypoint2, matDescriptor2);
                        BFMatcher matcher = new BFMatcher();
                        DMatch[] dMatches = matcher.Match(matDescriptor1, matDescriptor2);
                        if (dMatches.Length > 0)
                        {
                            int GOOD = Math.Min(50, (int)(dMatches.Length * 0.1));
                            DMatch[] dGood = new DMatch[GOOD];
                            for (int step = 0; step < GOOD; step++)
                            {
                                dGood[step] = new DMatch();
                                dGood[step] = dMatches[step];
                            }

                            Cv2.DrawMatches(matTmpl, keypoint1, matSrc, keypoint2, dGood, matDst, Scalar.All(-1), Scalar.All(-1), new List<byte>(), DrawMatchesFlags.NotDrawSinglePoints);
                        }
                    }
                    
                    fn_WriteLog($"[Matching] {strTitle} ({(DateTime.Now - timeStart).TotalMilliseconds} ms)");
                    fn_NewImage(matDst, $"Matching {mode}");
                }
            }
        }
    }
}
