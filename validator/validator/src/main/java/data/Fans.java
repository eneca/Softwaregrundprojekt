package data;

public class Fans {
    private int goblins, trolls, elves, nifflers, wombats;

    public String fansValid() {

        String sOutputString = "";//erzeugt leeren Diagnostikstring

        //erster check: ist jede Rasse ein- oder mehrmals vertreten?
        if (goblins == 0 || trolls == 0 || elves == 0 || nifflers == 0 || wombats == 0) {
            sOutputString = sOutputString + " You need to have at least one Fan of each Race!" + "\n";
        }

        //zweiter check: max anzahl fans eingehalten?
        if (goblins + trolls + elves + nifflers + wombats != 7) {
            sOutputString = sOutputString + " You need to have exactly 7 Fans!" + "\n";
        }
        return sOutputString;
    }
}
