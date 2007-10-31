using System;
using System.Collections.Generic;
using System.Text;

namespace PenStrokeRecognizer
{
    // Pen stroke recognition
    // ref: http://blog.monstuff.com/archives/000012.html

    public class DigitRecognizer : Recognizer
    {
        // all distances should be under 7
        public override void Initialize()
        {
            int alphabetSize = refLettersX.Length;
            if (refLettersX.Length != refLettersY.Length ||
                refLettersX.Length != refLetters.Length) { return; }

            Initialize(alphabetSize, 1000);
            // Initialize the normalized and interpolated tables for the letters definitions
            NormalizeRefLetters(refLettersX, refLettersY, refLetters, alphabetSize);
        }

        // Should return a token instead (a letter or a special event)
        public override string Recognize()
        {
            if (lastEventIndex == 1)
                return ""; // Ignore Dots

            String s1 = RecognizeBasic();

            //********** HACKY PART ! **********//

            if (s1.Equals("0") &&
               y2[subDivisionMax - 1] > 0.3)
                s1 = "6";

            if (s1.Equals("6") &&
               y2[subDivisionMax - 1] < 0.3)
                s1 = "0";

            if (s1.Equals("3") &&
               (tangent(1).Equals("bottomright") ||
                tangent(1).Equals("bottomleft") ||
                tangent(1).Equals("leftbottom")))
                s1 = "5";

            if (s1.Equals("5") &&
               (tangent(1).Equals("topright") ||
                tangent(1).Equals("righttop")))
                s1 = "3";

            if (s1.Equals("8") &&
               (tangent(1).Equals("rightbottom") ||
                tangent(1).Equals("righttop")))
                s1 = "three";

            /*
            if(s1.Equals("plus") &&
               (tangent(1).Equals("bottomright") ||
               tangent(subDivisionMax-1).Equals("leftbottom")))
                s1 = "times";
			
            if(s1.Equals("times") &&
               (tangent(1).Equals("righttop") ||
               tangent(subDivisionMax-1).Equals("bottomright")))
                s1 = "plus";
			
            if(s1.Equals("times") &&
               (tangent(1).Equals("righttop") ||
                tangent(subDivisionMax-1).Equals("leftbottom")))
                s1 = "";
			
            if(s1.Equals("plus") &&
               (tangent(1).Equals("bottomright") ||
                tangent(subDivisionMax-1).Equals("rightbottom")))
                s1 = "";
            */

            return s1;
        }



        static string[] refLetters = {
			"0",
			//"0",
			"1",
			"2",
			"3",
			"4",
			"5",
			"6",
			"7",
			"8",
			"9",
			//"9",
			//"minus",
			//"plus",
			//"times",
			//"gt",
			//"lt",
			"back"
		};

        static int[][] refLettersX = {
	    	new int[] {5, 0, 0, 5, 10, 10, 6}, /* zero */
	    	//new int[] {5, 10, 10, 5, 0, 0, 6}, /* zero */
	    	new int[] {5, 5}, /* one */
	    	new int[] {1, 4, 7, 0, 8}, /* two */
	    	new int[] {1, 4, 7, 3, 8, 4, 0}, /* three */
	    	new int[] {3, 2, 6}, /* four */
	    	new int[] {0, 0, 7, 9, 7, 4, 1}, /* five */
	    	new int[] {9, 6, 0, 0, 6, 10, 7, 1}, /* six */
	    	new int[] {0, 10, 10}, /* seven */
	    	new int[] {7, 5, 0, 5, 10, 5, 0, 0, 9}, /* eight */
	    	new int[] {5, 3, 0, 2, 6, 5}, /* nine */
	    	//new int[] {5, 3, 0, 2, 6, 5, 1}, /* nine */
	    	//new int[] {0, 10}, /* minus */
	    	//new int[] {0, 10, 5, 5}, /* plus */
	    	//new int[] {0, 10, 10, 0}, /* times */
	    	//new int[] {0, 10, 0}, /* gt */
	    	//new int[] {10, 0, 10}, /* lt */
        	new int[] {10, 0} /* backspace */
	    };
        static int[][] refLettersY = {
    		new int[] {0, 3, 7, 10, 7, 3, 0}, /* zero */
    		//new int[] {0, 2, 8, 10, 8, 2, 0}, /* zero */
    		new int[] {0, 10}, /* one */
    		new int[] {2, 0, 3, 10, 10}, /* two */
    		new int[] {2, 0, 3, 5, 8, 10, 9}, /* three */
    		new int[] {0, 7, 7}, /* four */
    		new int[] {0, 3, 3, 5, 9, 9, 8}, /* five */
    		new int[] {2, 0, 4, 8, 10, 8, 5, 7}, /* six */
    		new int[] {0, 0, 10}, /* seven */
    		new int[] {1, 0, 2, 4, 7, 10, 8, 6, 2}, /* eight */
    		new int[] {1, 0, 2, 4, 3, 8}, /* nine */
    		//new int[] {3, 0, 2, 4, 3, 8, 7}, /* nine */
    		//new int[] {5, 5}, /* minus */
    		//new int[] {5, 5, 0, 10}, /* plus */
    		//new int[] {0, 10, 0, 10}, /* times */
    		//new int[] {0, 5, 10}, /* gt */
    		//new int[] {0, 5, 10}, /* lt */
        	new int[] {5, 5} /* backspace */ 
    };

    }
}
