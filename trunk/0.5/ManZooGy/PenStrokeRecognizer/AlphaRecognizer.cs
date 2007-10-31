using System;
using System.Collections.Generic;
using System.Text;

namespace PenStrokeRecognizer
{
    // Pen stroke recognition
    // ref: http://blog.monstuff.com/archives/000012.html

    public class AlphaRecognizer : Recognizer
    {
        public override void Initialize()
        {
            int alphabetSize = refLettersX.Length;
            Initialize(alphabetSize, 1000);
            // Initialize the normalized and interpolated tables for the letters definitions
            NormalizeRefLetters(refLettersX, refLettersY, refLetters, alphabetSize);
        }

        // Should return a token instead (a letter or a special event)
        public override string Recognize()
        {
            // Letters with issues:

            // U, V, O
            // If a U has less than x "horizontal" tangents then V
            // If a U has last tangent to right then V
            // Number of points in each quadrant?

            // L, H

            // K

            // M: confused for N, U and A

            // R, D, P
            // D vs. R: use ending quadrant
            // P ends between 1/3 and 2/3 of [miny, maxy] and tangent to left
            // R last tangent different
            // D is default

            // M, P
            // some badly drawn P are taken for M...
            // some badly drawn p are taken for X...

            // G, C, L: use first tangent

            // R, A: mixed up a lot (when R has small head)

            // p, X: a little bit

            if (lastEventIndex == 1)
                return ""; // Ignore Dots

            String s1 = RecognizeBasic();


            //********** HACKY PART ! **********//

            //			bool flag = checkRight(x2, lastEventIndex); // BUG !! Should be subDivisionMax

            //bool flag = checkRight();
            //bool flag1 = checkLoop(x2, subDivisionMax);

            if (s1.Equals("C") &&
               (tangent(3).Equals("topright") ||
                tangent(3).Equals("righttop") ||
                tangent(3).Equals("rightbottom")))
                s1 = "E";

            if (s1.Equals("B") && corner(subDivisionMax - 1).Equals("bottomright"))
                s1 = "H";
            if (s1.Equals("H") && corner(subDivisionMax - 1).Equals("bottomleft"))
                s1 = "B";

            if (s1.Equals("U") &&
               (tangent(1).Equals("topright") ||
                tangent(1).Equals("topleft")))
                s1 = "N";

            // G that nearly closes at the top is O
            if (s1.Equals("G") && y2[subDivisionMax - 1] < 0.3)
                s1 = "O";

            if (s1.Equals("\\") && corner(0).Equals("topright"))
                s1 = "Q";

            if ((s1.Equals("\\") || s1.Equals("T")) &&
               (tangent(1).Equals("topleft") ||
                tangent(1).Equals("lefttop") ||
                tangent(1).Equals("leftbottom") ||
                tangent(1).Equals("topright")))
                s1 = "Q";

            if (s1.Equals("Q") &&
               (tangent(1).Equals("leftbottom") ||
               tangent(1).Equals("bottomleft")))
                s1 = "K";

            if (s1.Equals("L") && findHfromL())
                s1 = "H";

            if (s1.Equals("J") &&
               (tangent(1).Equals("leftbottom") ||
               tangent(1).Equals("topleft") ||
               tangent(1).Equals("lefttop")))
                s1 = "S";

            // Constraints
            if (s1.Equals("A") &&
               (!corner(0).Equals("bottomleft") ||
                !corner(subDivisionMax - 1).Equals("bottomright")))
                return "";
            // Also verify the start and end tangents


            if (s1.Equals("B") &&
               (!corner(0).Equals("topleft") ||
                !corner(subDivisionMax - 1).Equals("bottomleft")))
                return "";

            if (s1.Equals("C") &&
               (!corner(0).Equals("topright") ||
                !corner(subDivisionMax - 1).Equals("bottomright")))
                return "";

            if (s1.Equals("D") &&
               (!corner(0).Equals("bottomleft") ||
                !corner(subDivisionMax - 1).Equals("bottomleft")))
                return "";

            if (s1.Equals("E") &&
               !corner(subDivisionMax - 1).Equals("bottomright"))
                return "";

            if (s1.Equals("F") &&
               (!corner(0).Equals("topright") ||
                !corner(subDivisionMax - 1).Equals("bottomleft")))
                return "";

            if (s1.Equals("H") &&
               (!corner(0).Equals("topleft") ||
                !corner(subDivisionMax - 1).Equals("bottomright")))
                return "";

            if (s1.Equals("J") &&
               (!corner(0).Equals("topright") ||
                !corner(subDivisionMax - 1).Equals("bottomleft")))
                return "";

            if (s1.Equals("K") &&
               (!corner(0).Equals("topright") ||
                !corner(subDivisionMax - 1).Equals("bottomright")))
                return "";

            if (s1.Equals("L") &&
               (!corner(0).Equals("topleft") ||
                !corner(subDivisionMax - 1).Equals("bottomright")))
                return "";

            if (s1.Equals("M") &&
               (!corner(0).Equals("bottomleft") ||
                !corner(subDivisionMax - 1).Equals("bottomright")))
                return "";

            if (s1.Equals("N") &&
               (!corner(0).Equals("bottomleft") ||
                !corner(subDivisionMax - 1).Equals("topright")))
                return "";

            if (s1.Equals("Q") &&
               (!corner(0).Equals("topright") ||
                !corner(subDivisionMax - 1).Equals("bottomright")))
                return "";

            if (s1.Equals("R") &&
               (!corner(0).Equals("bottomleft") ||
                !corner(subDivisionMax - 1).Equals("bottomright")))
                return "";

            if (s1.Equals("S") &&
               (!corner(0).Equals("topright") ||
                !corner(subDivisionMax - 1).Equals("bottomleft")))
                return "";

            if (s1.Equals("T") &&
               (!corner(0).Equals("topleft") ||
                !corner(subDivisionMax - 1).Equals("bottomright")))
                return "";

            if (s1.Equals("U") &&
               (!corner(0).Equals("topleft") ||
                !corner(subDivisionMax - 1).Equals("topright")))
                return "";


            if (s1.Equals("W") &&
               (!corner(0).Equals("topleft") ||
                !corner(subDivisionMax - 1).Equals("topright")))
                return "";

            if (s1.Equals("Y") &&
               !corner(0).Equals("topleft"))
                return "";

            if (s1.Equals("Z") &&
               (!corner(0).Equals("topleft") ||
                !corner(subDivisionMax - 1).Equals("bottomright")))
                return "";

            return s1;
        }

