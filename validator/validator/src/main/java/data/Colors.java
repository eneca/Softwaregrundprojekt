package data;

public class Colors {
    String primary, secondary;

    public String colorsValid() {

        String sOutputString = "";//erzeugt leeren Diagnostikstring

        if (primary == null || secondary == null) {//sind die Farben beide da?
            sOutputString = sOutputString + " A Colour is missing!" + "\n";
        }


        if (primary.equals(secondary)) {//sind die Farben gleich?
            sOutputString = sOutputString + " Primary and Secondary colour cannot be the same!" + "\n";
        }

        return sOutputString;
    }
}
