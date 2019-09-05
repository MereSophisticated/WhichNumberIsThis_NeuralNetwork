
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;


namespace NevronskaMreža
{
    using Point = Tuple<double, double>;
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<LearningInput> learningInputList = new List<LearningInput>(); //network input for learning
        NeuralNetwork net = new NeuralNetwork(new int[] { 20, 25, 25, 6});
        public static double learningRate = 0.3;
        public static int epoch = 3000;
        public MainWindow()
        {
            InitializeComponent();
            
           
            
            

        }

        private void ButtonSubmit_Click(object sender, RoutedEventArgs e)
        {
            var isNumeric = int.TryParse(textboxThisIS.Text, out int n);
            if (!isNumeric)
            {
                MessageBox.Show("Input must be an integer!");
                return;
            }
            List<Point> coordinates = new List<Point>();
            foreach (Stroke s in DrawingCanvas.Strokes)
            {
                foreach (StylusPoint sp in s.StylusPoints)
                {
                    //double X = (double)sp.X;//(float)(sp.X / DrawingCanvas.Width);
                    //double Y = (double)sp.Y;//(float)(sp.Y / DrawingCanvas.Height);
                    Point pointToAdd = new Point(sp.X, sp.Y);
                    coordinates.Add(pointToAdd);
                    //Console.WriteLine(X + "," + Y);
                }
            }
            //coordinates.ForEach(p => Console.WriteLine(p));
            List<Point> pointListOut = DrawingSimplification(coordinates);
            //Console.WriteLine("\n\n\n\n\n\n\n\n\n\nRemaining points");
            //pointListOut.ForEach(p => Console.WriteLine(p));
            //Console.WriteLine(pointListOut.Count);

            List<Point> normalizedPoints = new List<Point>();
            pointListOut.ForEach(p => normalizedPoints.Add(new Point(p.Item1 / DrawingCanvas.Width, p.Item2 / DrawingCanvas.Height)));

            //Console.WriteLine("\n\n\n\n\n\n\n\n\n\nNormalized remaining points");
            //normalizedPoints.ForEach(p => Console.WriteLine(p));

            double[] coordinatesArray = new double[normalizedPoints.Count * 2];
            for(int i =0, j=0; i < normalizedPoints.Count; i++)
            {
                coordinatesArray[j] = normalizedPoints[i].Item1;
                j++;
                coordinatesArray[j] = normalizedPoints[i].Item2;
                j++;
            }
            learningInputList.Add(new LearningInput(coordinatesArray, n)); //Convert.ToInt32(textboxThisIS.Text)

            StylusPointCollection newPoints = new StylusPointCollection();
            pointListOut.ForEach(p => newPoints.Add(new StylusPoint(p.Item1,p.Item2)));

            Stroke newStroke = new Stroke(newPoints);
            DrawingCanvas.Strokes.Clear();
            DrawingCanvas.Strokes.Add(newStroke);

            //for(int i =0; i < coordinatesArray.Length; i++)
            //{
            //    Console.WriteLine(coordinatesArray[i]);
            //}
        }


        private List<Point> DrawingSimplification(List<Point> pointList)
        {
            //Point startPoint = pointList[0];
            //Point endPoint = pointList[pointList.Count - 1];
            Point mid;

           while(pointList.Count > 10)
           {
                for (int i = 1; i < pointList.Count - 1; i += 2)
                {
                    //poišči središče dveh točk
                    mid = MidPoint(pointList[i], pointList[i + 1]);
                    pointList[i] = mid;
                    pointList.RemoveAt(i + 1);
                    if(pointList.Count == 10)
                    {
                        break;
                    }

                }
                
            }


            return pointList;

        }

        private Point MidPoint(Point point1, Point point2)
        {
            Point middle = new Point(  ((point1.Item1 + point2.Item1) / 2.0), ((point1.Item2 + point2.Item2) / 2.0));
            return middle;
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            DrawingCanvas.Strokes.Clear();
        }

