using UnityEngine;
using Accord.MachineLearning.DecisionTrees;
using Accord.MachineLearning.DecisionTrees.Learning;
using Accord.Math.Optimization.Losses;
using Accord.Statistics.Analysis;
using System.Collections.Generic;
using System.Linq;

public class DecisionTreeClassifier : MonoBehaviour
{
    public int PredictAttack(DecisionTree decisionTree, List<int> inputSample)
    {
        int[] inputArray = inputSample.ToArray();
        return decisionTree.Decide(inputArray);
    }

    public DecisionTree TrainDecisionTree(int[][] input, int windowSize)
    {
        List<(int[], int)> preprocessedData = PreprocessDataSlidingWindow(input, windowSize);

        int[][] preprocessedInputs = preprocessedData.Select(x => x.Item1).ToArray();
        int[] preprocessedOutputs = preprocessedData.Select(x => x.Item2).ToArray();

        // Create a new decision tree with `windowSize` attributes and 4 possible output classes (0, 1, 2, or 3)
        DecisionVariable[] variables = new DecisionVariable[windowSize];
        for (int i = 0; i < windowSize; i++)
        {
            variables[i] = new DecisionVariable("Feature" + (i + 1), 4);
        }
        var tree = new DecisionTree(variables, 4);

        // Create a new instance of the C4.5 learning algorithm
        var teacher = new C45Learning(tree);

        // Train the decision tree using the preprocessed inputs and outputs
        teacher.Learn(preprocessedInputs, preprocessedOutputs);

        // Evaluate the accuracy of the decision tree
        var result = new GeneralConfusionMatrix(predicted: tree.Decide(preprocessedInputs), expected: preprocessedOutputs);
        Debug.Log("Decision tree accuracy: " + result.Accuracy);

        return tree;
    }

    private List<(int[], int)> PreprocessDataSlidingWindow(int[][] input, int windowSize)
    {
        List<(int[], int)> preprocessedData = new List<(int[], int)>();

        for (int i = 0; i < input.Length; i++)
        {
            for (int j = 0; j < input[i].Length - windowSize; j++)
            {
                int[] window = new int[windowSize];

                for (int k = 0; k < windowSize; k++)
                {
                    window[k] = input[i][j + k];
                }

                int correspondingOutput = input[i][j + windowSize];
                preprocessedData.Add((window, correspondingOutput));
            }
        }

        return preprocessedData;
    }
}