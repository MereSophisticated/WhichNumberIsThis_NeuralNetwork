using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NevronskaMreža
{
    class NeuralNetwork
    {
        int[] layer;
        Layer[] layers;
        public NeuralNetwork(int[] layer) //each number in array is number of neurons in a layer
        {
            this.layer = new int[layer.Length];
            for(int i=0; i < layer.Length; i++)
            {
                this.layer[i] = layer[i];
            }
            layers = new Layer[layer.Length-1];

            //create layers
            for(int i =0; i < layers.Length; i++)
            {
                layers[i] = new Layer(layer[i], layer[i + 1]);  //Layer(number of inputs, number of outputs)
            }
        }

        //FeedForwarda po celotni nevronski mreži
        public double[] FeedForward(double[] inputs)
        {
            layers[0].FeedForward(inputs); //Input goes to first layer of layers

            for(int i =1; i < layers.Length; i++)
            {
                layers[i].FeedForward(layers[i-1].outputs); //feedforward gets output of the previous layer
            }
            return layers[layers.Length -1].outputs; //last layers output values
        }


        public void BackProp(double[] expected)
        {
            //FeedForward every single layer
            for(int i = layers.Length -1; i >= 0; i--)
            {
                if(i == layers.Length - 1)
                {
                    layers[i].BackPropagationOutput(expected);
                }
                else
                {
                    layers[i].BackPropagationHidden(layers[i + 1].gamma, layers[i + 1].weights); //BackPropHidden(gamma foward, weights forward)
                } 
            }

            //iterate over all layers and update weights
            for(int i = 0; i < layers.Length; i++)
            {
                layers[i].UpdateWeights();
            }
        }
    }
}
