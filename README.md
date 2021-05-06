# Workshop_Combat

 Unity Project files for VGDC Unity Combat System Workshop 5/6/2021

- [See Zoom recording](no)

### Combat System Hierarchy

* **Entity** is the base class
  > Holds data for *health*

  * **Fighter** is an **Entity**
    > Has one **Hitbox** (or more)

    * **Player** is a **Fighter**
      > Holds data for reading *player input*

* **Hitbox** collides with **Entity** objects
  > Holds data for *attacks*

---

See our beginner Unity Workshops:

- [Workshop_Space Project](https://github.com/mattlawr/Workshop1_Space)