        private void ButtonSubmitAll_Click(object sender, RoutedEventArgs e)
        {
            double[] expectedOutput = { 0, 0, 0, 0, 0, 0 };

            for(int i =0, j = 0; i < learningInputList.Count; i++)
            {
                //Console.WriteLine(learningInputList[i].coordinates.Length);
            }
            double meanSquaredError;
            double[] outputValues = new double[6];
            double errorSum;

            do
            {
                errorSum = 0;
                for (int i = 0; i < epoch; i++)  //5000 je epohe
                {
                    for (int j = 0; j < learningInputList.Count; j++)
                    {
                        outputValues = net.FeedForward(learningInputList[j].coordinates);
                        expectedOutput[learningInputList[j].represents - 1] = 1;
                        net.BackProp(expectedOutput);
                        

                        for (int k = 0; k < outputValues.Length; k++)
                        {
                            errorSum += Math.Pow(outputValues[k] - expectedOutput[k], 2);
                        }

                        expectedOutput[learningInputList[j].represents - 1] = 0;
                    }
                }

                meanSquaredError = errorSum / (expectedOutput.Length * epoch);
                Console.WriteLine(meanSquaredError);

            } while (meanSquaredError > 0.005);
            

            //izpis koordinat in kaj te predstavljajo
            //for(int i=0; i < learningInputList.Count; i++)
            //{
            //    Console.WriteLine(learningInputList[i].represents);
            //    for(int j = 0; j < learningInputList[i].coordinates.Length; j++)
            //    {
            //        Console.WriteLine(learningInputList[i].coordinates[j]);

            //    }
            //}


        }

        private void ButtonCheck_Click(object sender, RoutedEventArgs e)
        {
            List<Point> coordinates = new List<Point>();
            foreach (Stroke s in DrawingCanvas.Strokes)
            {
                foreach (StylusPoint sp in s.StylusPoints)
                {
                    float X = (float)sp.X;//(float)(sp.X / DrawingCanvas.Width);
                    float Y = (float)sp.Y;//(float)(sp.Y / DrawingCanvas.Height);
                    Point pointToAdd = new Point(X, Y);
                    coordinates.Add(pointToAdd);
                    //Console.WriteLine(X + "," + Y);
                }
            }

            List<Point> pointListOut = DrawingSimplification(coordinates);
           

            List<Point> normalizedPoints = new List<Point>();
            pointListOut.ForEach(p => normalizedPoints.Add(new Point(p.Item1 / DrawingCanvas.Width, p.Item2 / DrawingCanvas.Height)));


            double[] coordinatesArray = new double[normalizedPoints.Count * 2];
            for (int i = 0, j = 0; i < normalizedPoints.Count; i++)
            {
                coordinatesArray[j] = normalizedPoints[i].Item1;
                j++;
                coordinatesArray[j] = normalizedPoints[i].Item2;
                j++;
            }

            //get last layers output values
            double[] outputValues = new double[6];
            outputValues = net.FeedForward(coordinatesArray);
            //Console.WriteLine("Represents");
            //for(int i =0; i < outputValues.Length; i++)
            //{
            //    Console.WriteLine(outputValues[i]+ " ");
            //}
            double maxValue = outputValues.Max();
            int maxIndex = outputValues.ToList().IndexOf(maxValue);
            int representedNumber = maxIndex + 1;
            MessageBox.Show(representedNumber.ToString());
        }

        private void ChangeSetting_Click(object sender, RoutedEventArgs e)
        {
            var isNumeric = double.TryParse(textBoxLearningRate.Text, out double n);
            var isNumeric2 = int.TryParse(textBoxEpoche.Text, out int n2);
            if (!isNumeric)
            {
                MessageBox.Show("Learning rate must be a double!");
                return;
            }
            if (!isNumeric2)
            {
                MessageBox.Show("Epoch must be an integer!");
            }
            learningRate =n;
            epoch = n2;
        }

        private void ButtonClearAllSubmited_Click(object sender, RoutedEventArgs e)
        {
            learningInputList = new List<LearningInput>();
            net = new NeuralNetwork(new int[] { 20, 25, 25, 6 });
        }
    }
}