        // returns true if H		
        public bool findHfromL()
        {
            int counter = 0;
            for (int i = subDivisionMax / 3; i < subDivisionMax; i++)
            {
                if ((-y2[i] + y2[i - 1]) / (x2[i] - x2[i - 1]) > 0.4) counter++;
            }
            if (counter > 1 && tangent(subDivisionMax - 1).Equals("bottomright")) return true;
            return false;
        }

        /*
        // ?
        public bool checkRight()
        {
            float f = 0.05F;
			
            // records the index of the biggest x
            int maximumXIndex = 0;
			
            // ?
            int j = 0;
			
			
            // records which x is the biggest so far
            int[] majoringStairs = new int[lastEventIndex];
			
            // records which index got the biggest x so far
            int[] majoringIndexes  = new int[lastEventIndex];
            majoringStairs[0] = x[0]; 
            majoringIndexes[0] = 0;
            for(int index = 1; index < lastEventIndex; index++) {
                if(x[index] > majoringStairs[index - 1]) {
                    majoringStairs[index] = x[index];
                    majoringIndexes[index] = index;
                    maximumXIndex = index;
                } else {
                    majoringStairs[index] = majoringStairs[index - 1];
                    majoringIndexes[index] = majoringIndexes[index - 1];
                }
            }
	
            for(int index = maximumXIndex - 1; index >= 0; index--) {
                if((float)(majoringStairs[index] - x[index]) > f && 
                       y[majoringIndexes[index]] < y[index] && 
                       y[index] < y[maximumXIndex] && 
                       length[maximumXIndex] - length[index] < 1.0F &&
                       length[index] - length[majoringIndexes[index]] < 1.0F) 
                {
                    return true;
                }
            }
	
            j = 0;
            majoringStairs[lastEventIndex - 1] = x[lastEventIndex - 1];
            majoringIndexes[lastEventIndex - 1] = lastEventIndex - 1;
            for(int k1 = lastEventIndex - 2; k1 >= 0; k1--) {
                if(x[k1] > majoringStairs[k1 + 1]) {
                    majoringStairs[k1] = x[k1];
                    majoringIndexes[k1] = k1;
                    j = k1;
                } else {
                    majoringStairs[k1] = majoringStairs[k1 + 1];
                    majoringIndexes[k1] = majoringIndexes[k1 + 1];
                }
            }
	
            for(int l1 = j + 1; l1 < lastEventIndex; l1++) {
                if((float)(majoringStairs[l1] - x[l1]) > f && y[majoringIndexes[l1]] > y[l1] && y[l1] > y[j] && length[l1] - length[j] < 1.0F && length[majoringIndexes[l1]] - length[l1] < 1.0F) {
                    return true; 
                }
            }
	
            return false;
        }
		
        // ?
        public bool checkLoop(float[] af, int i)
        {
            return (double)af[0] > 0.10000000000000001D && (double)af[0] < 0.90000000000000002D;
        }
        */
        static string[] refLetters = {
			"A",
			// "B",
			// "B", 
			// "B",
			"B", /* new b */
			"C",
			
			"C",
			// "D",
			"D",
			"E",
			"E", /* new e */
			"F",
			
			"G",
			"H",
			"I",
			"J",
			"K",
			
			"L",
			"M",
			"N",
			"O",
			"P",
			
			"P",
			// "Q",
			"Q", /* new q */
			"R",
			"R",
			"S",
			
			"T",
			"U",
			"U",
			"V",
			"V",
			
			"W",
			"X",
			"Y",
			"Z",
			//"Z",
			
			"\\",
			"/",
			"space", // " ", // space로 표시가 되지 않아 변경함. by mio 25 Oct 2007
			"back"
		};

