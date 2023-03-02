package de.team05.server.model;

import java.util.Objects;

/**
 * Represents a square on the Quidditch pitch.
 */
public class Square {

    /**
     * This square's x coordinate.
     */
    public Integer x;

    /**
     * This square's y coordinate.
     */
    public Integer y;

    /**
     * Creates a sqiare with the given x and y.
     *
     * @param x coordinate.
     * @param y coordinate.
     */
    public Square(Integer x, Integer y) {
        this.x = x;
        this.y = y;
    }

    /**
     * Checks whether this square has either coordinate set to null.
     *
     * @return Null if either this square's x or y is/are null.
     */
    public boolean hasNullPos() {
        return x == null || y == null;
    }

    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (o == null || getClass() != o.getClass()) return false;
        Square square = (Square) o;
        return Objects.equals(x, square.x) &&
                Objects.equals(y, square.y);
    }

    @Override
    public String toString() {
        return "Square{" +
                "x=" + x +
                ", y=" + y +
                '}';
    }
}
