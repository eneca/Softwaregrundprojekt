package data;

public class Player {
    String name, broom, sex;

    public Player(String name, String broom, String sex) {
        this.name = name;
        this.broom = broom;
        this.sex = sex;
    }

    public boolean isPlayerNull() {

        if (name == null) return true;
        if (broom == null) return true;
        return sex == null;

    }
}
