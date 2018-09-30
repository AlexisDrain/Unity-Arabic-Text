using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ArabicTextUtility : EditorWindow {

	public string textFieldA = "";
	public string textFieldB = "";
	
	[MenuItem("Tools/Arabic Text Utility")]
	static void Init () {
		EditorWindow.GetWindow(typeof(ArabicTextUtility)).Show();
		
	}

	private void OnGUI() {

		GUILayout.Label("Input");
		textFieldA = EditorGUILayout.TextArea(textFieldA,GUILayout.Height(60));
		GUILayout.Label("Result:");
		textFieldB = EditorGUILayout.TextArea(textFieldB,GUILayout.Height(60));
		
		if (GUILayout.Button ("Input to Result")) {
			ConvertText();
		}
	}

	private void ConvertText() {

		string tempText = "";
		int hashNumShifted, followingHashShifted, previousHashShifted;
		char newChar;
		int position = 0;
		
		
		
		for (int i = 0; i < textFieldA.Length; i+=1) {
			
			
			/*
			 
			  1) setup hash values:
			  
			*/
			
			// current
			hashNumShifted = textFieldA[i].GetHashCode() - 1569;
			// following char
			if (i + 1 < textFieldA.Length) {
				followingHashShifted = textFieldA[i+1].GetHashCode() - 1569;
				if (HashIsArabic(followingHashShifted) == false) {
					followingHashShifted = -1;
				}
			}
			else {
				followingHashShifted = -1;
			}
			// previous char
			if (i - 1 >= 0) {
				previousHashShifted = textFieldA[i-1].GetHashCode() - 1569;
				if (HashIsArabic(previousHashShifted) == false || HashIsDisconnected(previousHashShifted) == true) {
					previousHashShifted = -1;
				}
			}
			else {
				previousHashShifted = -1;
			}
			
			/*
			 
			 2) if letter isn't arabic. skip
			 
			*/
			if (HashIsArabic(hashNumShifted) == false) {
				newChar = textFieldA[i];
				tempText = tempText.Insert(0, newChar.ToString());
				continue;
			}
			
			/*
			 
			   3) setup position
			   
					0: Isolated
					1: Final
					2: Initial
					3: Middle
					
					hash equals -1 is null or not arabic
			*/
			
			position = 0;
			
			if (previousHashShifted != -1 && followingHashShifted == -1) {
				position = 1;
			}
			if (previousHashShifted == -1 && followingHashShifted != -1) {
				position = 2;
			}
			if (previousHashShifted != -1 && followingHashShifted != -1) {
				position = 3;
			}
			
			/*
			 
			   4) get character from the glyphmap
			   
			*/
			
			// special case: lam alef:
			if (hashNumShifted == 35) {
				
				// alef madd
				if (followingHashShifted == 1) {
				
					newChar = GlyphMap[42, position];
					tempText = tempText.Insert(0, newChar.ToString());
					i += 1; // skip a step
					continue;
				}
				// hamza above
				if (followingHashShifted == 2) {

					newChar = GlyphMap[43, position];
					tempText = tempText.Insert(0, newChar.ToString());
					i += 1; // skip a step
					continue;
				}
				// hamza below
				if (followingHashShifted == 4) {
				
					newChar = GlyphMap[44, position];
					tempText = tempText.Insert(0, newChar.ToString());
					i += 1; // skip a step
					continue;
				}
				
				// alef
				if (followingHashShifted == 6) {
				
					newChar = GlyphMap[45, position];
					tempText = tempText.Insert(0, newChar.ToString());
					i += 1; // skip a step
					continue;
				}
			}
			
			// regular case
			
			if (HashIsArabic(hashNumShifted)) {
				newChar = GlyphMap[hashNumShifted, position];
				tempText = tempText.Insert(0, newChar.ToString());
			}
		}
		textFieldB = tempText;
	}

	/*
	 *
	 * checks if hash code is in Arabic range. assumes that they have been shifted by 1569 positions
	 * 
	 */
	private bool HashIsArabic(int hashcode) {
		
		if (hashcode >= 0 && hashcode <= 41) {
			return true;
		}

		return false;
	}
	private bool HashIsDisconnected(int hashcode) {

		for (int i = 0; i < disconectedLetters.Length; i += 1) {
			
			if (disconectedLetters[i] == hashcode) {
				return true;
			}
		}

		return false;
	}
	// based on Glyphmap by Dina Lasheen - MSFT
	// https://blogs.msdn.microsoft.com/global_developer/2011/05/09/shaping-arabic-characters/

	// connects with the previous letters, but doesn't connect the the next letter
	private readonly int[] disconectedLetters = new int[] {
		0, 1, 2, 3, 4, 6, 14, 15, 16, 17, 39, 40, 42, 43, 44, 45
	};
	private readonly char[,] GlyphMap = new char[46, 4] 

        {

         // Isolated, Final, Initial, Middle Forms
		// 0
            {'\xFE80','\xFE80','\xFE80','\xFE80'}, /* HAMZA 0x0621*/
		// 1
            {'\xFE81','\xFE82','\xFE81','\xFE82'}, /* ALEF WITH MADDA ABOVE 0x622 */
		// 2
             {'\xFE83','\xFE84','\xFE83','\xFE84'}, /* ALEF WITH HAMZA ABOVE 0x0623*/
		// 3
             {'\xFE85','\xFE86','\xFE85','\xFE86'}, /* WAW WITH HAMZA ABOVE 0x0624 */
		// 4
             {'\xFE87','\xFE88','\xFE87','\xFE88'}, /* ALEF WITH HAMZA BELOW 0x0625*/
		// 5
             {'\xFE89','\xFE8A','\xFE8B','\xFE8C'}, /* YEH WITH HAMZA ABOVE 0x0626*/
		// 6
             {'\xFE8D','\xFE8E','\xFE8D','\xFE8E'}, /* ALEF 0x0627*/
		// 7
             {'\xFE8F','\xFE90','\xFE91','\xFE92'}, /* BEH 0x0628*/
		// 8
             {'\xFE93','\xFE94','\xFE93','\xFE94'}, /* TEH MARBUTA 0x0629*/
		// 9
             {'\xFE95','\xFE96','\xFE97','\xFE98'}, /* TEH 0x062A*/
		// 10
             {'\xFE99','\xFE9A','\xFE9B','\xFE9C'}, /* THEH 0x062B*/
		// 11
             {'\xFE9D','\xFE9E','\xFE9F','\xFEA0'}, /* JEEM 0x062C*/
		// 12
             {'\xFEA1','\xFEA2','\xFEA3','\xFEA4'}, /* HAH 0x062D*/
		// 13
             {'\xFEA5','\xFEA6','\xFEA7','\xFEA8'}, /* KHAH 0x062E*/
		// 14
             {'\xFEA9','\xFEAA','\xFEA9','\xFEAA'}, /* DAL 0x062F*/
		// 15
             {'\xFEAB','\xFEAC','\xFEAB','\xFEAC'}, /* THAL0x0630 */
		// 16
             {'\xFEAD','\xFEAE','\xFEAD','\xFEAE'}, /* RAA 0x0631*/
		// 17
             {'\xFEAF','\xFEB0','\xFEAF','\xFEB0'}, /* ZAIN 0x0632*/
		// 18
             {'\xFEB1','\xFEB2','\xFEB3','\xFEB4'}, /* SEEN 0x0633*/
		// 19
             {'\xFEB5','\xFEB6','\xFEB7','\xFEB8'}, /* SHEEN 0x0634*/
		// 20
             {'\xFEB9','\xFEBA','\xFEBB','\xFEBC'}, /* SAD 0x0635*/
		// 21
             {'\xFEBD','\xFEBE','\xFEBF','\xFEC0'}, /* DAD 0x0636*/
		// 22
             {'\xFEC1','\xFEC2','\xFEC3','\xFEC4'}, /* TAH 0x0637*/
		// 23
             {'\xFEC5','\xFEC6','\xFEC7','\xFEC8'}, /* ZAH 0x0638*/
		// 24
             {'\xFEC9','\xFECA','\xFECB','\xFECC'}, /* AIN 0x0639*/
		// 25
             {'\xFECD','\xFECE','\xFECF','\xFED0'}, /* GHAIN 0x063A*/
		// 26
	         {'0','0','0','0'}, /* space*/
	    // 27
	         {'0','0','0','0'}, /* space*/
	    // 28
	         {'0','0','0','0'}, /* space*/
	    // 29
	        {'0','0','0','0'}, /* space*/
	    // 30
	        {'0','0','0','0'}, /* space*/
	    // 31
            {'\x0640','\x0640','\x0640','\x0640'}, /* TATWEEL 0x0640*/
		// 32
            {'\xFED1','\xFED2','\xFED3','\xFED4'}, /* FAA 0x0641*/
		// 33
            {'\xFED5','\xFED6','\xFED7','\xFED8'}, /* QAF 0x0642*/
		// 34
            {'\xFED9','\xFEDA','\xFEDB','\xFEDC'}, /* KAF 0x0643*/
		// 35
            {'\xFEDD','\xFEDE','\xFEDF','\xFEE0'}, /* LAM 0x0644*/
		// 36
            {'\xFEE1','\xFEE2','\xFEE3','\xFEE4'}, /* MEEM 0x0645*/
		// 37
            {'\xFEE5','\xFEE6','\xFEE7','\xFEE8'}, /* NOON 0x0646*/
		// 38
            {'\xFEE9','\xFEEA','\xFEEB','\xFEEC'}, /* HEH 0x0647*/
		// 39
            {'\xFEED','\xFEEE','\xFEED','\xFEEE'}, /* WAW 0x0648*/
		// 40
            {'\xFEEF','\xFEF0','\xFBE8','\xFBE9'}, /* ALEF MAKSURA 0x0649*/
		// 41
            {'\xFEF1','\xFEF2','\xFEF3','\xFEF4'}, /* YEH 0x064A*/
		// 42
            {'\xFEF5','\xFEF6','\xFEF5','\xFEF6'}, /* LAM ALEF MADD*/
		// 43
            {'\xFEF7','\xFEF8','\xFEF7','\xFEF8'}, /* LAM ALEF HAMZA ABOVE*/
		// 44
            {'\xFEF9','\xFEFa','\xFEF9','\xFEFa'}, /* LAM ALEF HAMZA BELOW*/
		// 45
            {'\xFEFb','\xFEFc','\xFEFb','\xFEFc'}, /* LAM ALEF */

         };
	
}