        static int[][] refLettersX = {
        new int[] {0, 5, 10}, /* A */
        // new int[] {0, 0, 0, 10, 10, 5, 10, 10, 0}, /* B */
        //new int[] {0, 0, 10, 10, 0, 10, 10, 0}, /* B */ // remove
        // new int[] {0, 10, 10, 0, 10, 10, 0}, /* B */
        new int[] {0, 0, 0, 3, 3, 0}, /* new b */
        new int[] {10, 0, 0, 10}, /* C */
        
        new int[] {5, 0, 0, 10}, /* C */
        // new int[] {0, 0, 0, 10, 10, 0}, /* D */ // remove
        new int[] {0, 0, 10, 10, 0}, /* D */
        new int[] {10, 0, 0, 3, 0, 0, 10}, /* E */
        new int[] {0, 7, 7, 0, 0, 7}, /* new e */
        new int[] {10, 0, 0}, /* F */
        
        new int[] {10, 0, 0, 10, 10, 5}, /* G */
        new int[] {0, 0, 0, 3, 3},  /* H */
        new int[] {5, 5}, /* I */
        new int[] {10, 10, 0}, /* J */
        new int[] {10, 0, 0, 10},  /* K */
        
        new int[] {0, 0, 10}, /* L */
        new int[] {0, 0, 5, 10, 10}, /* M */
        new int[] {0, 0, 10, 10}, /* N */
        new int[] {5, 0, 0, 10, 10, 5}, /* O */
        new int[] {0, 0, 0, 10, 10, 0}, /* P */
        
        new int[] {0, 0, 10, 10, 0}, /* P */
        // new int[] {5, 0, 0, 10, 10, 5, 10}, /* Q */
        new int[] {4, 0, 0, 4, 4}, /* new q */
        new int[] {0, 0, 0, 10, 10, 0, 10},  /* R */
        new int[] {0, 0, 10, 10, 0, 10}, /* R */
        new int[] {10, 0, 0, 10, 10, 0},  /* S */
        
        new int[] {0, 8, 8}, /* T */
        new int[] {0, 5, 10},  /* U */
        new int[] {0, 0, 10, 10}, /* U */
        new int[] {10, 5, 0}, /* V */
        new int[] {0, 3, 6, 10}, /* V */
        
        new int[] {0, 0, 5, 10, 10}, /* W */
        new int[] {0, 10, 10, 0}, /* X */
        new int[] {0, 0, 5, 5, 5, 5, 5, 10}, /* Y */
        new int[] {0, 10, 0, 10}, /* Z */
        //new int[] {3, 7, 0, 10},  /* Z */
        
        new int[] {0, 12}, /* \\ */
        new int[] {12, 0}, /* / */
        new int[] {0, 10}, /* space */
        new int[] {10, 0} /* backspace */
    };
        static int[][] refLettersY = {
        new int[] {10, 0, 10}, /* A */
        // new int[] {0, 10, 0, 0, 5, 5, 5, 10, 10}, /* B */
        // new int[] {10, 0, 0, 5, 5, 5, 10, 10}, /* B */ // remove
        // new int[] {0, 0, 5, 5, 5, 10, 10},  /* B */ // remove
        new int[] {0, 10, 7, 7, 10, 10}, /* new b */
        new int[] {0, 0, 10, 10}, /* C */
        
        new int[] {0, 0, 10, 10}, /* C */
        // new int[] {0, 10, 0, 0, 10, 10}, /* D */ // remove
        new int[] {10, 0, 0, 10, 10}, /* D */
        new int[] {0, 0, 5, 5, 5, 10, 10}, /* E */
        new int[] {3, 3, 0, 0, 7, 7}, /* new e */
        new int[] {0, 0, 10}, /* F */
        
        new int[] {0, 0, 10, 10, 5, 5}, /* G */
        new int[] {0, 10, 7, 7, 10}, /* H */
        new int[] {0, 10}, /* I */
        new int[] {0, 10, 10}, /* J */
        new int[] {0, 10, 0, 10}, /* K */
        
        new int[] {0, 10, 10}, /* L */
        new int[] {10, 0, 5, 0, 10}, /* M */
        new int[] {10, 0, 10, 0}, /* N */
        new int[] {0, 0, 10, 10, 0, 0},  /* O */
        new int[] {0, 10, 0, 0, 5, 5},  /* P */
        
        new int[] {10, 0, 0, 5, 5}, /* P */
        //new int[] {0, 0, 10, 10, 0, 0, 0},  /* Q */
        new int[] {0, 0, 4, 4, 7}, /* new q */
        new int[] {0, 10, 0, 0, 5, 5, 10},  /* R */
        new int[] {10, 0, 0, 5, 5, 10},  /* R */
        new int[] {0, 2, 4, 6, 8, 10},  /* S */
        
        new int[] {0, 0, 10},  /* T */
        new int[] {0, 10, 0},  /* U */
        new int[] {0, 10, 10, 0},  /* U */
        new int[] {0, 10, 0},  /* V */
        new int[] {0, 10, 0, 0},  /* V */
        
        new int[] {0, 10, 5, 10, 0},  /* W */
        new int[] {0, 10, 0, 10},  /* X */
        new int[] {0, 5, 5, 0, 5, 10, 5, 5},  /* Y */
        new int[] {0, 0, 10, 10},  /* Z */
        //new int[] {0, 0, 10, 10},  /* Z */
        
        new int[] {0, 10},  /* \\ */
        new int[] {0, 10},  /* / */
        new int[] {5, 5},  /* space */
        new int[] {5, 5} /* backspace */
    };

    }

}
