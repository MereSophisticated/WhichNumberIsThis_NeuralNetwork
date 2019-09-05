using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NevronskaMreža
{
    public class Layer
    {
        int numberOfInputs; //Number of neurons in the previous layer
        int numberOfOutputs; //number of neurons in the current layer

        public double[] outputs;
        double[] inputs;
        public double[,] weights;
        double[,] weightsDelta;
        public double[] gamma;
        double[] error;
        public static Random random = new Random();
        public Layer(int numberOfInputs, int numberOfOutputs)
        {
            this.numberOfInputs = numberOfInputs;
            this.numberOfOutputs = numberOfOutputs;

            outputs = new double[numberOfOutputs];
            inputs = new double[numberOfInputs];
            weights = new double[numberOfOutputs, numberOfInputs];
            weightsDelta = new double[numberOfOutputs, numberOfInputs];
            gamma = new double[numberOfOutputs];
            error = new double[numberOfOutputs];


            InitializeWeights();
        }

        public void InitializeWeights()
        {
            for (int i = 0; i < numberOfOutputs; i++)
            {
                for (int j = 0; j < numberOfInputs; j++)
                {
                    weights[i, j] = random.NextDouble() - 0.5f;
                }
            }
        }
        public double[] FeedForward(double[] inputs)
        {
            this.inputs = inputs;

            //all neurons in current layer
            for (int i = 0; i < numberOfOutputs; i++)
            {
                outputs[i] = 0;
                //all neurons in previous layer
                for (int j = 0; j < numberOfInputs; j++)
                {
                    outputs[i] += inputs[j] * weights[i, j];
                }

                outputs[i] = Math.Tanh(outputs[i]);
            }
            return outputs;
        }

        public double TanHDerivative(double value)
        {
            return 1 - (value * value);
        }
        public void BackPropagationOutput(double[] expected)
        {
            //calculating errors
            for (int i = 0; i < numberOfOutputs; i++)
            {
                error[i] = outputs[i] - expected[i];
            }

            //calculating gamma values (gamma formula)
            for (int i = 0; i < numberOfOutputs; i++)
            {
                gamma[i] = error[i] * TanHDerivative(outputs[i]);
            }

            //calculate derivative for every single weight in the output layer
            //previous layers outputs ends up having the same reference in memory as the input of the current layer
            for (int i = 0; i < numberOfOutputs; i++)
            {
                for (int j = 0; j < numberOfInputs; j++)
                {
                    weightsDelta[i, j] = gamma[i] * inputs[j];
                }
            }
        }

        public void BackPropagationHidden(double[] gammaForward, double[,] weightsForward)
        {
            //iterate over all gamma values of the hidden layer
            for (int i = 0; i < numberOfOutputs; i++)
            {
                gamma[i] = 0;
                //iterate over all gamma values of the forward layer
                for (int j = 0; j < gammaForward.Length; j++)
                {
                    gamma[i] += gammaForward[j] * weightsForward[j, i];
                }
                gamma[i] *= TanHDerivative(outputs[i]);
            }

            //get new delta values
            for (int i = 0; i < numberOfOutputs; i++)
            {
                for (int j = 0; j < numberOfInputs; j++)
                {
                    weightsDelta[i, j] = gamma[i] * inputs[j];
                }
            }
        }

        public void UpdateWeights()
        {
            for (int i = 0; i < numberOfOutputs; i++)
            {
                for (int j = 0; j < numberOfInputs; j++)
                {
                    weights[i, j] -= weightsDelta[i, j] * MainWindow.learningRate; 
                }
            }
        }
    }
}
