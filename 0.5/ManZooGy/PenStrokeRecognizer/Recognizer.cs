using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace PenStrokeRecognizer
{
    // Pen stroke recognition
    // ref: http://blog.monstuff.com/archives/000012.html

    // TODO:
    // Make a AlphabetRecognizer and DigitRecognizer objects, inheriting from Recognizer with:
    // -the array of points defining letters and for each a reference string code
    // -a hook PostNormalizationHook, that processes the data some more and 
    // 		the reference string code and returns a "code" 
    // 		 (either letter or action like backspace)

    public abstract class Recognizer
    {
        // number of letters in the reference alphabet
        protected int refAlphabetSize;

        // number of points to interpolate in the letters definitions
        protected int subDivisionMax;

        // stdx and stdy are normalized versions of the reference alphabet
        protected float[,] stdx;
        protected float[,] stdy;
        protected string[] stdtoken; // representation of reference letter


        // This should be moved out of this class
        protected int eventMax;
        protected int lastEventIndex = 0; // Current index in x and y
        // x and y hold the current drawing on the screen
        protected int[] x;
        protected int[] y;

        // x2 and y2 are normalized and interpolated versions of x an y
        protected float[] x2;
        protected float[] y2;


        // letterxNorm and letteryNorm holds the normalized version of a line. 
        // Used to normalize the reference alphabet and the current line.
        protected float[] letterxNorm;
        protected float[] letteryNorm;

        // Accumulated length of line, used for interpolation
        protected float[] length;

        // Stores the distance between x and y and the reference alphabet
        protected float[] distances;

        // You need to implement this in concret class
        // Call Initialize with your alphabetSize and load your reference 
        // 		alphabet with NormalizeRefLetters
        public abstract void Initialize();

        protected void Initialize(int alphabetSize, int eventMax)
        {
            refAlphabetSize = alphabetSize;
            subDivisionMax = 25;
            stdx = new float[alphabetSize, subDivisionMax];
            stdy = new float[alphabetSize, subDivisionMax];
            stdtoken = new string[alphabetSize];

            this.eventMax = eventMax;
            lastEventIndex = 0;
            x = new int[eventMax];
            y = new int[eventMax];
            x2 = new float[subDivisionMax];
            y2 = new float[subDivisionMax];

            letterxNorm = new float[eventMax];
            letteryNorm = new float[eventMax];

            length = new float[eventMax];
            distances = new float[alphabetSize];
        }

        // Accumulate points in x and y
        public void AddPoint(Point p, bool first)
        {
            if (first) { lastEventIndex = 0; }

            x[lastEventIndex] = p.X;
            y[lastEventIndex] = p.Y;
            lastEventIndex = (lastEventIndex + 1) % eventMax;
        }

        // Normalize a letter (letterx, lettery with numpoints) into 
        // 		letterxInterpol and letteryInterpol
        protected void NormalizeLetter(int[] letterx, int[] lettery, int numpoints,
                              float[] letterxInterpol, float[] letteryInterpol)
        {
            //int numpoints = letterx.Length; // number of points?
            int minx = letterx[0]; // min des letterx
            int miny = lettery[0];  // min des lettery
            int maxx = letterx[0]; // max des letterx
            int maxy = lettery[0]; // max des lettery
            for (int index = 0; index < numpoints; index++)
            {
                if (letterx[index] < minx)
                    minx = letterx[index];
                if (lettery[index] < miny)
                    miny = lettery[index];
                if (letterx[index] > maxx)
                    maxx = letterx[index];
                if (lettery[index] > maxy)
                    maxy = lettery[index];
            }

            String s = "";
            if ((maxx - minx) + (maxy - miny) < 0)
            {
                s = "dot";
            }
            else if (maxy == miny || (maxx - minx) / (maxy - miny) > 6)
            {
                s = "horizontal";
            }
            else if (maxx == minx || (maxy - miny) / (maxx - minx) > 6)
            {
                s = "vertical";
            }
            else
            {
                s = "fat";
            }

            // initialize letterxNorm and letteryNorm arrays
            // letterxNorm and letteryNorm are normalized versions 
            // 		(values from 0 to 1) with 0 being a bit sticky ;-)
            for (int index = 0; index < numpoints; index++)
            {
                if (s.Equals("vertical") || s.Equals("point"))
                {
                    letterxNorm[index] = 0.0F;
                }
                else
                {
                    letterxNorm[index] = ((0.0F + (float)letterx[index]) - (float)minx) / ((0.0F + (float)maxx) - (float)minx);
                }
                if (s.Equals("horizontal") || s.Equals("point"))
                {
                    letteryNorm[index] = 0.0F;
                }
                else
                {
                    letteryNorm[index] = ((0.0F + (float)lettery[index]) - (float)miny) / ((0.0F + (float)maxy) - (float)miny);
                }
            }


            // initialize length
            // length represents the accumulated length of the segments
            length[0] = 0.0F;
            for (int index = 1; index < numpoints; index++)
            {
                length[index] = length[index - 1] +
                        (float)Math.Sqrt((letterxNorm[index] - letterxNorm[index - 1]) * (letterxNorm[index] - letterxNorm[index - 1]) +
                                        (letteryNorm[index] - letteryNorm[index - 1]) * (letteryNorm[index] - letteryNorm[index - 1]));
            }



            // interpolate letterxNorm and letteryNorm to have subDivisionMax 
            // 		definition points for each letter.
            for (int subDivisionIndex = 0; subDivisionIndex < subDivisionMax; subDivisionIndex++)
            {
                int index = 1; // index in the "length" array

                // find the index of the first point after "percentage" of the path
                float totalLength = length[numpoints - 1];
                float percentage = ((float)subDivisionIndex / (float)(subDivisionMax - 1));
                float percentageLength = totalLength * percentage;
                for (index = 1; length[index] < percentageLength; index++) ;

                // security check
                if (index > numpoints - 1)
                {
                    index = numpoints - 1;
                }

                float f3;
                if (length[index] == length[index - 1])
                {
                    f3 = 0.0F;
                }
                else
                {
                    f3 = (percentageLength - length[index]) / (length[index - 1] - length[index]);
                }

                // letterxInterpol and letteryInterpol are interpolations of letterxNorm and letteryNorm
                letterxInterpol[subDivisionIndex] = f3 * letterxNorm[index - 1] + (1.0F - f3) * letterxNorm[index];
                letteryInterpol[subDivisionIndex] = f3 * letteryNorm[index - 1] + (1.0F - f3) * letteryNorm[index];
            }
        }

        // Load the reference alphabet in stdx, stdy and stdtoken
        public void NormalizeRefLetters(int[][] refLettersX, int[][] refLettersY,
                                        string[] refLetter, int refAlphabetSize)
        {
            if (refLettersX.Length != refAlphabetSize ||
                refLettersY.Length != refAlphabetSize ||
                refLetter.Length != refAlphabetSize) { return; }

            // Temporary storage to store a letter from the refAlphabet			    
            float[] refLetterX = new float[subDivisionMax];
            float[] refLetterY = new float[subDivisionMax];

            for (int i = 0; i < refAlphabetSize; i++)
            {
                for (int subDivisionIndex = 0; subDivisionIndex < subDivisionMax; subDivisionIndex++)
                {
                    refLetterX[subDivisionIndex] = stdx[i, subDivisionIndex];
                    refLetterY[subDivisionIndex] = stdy[i, subDivisionIndex];
                }

                NormalizeLetter(refLettersX[i], refLettersY[i], refLettersX[i].Length, refLetterX, refLetterY);

                for (int subDivisionIndex = 0; subDivisionIndex < subDivisionMax; subDivisionIndex++)
                {
                    stdx[i, subDivisionIndex] = refLetterX[subDivisionIndex];
                    stdy[i, subDivisionIndex] = refLetterY[subDivisionIndex];
                }

                stdtoken[i] = refLetter[i];
            }
        }

        // Normalize the current letter (x and y)
        // Find the one in the reference alphabet that is the closest
        // Return the corresponding reference token
        public string RecognizeBasic()
        {
            NormalizeLetter(x, y, lastEventIndex, x2, y2);

            for (int letterIndex = 0; letterIndex < refAlphabetSize; letterIndex++)
            {
                distances[letterIndex] = stdDistance(x2, y2, letterIndex);
            }
            int minIndex = minimumIndex(distances);
            return stdtoken[minIndex];
        }

        public abstract string Recognize();


        public int minimumIndex(float[] list)
        {
            float min = list[0];
            int minIndex = 0;
            for (int i = 1; i < list.Length; i++)
            {
                if (list[i] < min)
                {
                    min = list[i];
                    minIndex = i;
                }
            }
            return minIndex;
        }

        public int minimumIndexVerify(float[] list, int badindex)
        {
            float min = 15;
            int minIndex = 0;
            for (int i = 1; i < list.Length; i++)
            {
                if (list[i] < min && i != badindex)
                {
                    min = list[i];
                    minIndex = i;
                }
            }
            return minIndex;
        }

        // Measure the distance between a drawn line and a reference letter
        public float stdDistance(float[] charX, float[] charY, int stdIndex)
        {
            float distance = 0.0F;
            for (int i = 0; i < subDivisionMax; i++)
            {
                distance = (float)((double)distance + Math.Sqrt((stdx[stdIndex, i] - charX[i]) * (stdx[stdIndex, i] - charX[i]) + (stdy[stdIndex, i] - charY[i]) * (stdy[stdIndex, i] - charY[i])));
            }

            return distance;
        }

        public string corner(int index)
        {
            if (index < 0 || index >= subDivisionMax) { return "wow"; }

            if (x2[index] < 0.5 &&
                y2[index] < 0.5) { return "topleft"; }
            if (x2[index] > 0.5 &&
                y2[index] < 0.5) { return "topright"; }
            if (x2[index] < 0.5 &&
                y2[index] > 0.5) { return "bottomleft"; }
            return "bottomright";
        }

        public string tangent(int index)
        {
            if (!(index > 0) || !(index < subDivisionMax)) { return "wow"; }
            float vx = x2[index] - x2[index - 1];
            float vy = y2[index] - y2[index - 1];
            string resultx = "";
            string resulty = "";

            if (vx >= 0)
            {
                resultx = "right";
            }
            else
            {
                resultx = "left";
            }

            if (vy >= 0)
            {
                resulty = "bottom";
            }
            else
            {
                resulty = "top";
            }

            if (Math.Abs(vx) > Math.Abs(vy))
            {
                return resultx + resulty;
            }
            else
            {
                return resulty + resultx;
            }
        }

        public string Verify()
        {
            float minScore = 15;
            int minLetter1Index = 0;
            int minLetter2Index = 0;
            for (int index = 0; index < refAlphabetSize; index++)
            {
                for (int index2 = 0; index2 < subDivisionMax; index2++)
                {
                    AddPoint(new Point((int)(stdx[index, index2] * 100), (int)(stdy[index, index2] * 100)), index2 == 0 ? true : false);
                }
                RecognizeBasic();
                int minIndex = minimumIndexVerify(distances, index);
                if (distances[minIndex] < minScore)
                {
                    minScore = distances[minIndex];
                    minLetter1Index = index;
                    minLetter2Index = minIndex;
                }
            }
            return "" + minLetter1Index + "(" + stdtoken[minLetter1Index] + ") - " +
                        minLetter2Index + "(" + stdtoken[minLetter2Index] + ") = " + minScore;
        }
    }
}
