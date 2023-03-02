package data;

import java.util.Arrays;
import java.util.LinkedList;
import java.util.List;

public class Teamconfig {


    private Colors colors;
    private String image;
    private Fans fans;
    private PlayerConstellation players;
    private String name;
    private String motto;

    public Teamconfig(Colors colors, String image, Fans fans, PlayerConstellation players, String name, String motto) {
        this.colors = colors;
        this.image = image;
        this.fans = fans;
        this.players = players;
        this.name = name;
        this.motto = motto;
    }

    public String teamconfigChecker() {
        String errorMessageString = "";

        //checking in Teamconfig.java
        if (image == null) {
            errorMessageString = errorMessageString + " The provided Image is invalid or missing!" + "\n";
        }
        if (name == null) {
            errorMessageString = errorMessageString + " The provided Name is invalid or missing!" + "\n";
        }
        if (motto == null) {
            errorMessageString = errorMessageString + " The provided Motto is invalid or missing!" + "\n";
        }

        if (!besencheck()) {
            errorMessageString = errorMessageString + " You do not have each broom at least once or your broom-names are misspelled!" + "\n";
        }

        if (!numberOfSexes()) {
            errorMessageString = errorMessageString + " You need to have at least 3 players of each sex!" + "\n";
        }

        if (players.getSeeker() == null || players.getKeeper() == null || players.getChaser1() == null || players.getChaser2() == null || players.getChaser3() == null || players.getBeater1() == null || players.getBeater2() == null) {
            errorMessageString = errorMessageString + " At least one position in your team is not occupied!";
        }

        //checking in corresponding class
        errorMessageString = errorMessageString + colors.colorsValid();

        errorMessageString = errorMessageString + fans.fansValid();

        errorMessageString = errorMessageString + isEveryPlayerOkay();


        return errorMessageString;
    }

    private String isEveryPlayerOkay() {
        if (players.getSeeker().isPlayerNull() || players.getKeeper().isPlayerNull() || players.getChaser1().isPlayerNull() || players.getChaser2().isPlayerNull() || players.getChaser3().isPlayerNull() || players.getBeater1().isPlayerNull() || players.getBeater2().isPlayerNull()) {
            return " At least one of your players is invalid. Check name, broom and sex!" + "\n";
        }

        return "";
    }

    private boolean numberOfSexes() {
        String[] sexes = new String[7];
        int iCountF = 0, iCountM = 0;
        try {
            sexes[0] = players.getSeeker().sex;
            sexes[1] = players.getKeeper().sex;
            sexes[2] = players.getChaser1().sex;
            sexes[3] = players.getChaser2().sex;
            sexes[4] = players.getChaser3().sex;
            sexes[5] = players.getBeater1().sex;
            sexes[6] = players.getBeater2().sex;
        } catch (NullPointerException n) {
        }


        for (int i = 0; i < 7; i++) {
            if (sexes[i].equals("f")) {
                iCountF++;
            } else if (sexes[i].equals("m")) {
                iCountM++;

            }
        }

        return iCountF <= 4 && iCountM <= 4 && iCountF + iCountM == 7 && iCountF >= 3 && iCountM >= 3;

    }

    private boolean besencheck() {
        List<String> besennamen = new LinkedList<>();
        String[] besenmarken = {"tinderblast", "cleansweep11", "comet260", "nimbus2001", "firebolt"};
        try {
            besennamen.add(players.getSeeker().broom);
            besennamen.add(players.getKeeper().broom);
            besennamen.add(players.getChaser1().broom);
            besennamen.add(players.getChaser2().broom);
            besennamen.add(players.getChaser3().broom);
            besennamen.add(players.getBeater1().broom);
            besennamen.add(players.getBeater2().broom);

            return besennamen.containsAll(Arrays.asList(besenmarken));
        } catch (NullPointerException n) {
            return false;
        }
    }

    public boolean sexCountValid() {
        int iCountFemales = 0;
        if (players.getSeeker().sex.equals("f")) {
            iCountFemales += 1;
        }
        if (players.getKeeper().sex.equals("f")) {
            iCountFemales += 1;
        }
        if (players.getChaser1().sex.equals("f")) {
            iCountFemales += 1;
        }
        if (players.getChaser2().sex.equals("f")) {
            iCountFemales += 1;
        }
        if (players.getChaser3().sex.equals("f")) {
            iCountFemales += 1;
        }
        if (players.getBeater1().sex.equals("f")) {
            iCountFemales += 1;
        }
        if (players.getBeater2().sex.equals("f")) {
            iCountFemales += 1;
        }

        return iCountFemales <= 4 && iCountFemales >= 3;

    }
}

