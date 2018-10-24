using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	// Variables
	private bool distribuerYet;

	// Lists
	private List<int> jouerList = new List<int>();
	private List<int> jouerCarte = new List<int>();
	private List<int> dealerList = new List<int>();
	private List<int> dealerCarte = new List<int>();

	// Jouer
	public List<GameObject> m_jouerCartes = new List<GameObject>();
	public Text jouerScore;

	// Dealer
	public List<GameObject> m_dealerCartes = new List<GameObject>();
	public Text dealerScore;

	// Randomize
	private System.Random rnd = new System.Random();
	private int card;

	// Popup Alert
	public GameObject popupMsg;
	public Text popupAlertText;

	// Announcer
	public Text winnerAnnouncerText;


	// Use this for initialization
	void Start () {
		for (int i = 0; i < 5; i++) {
			jouerCarte.Add(0);
			jouerList.Add(0);
			dealerCarte.Add(0);
			dealerList.Add(0);
		}

		popupMsg.SetActive(false);
		distribuerYet = false; // Distribuer btn has not been pressed
	}
	
	// Update is called once per frame
	void Update () {
		jouerScore.text = "Jouer " + carteTotalValue(jouerCarte).ToString();
		dealerScore.text = "Dealer " + carteTotalValue(dealerCarte).ToString();
	}

	public void Distribuer() {
		distribuerYet = true;

		// Reset the value of the lists
		for (int i = 0; i < 5; i++) {
			jouerCarte[i] = 0;
			jouerList[i] = 0;
			dealerCarte[i] = 0;
			dealerList[i] = 0;
		}

		// Reset the sprites from the previous game
		m_jouerCartes[2].GetComponent<Image>().sprite = Resources.Load<Sprite>("Materials/carte_face");
		m_jouerCartes[3].GetComponent<Image>().sprite = Resources.Load<Sprite>("Materials/carte_face");
		m_jouerCartes[4].GetComponent<Image>().sprite = Resources.Load<Sprite>("Materials/carte_face");

		m_dealerCartes[2].GetComponent<Image>().sprite = Resources.Load<Sprite>("Materials/carte_face");
		m_dealerCartes[3].GetComponent<Image>().sprite = Resources.Load<Sprite>("Materials/carte_face");
		m_dealerCartes[4].GetComponent<Image>().sprite = Resources.Load<Sprite>("Materials/carte_face");

		// Also reset the scores
		jouerScore.text = "Jouer 0";
		dealerScore.text = "Dealer 0";

		// Random 2 cards for each player

		// Jouer first
		card = rnd.Next(1, 53);
		jouerList[0] = card;
		jouerCarte[0] = carteValue(card);
		m_jouerCartes[0].GetComponent<Image>().sprite = Resources.Load<Sprite>("Materials/" + jouerList[0].ToString());
		Debug.Log(card.ToString());
		Debug.Log(jouerCarte[0].ToString());

		// Jouer 2nd roll
		card = rnd.Next(1, 53);
		jouerList[1] = card;
		jouerCarte[1] = carteValue(card);
		m_jouerCartes[1].GetComponent<Image>().sprite = Resources.Load<Sprite>("Materials/" + jouerList[1].ToString());
		Debug.Log(card.ToString());
		Debug.Log(jouerCarte[1].ToString());

		// Dealer
		card = rnd.Next(1, 53);
		dealerList[0] = card;
		dealerCarte[0] = carteValue(card);
		m_dealerCartes[0].GetComponent<Image>().sprite = Resources.Load<Sprite>("Materials/" + dealerList[0].ToString());
		Debug.Log(card.ToString());
		Debug.Log(dealerCarte[0].ToString());

		// Dealer 2nd roll
		card = rnd.Next(1, 53);
		dealerList[1] = card;
		dealerCarte[1] = carteValue(card);
		m_dealerCartes[1].GetComponent<Image>().sprite = Resources.Load<Sprite>("Materials/" + dealerList[1].ToString());
		Debug.Log(card.ToString());
		Debug.Log(dealerCarte[1].ToString());
	}

	public void PrendUneCarte() {
		if (!distribuerYet) {
			popupMsg.SetActive(true);
			popupAlertText.text = "Vous devez avoir les cartes distribuées en premier!";
		}

		// JOUER prend une carte
    
    	// Check if jouer's score has exceeded limit (21)
    	// or jouer already has 5 cartes
    	// or jouer has got DoubleA / BlackJack
    	if ((carteTotalValue(jouerCarte) >= 21) || 
      		(jouerList[4] != 0) ||
      		(carteTotalValue(jouerCarte) == 1 || carteTotalValue(jouerCarte) == 2)) {
        		Debug.Log("Vous ne pouvez pas prendre plus de cartes.");
				popupMsg.SetActive(true);
				popupAlertText.text = "Vous ne pouvez pas prendre plus de cartes.";
        		return;
    	}
    	else {
      		// Random a carte 
			card = rnd.Next(1, 53);

      		// Ajouter à la main du joueur 
      		var n = 0; // number of current cartes
      		for (int i = 0; i < jouerList.Count; i++) {
        		if (jouerList[i] != 0 && jouerList[i] != 0) {
          			n++;
        		}
      		}
      	jouerList[n] = card;
      	jouerCarte[n] = carteValue(card);
		m_jouerCartes[n].GetComponent<Image>().sprite = Resources.Load<Sprite>("Materials/" + jouerList[n].ToString());
		}
	}

	public void OffAlert() {
		popupMsg.SetActive(false);
	}

	public void FinDuJeu() {
		if (!distribuerYet) {
			popupMsg.SetActive(true);
			popupAlertText.text = "Vous devez avoir les cartes distribuées en premier!";
		}

		// End the game when jouer press button `Terminer`
    	// This is also AI's turn to play, and the outcome.
    	// ================================================
    	// Note: Both players (AI and human) cannot see their opponent's cartes

		// First, jouer score must exceed acceptale score (>=17)
		// or Jouer has 5 cartes
		if (carteTotalValue(jouerCarte) != 1 && carteTotalValue(jouerCarte) != 2 
		&& carteTotalValue(jouerCarte) < 17 && jouerList[4] == 0) {
			Debug.Log("Votre score doit être au moins de 17!");
			popupMsg.SetActive(true);
			popupAlertText.text = "Votre score doit être au moins de 17!";
			return;
		}
		else {
			// Set the AI's score
			// Let the AI's process begin
			procedureAI();

			// Calculate winner
			switch (calcWinner(carteTotalValue(dealerCarte), carteTotalValue(jouerCarte), dealerList, jouerList)) {
				case -1:
					// AI Won
					winnerAnnouncerText.text = "AI est victorieux!";
					return;
				case 0:
					// Draw
					winnerAnnouncerText.text = "Jeu est nul!";
					return;
				case 1:
					winnerAnnouncerText.text = "Vous avez gagné!";
					return;
				default:
					return;
			}
		}
	}

	private void procedureAI() {
		// AI's process to win the game (or try to)

		// Check its own score if it has passed the threshold? (17)
		while (carteTotalValue(dealerCarte) < 17 && dealerList[4] == 0
		&& carteTotalValue(dealerCarte) != 1 && carteTotalValue(dealerCarte) != 2) {
			// While AI's score < 17 and it has < 5 cartes
			prendUneCarteAI();
			if (dealerList[4] != 0 || carteTotalValue(dealerCarte) >= 17) {
				break;
			}
		}

		// Check the situation for next steps
		if ((dealerList[4] != 0 && dealerList[4] != 0) ||
		carteTotalValue(dealerCarte) == 1 || carteTotalValue(dealerCarte) == 2 || 
    	carteTotalValue(dealerCarte) >= 21) {
			// If AI has 5 cartes or AI has double-A or BlackJack or >= 21, 
      		// no need to do anything else.---> it is done
			return;
		}
		else {
			// AI's score has reached above 17 and not reach 5 cartes yet
      		// The strategy: 
      		// **IF jouer has 4 or 5 cartes, there's a high possibility that 
      		// jouer's score is above 21, so AI would remain its score in that case.
      		// ------------------------------------------------------------------------
     	 	// **IF jouer has 3 cartes, there is a slightly higher chance that 
      		// jouer's score is still within maximum threshold. Therefore AI would
      		// only end the game if it has considerable score (18 --> 21)
      		// ------------------------------------------------------------------------
      		// **IF jouer has only 2 cartes, jouer's score definitely acceptable
      		// around 17 --> 20, also those highrollers' scores: double-A and BlackJack.
      		// Thus AI must try to draw some more cartes if its current score is only 
      		// around 17, 18. 
      		// ------------------------------------------------------------------------
      		// ****NOTE: IF jouer attempts to terminer at 17 score many times, 
      		// AI would record the `possiblity` for this and increase its rate, 
      		// hence perform different actions in the next case. But this is a rather
      		// complicated function, so the author would save it for the other occasion xD.
		
			// Jouer has 4 or 5 cartes
      		if (jouerList[4] != 0 || (jouerList[3] != 0 && jouerList[4] == 0)) {
        		// Stay at this score. End the game.
        		return;
      		}
			else if (jouerList[2] != 0 && jouerList[3] == 0) {
				if (carteTotalValue(dealerCarte) < 18) {
					// Pick 1 carte is enough --- no it's not, 
					// somehow with the tricky Ace 17score can get to 13 xD
					do {
						prendUneCarteAI();
						if (dealerList[4] != 0 || carteTotalValue(dealerCarte) > 17) {
							break;
						}
					}
					while (carteTotalValue(dealerCarte) < 17 && dealerList[4] == 0
            			&& carteTotalValue(dealerCarte) != 1 && carteTotalValue(dealerCarte) != 2);

					// function prendUneCarteAI() has already updated the data.
					return;
				}
				else {
					return;
				}
			}
			// Jouer has 2 cartes
			else {
				while (carteTotalValue(dealerCarte) <= 18) {
					prendUneCarteAI();
				}
					return;
			}
		}
	}

	private void prendUneCarteAI() {
		// sub-function for AI to draw a carte

		// Check the limit
		if ((carteTotalValue(dealerCarte) >= 21) || 
		(dealerList[4] != 0) || 
		(carteTotalValue(dealerCarte) == 1 || carteTotalValue(dealerCarte) == 2)) {
			return;
		}
		else {
			card = rnd.Next(1, 53);

			int n = 0; // number of current cartes
			for (int i = 0; i < dealerList.Count; i++) {
				if (dealerList[i] != 0 && dealerList[i] != 0) {
					n++;
				}
			}

			dealerList[n] = card;
			dealerCarte[n] = carteValue(card);
			m_dealerCartes[n].GetComponent<Image>().sprite = Resources.Load<Sprite>("Materials/" + dealerList[n].ToString());
		}
	}

	private int carteValue(int carteIndex) {
		switch (carteIndex) {
    		case 5:
   	 		case 6:
    		case 7: 
    		case 8:
      			return 2;
    		case 9:
    		case 10:
    		case 11: 
    		case 12:
      			return 3;
    		case 13:
    		case 14:
    		case 15:
    		case 16:
     			return 4;
    		case 17:
    		case 18:
    		case 19:
    		case 20:
      			return 5;
    		case 21:
    		case 22:
    		case 23:
    		case 24:
      			return 6;
    		case 25:
    		case 26:
    		case 27:
    		case 28:
      			return 7;
    		case 29:
    		case 30:
    		case 31:
    		case 32:
      			return 8;
    		case 33:
    		case 34:
    		case 35:
    		case 36:
      			return 9;
    		case 37:
    		case 38:
    		case 39:
    		case 40:
    		case 41:
    		case 42:
    		case 43:
    		case 44:
    		case 45:
    		case 46:
    		case 47:
    		case 48:
    		case 49:
    		case 50:
    		case 51:
    		case 52:
      			return 10;
    		case 1:
    		case 2:
    		case 3:
    		case 4:
    			return 11;
    		default:
      			return 0;
  		}
	}

	private int carteTotalValue(List<int> carteList) {
		// minimum score: 3 (A, 2)
  		// so we can have a range [1, 2] to store possible outcomes
  		// for instance: 
  		// 1- double A (A, A)
  		// 2- Blackjack (A, 10[value])

		int n = 0; // number of current cartes
		for (int i = 0; i < carteList.Count; i++) {
			if (carteList[i] != 0) {
				n++;
			}
		}

		switch (n) {
    		default:
    		case 0:
    		case 1:
      			return 0;
    		// ==============================================================================
    		case 2:
      			// Double A
      			if (carteList[0] == 11 && carteList[1] == 11) {
        			return 1;
      			} 
      			// BlackJack
      			else if ((carteList[0] == 11 && carteList[1] == 10) || (carteList[1] == 11 && carteList[0] == 10)) {
        			return 2;
      			}
      			else {
        			return (carteList[0] + carteList[1]);
     	 		}
    		// ==============================================================================
    		case 3:
				int s3;
      			// No A
      			if (carteList[0] != 11 && carteList[1] != 11 && carteList[2] != 11) {
        			return (carteList[0] + carteList[1] + carteList[2]);
      			}

      			// 1 A
      			else if ((carteList[0] == 11 && carteList[1] != 11 && carteList[2] != 11) ||
        			(carteList[1] == 11 && carteList[0] != 11 && carteList[2] != 11) ||
        			(carteList[2] == 11 && carteList[1] != 11 && carteList[0] != 11)) {
        			// Calculate sum value of non-A cartes
        			s3 = 0;
        			for (int i = 0; i < 3; i++) {
          				if (carteList[i] != 11) {
            				s3 += carteList[i];
          				}
        			}

        			if (s3 + 11 > 21) {
          				if (s3 + 10 > 21) {
            				return (s3 + 1);
          				}
         	 			else 
            				return (s3 + 10);
        			}
        			else 
          				return (s3 + 11);
      			}

      			// 2 A
      			else {
        			// Calculate sum value of non-A cartes
        			s3 = 0;
        			for (int i = 0; i < 2; i++) {
          				if (carteList[i] != 11) {
            				s3 += carteList[i];
          				}
        			}

        			// Only 3 possible cases: 1 A must be at 1 value, 
        			// the other one varies in array [1, 10, 11]
        			if (s3 + 12 > 21) {
          				if (s3 + 11 > 21) {
            				return (s3 + 2);
          				}
          				else 
            				return (s3 + 11);       
        			}
        			else 
          				return (s3 + 12);
      			}
    		// ==============================================================================
    		case 4:
      			// At 4 cartes, Ace counts as 1 (value)
      			int s4 = 0;
      			for (int i = 0; i < 4; i++) {
        			if (carteList[i] != 11) {
          				s4 += carteList[i];
        			}
        			else
          				s4 += 1;
      			}
      			return s4;
    		// ==============================================================================  
    		case 5:
      			// At 5 cartes, Ace counts as 1 (value)
      			// Calculate sum value of non-A cartes
      			int s5 = 0;
      			for (int i = 0; i < 5; i++) {
        			if (carteList[i] != 11) {
          				s5 += carteList[i];
        			}
        			else
          				s5 += 1;
      			}
      			return s5;
  		}
	}

	private int calcWinner(int dscore, int jscore, List<int> dList, List<int> jList) {
		// RULES (at least in my place tho): double-A(1) > 5-small-rolls > BlackJack(2).
  		// Return -1 if AI currently won, 0 if draw, and 1 if jouer won.
  		// First check if any player has exceed 21
		bool dHas5 = false; // jouer 5 cartes yet?
		bool jHas5 = false; // dealer 5 cartes yet?
		
		if (dList[4] != 0 && dList[4] != 0) {
			dHas5 = true;
		}

		if (jList[4] != 0 && jList[4] != 0) {
			jHas5 = true;
		}

		// Calculate the winner
  		// Jouer got 5 cartes
  		if (jHas5) {
    		if (jscore <= 21) {
      			if (dHas5) {
        			if (dscore < jscore) {
          				return -1; // AI won
        			}
        			else if (dscore == jscore) {
          				return 0; // draw
        			}
        			else
          				return 1; // Jouer won
      			}
      			else {
        			// AI has double-A
        			if (dscore == 1) {
          				return -1; // AI won
        			}
        			else
          				return 1; // Jouer won
      			}
    		}
    		else {
      			if (dscore > 21) {
        			return 0; // draw
      			}
      			else 
        			return -1; // AI won
    		}
  		}
  		else {
    		if (jscore <= 21) {
      			// Jouer has double-A
      			if (jscore == 1) {
        			if (dscore == 1) {
          				return 0; // draw
        			}
        			else {
          				return 1; // Jouer won
        			}
      			}
      			// Jouer has BlackJack
      			else if (jscore == 2) {
        			if (dHas5) {
          				if (dscore <= 21) {
            				return -1; // AI won
          				}
          				else
            				return 1; // Jouer won
        			}
        			else {
          				if (dscore == 1) {
            				return -1; // AI won
          				}
          				else if (dscore == 2) {
            				return 0; // draw
          				}
          				else 
            				return 1; // Jouer won
        			}
      			}
      			// Normal case, jscore <=21
     	 		else {
        			if (dHas5) {
          				if (dscore <= 21) {
            				return -1; // AI won
         	 			}
         	 			else
            				return 1; // Jouer won
        			}
        			else {
          				if (dscore == 2 || dscore == 1 
						  || (dscore > jscore && dscore <= 21)) {
            				return -1; // AI won
          				}
          				else if (dscore == jscore) {
            				return 0; // draw
          				}
          				else
            				return 1; // Jouer won
        			}
      			}
    		}
    		else {
      			if (dscore > 21) {
        			return 0; // draw
      			}
      			else
        			return -1; // AI won
    		}
  		}
	}

}
