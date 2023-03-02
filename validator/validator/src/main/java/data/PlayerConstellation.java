package data;

public class PlayerConstellation {
    private Player seeker, keeper, chaser1, chaser2, chaser3, beater1, beater2;

    public PlayerConstellation(Player seeker, Player keeper, Player chaser1, Player chaser2, Player chaser3, Player beater1, Player beater2) {
        this.seeker = seeker;
        this.keeper = keeper;
        this.chaser1 = chaser1;
        this.chaser2 = chaser2;
        this.chaser3 = chaser3;
        this.beater1 = beater1;
        this.beater2 = beater2;
    }

    public Player getSeeker() {
        return seeker;
    }

    public void setSeeker(Player seeker) {
        this.seeker = seeker;
    }

    public Player getKeeper() {
        return keeper;
    }

    public void setKeeper(Player keeper) {
        this.keeper = keeper;
    }

    public Player getChaser1() {
        return chaser1;
    }

    public void setChaser1(Player chaser1) {
        this.chaser1 = chaser1;
    }

    public Player getChaser2() {
        return chaser2;
    }

    public void setChaser2(Player chaser2) {
        this.chaser2 = chaser2;
    }

    public Player getChaser3() {
        return chaser3;
    }

    public void setChaser3(Player chaser3) {
        this.chaser3 = chaser3;
    }

    public Player getBeater1() {
        return beater1;
    }

    public void setBeater1(Player beater1) {
        this.beater1 = beater1;
    }

    public Player getBeater2() {
        return beater2;
    }

    public void setBeater2(Player beater2) {
        this.beater2 = beater2;
    }
}
