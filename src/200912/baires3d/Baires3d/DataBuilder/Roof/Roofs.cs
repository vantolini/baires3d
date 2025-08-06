// Decompiled by DJ v3.9.9.91 Copyright 2005 Atanas Neshkov  Date: 26/07/2009 01:41:27 a.m.
// Home Page : http://members.fortunecity.com/neshkov/dj.html  - Check often for new version!
// Decompiler options: packimports(3) 
// Source File Name:   Roofs.java

import java.applet.Applet;
import java.awt.*;
import java.awt.event.*;
import java.io.PrintStream;

public class Roofs extends Applet
    implements ActionListener, ItemListener
{

    public Roofs()
    {
        running = false;
    }

    public static void main(String args[])
    {
        Frame frame = new Frame("Roofs - by David B\351langer");
        Roofs roofs = new Roofs();
        frame.setSize(600, 600);
        frame.add(roofs);
        frame.show();
        roofs.init();
    }

    public void init()
    {
        polySamples = new RealPolygon[11];
        RealPolygon realpolygon = new RealPolygon();
        RealPolygon realpolygon1 = new RealPolygon();
        RealPolygon realpolygon2 = new RealPolygon();
        RealPolygon realpolygon3 = new RealPolygon();
        RealPolygon realpolygon4 = new RealPolygon();
        RealPolygon realpolygon5 = new RealPolygon();
        RealPolygon realpolygon6 = new RealPolygon();
        RealPolygon realpolygon7 = new RealPolygon();
        RealPolygon realpolygon8 = new RealPolygon();
        RealPolygon realpolygon9 = new RealPolygon();
        RealPolygon realpolygon10 = new RealPolygon();
        realpolygon.addVertex(new Vertex(50D, 100D));
        realpolygon.addVertex(new Vertex(350D, 190D));
        realpolygon.addVertex(new Vertex(350D, 210D));
        realpolygon.addVertex(new Vertex(50D, 300D));
        realpolygon1.addVertex(new Vertex(200D, 100D));
        realpolygon1.addVertex(new Vertex(350D, 190D));
        realpolygon1.addVertex(new Vertex(350D, 210D));
        realpolygon1.addVertex(new Vertex(50D, 300D));
        realpolygon2.addVertex(new Vertex(50D, 100D));
        realpolygon2.addVertex(new Vertex(50D, 300D));
        realpolygon2.addVertex(new Vertex(350D, 300D));
        realpolygon2.addVertex(new Vertex(350D, 100D));
        realpolygon3.addVertex(new Vertex(50D, 100D));
        realpolygon3.addVertex(new Vertex(70D, 300D));
        realpolygon3.addVertex(new Vertex(330D, 300D));
        realpolygon3.addVertex(new Vertex(350D, 100D));
        realpolygon3.addVertex(new Vertex(200D, 60D));
        realpolygon4.addVertex(new Vertex(30D, 140D));
        realpolygon4.addVertex(new Vertex(40D, 120D));
        realpolygon4.addVertex(new Vertex(320D, 70D));
        realpolygon4.addVertex(new Vertex(370D, 115D));
        realpolygon4.addVertex(new Vertex(320D, 370D));
        realpolygon4.addVertex(new Vertex(200D, 390D));
        realpolygon5.addVertex(new Vertex(90D, 110D));
        realpolygon5.addVertex(new Vertex(185D, 160D));
        realpolygon5.addVertex(new Vertex(295D, 110D));
        realpolygon5.addVertex(new Vertex(265D, 275D));
        realpolygon5.addVertex(new Vertex(120D, 275D));
        realpolygon6.addVertex(new Vertex(100D, 120D));
        realpolygon6.addVertex(new Vertex(25D, 235D));
        realpolygon6.addVertex(new Vertex(335D, 235D));
        realpolygon6.addVertex(new Vertex(290D, 120D));
        realpolygon6.addVertex(new Vertex(200D, 145D));
        realpolygon7.addVertex(new Vertex(170D, 25D));
        realpolygon7.addVertex(new Vertex(90D, 110D));
        realpolygon7.addVertex(new Vertex(120D, 140D));
        realpolygon7.addVertex(new Vertex(60D, 170D));
        realpolygon7.addVertex(new Vertex(155D, 390D));
        realpolygon7.addVertex(new Vertex(280D, 335D));
        realpolygon7.addVertex(new Vertex(165D, 115D));
        realpolygon7.addVertex(new Vertex(210D, 60D));
        realpolygon8.addVertex(new Vertex(60D, 110D));
        realpolygon8.addVertex(new Vertex(100D, 110D));
        realpolygon8.addVertex(new Vertex(115D, 170D));
        realpolygon8.addVertex(new Vertex(130D, 110D));
        realpolygon8.addVertex(new Vertex(300D, 110D));
        realpolygon8.addVertex(new Vertex(300D, 215D));
        realpolygon8.addVertex(new Vertex(230D, 255D));
        realpolygon8.addVertex(new Vertex(300D, 295D));
        realpolygon8.addVertex(new Vertex(60D, 370D));
        realpolygon9.addVertex(new Vertex(220D, 47D));
        realpolygon9.addVertex(new Vertex(160D, 318D));
        realpolygon9.addVertex(new Vertex(10D, 388D));
        realpolygon9.addVertex(new Vertex(277D, 280D));
        realpolygon9.addVertex(new Vertex(402D, 416D));
        realpolygon9.addVertex(new Vertex(204D, 174D));
        realpolygon10.addVertex(new Vertex(139D, 104D));
        realpolygon10.addVertex(new Vertex(42D, 233D));
        realpolygon10.addVertex(new Vertex(83D, 343D));
        realpolygon10.addVertex(new Vertex(325D, 343D));
        realpolygon10.addVertex(new Vertex(230D, 283D));
        realpolygon10.addVertex(new Vertex(316D, 310D));
        realpolygon10.addVertex(new Vertex(369D, 137D));
        realpolygon10.addVertex(new Vertex(264D, 178D));
        realpolygon10.addVertex(new Vertex(316D, 104D));
        polySamples[0] = realpolygon;
        polySamples[1] = realpolygon1;
        polySamples[2] = realpolygon2;
        polySamples[3] = realpolygon3;
        polySamples[4] = realpolygon4;
        polySamples[5] = realpolygon5;
        polySamples[6] = realpolygon6;
        polySamples[7] = realpolygon7;
        polySamples[8] = realpolygon8;
        polySamples[9] = realpolygon10;
        polySamples[10] = realpolygon9;
        setBackground(new Color(bgColor));
        showStatus("The Roof Applet -- by David B\351langer");
        Button button = new Button("Run");
        Button button1 = new Button("Step");
        Button button2 = new Button("Reset");
        msgBox = new TextArea(6, 40);
        msgBox.setEditable(false);
        button2.setActionCommand("reset");
        button2.addActionListener(this);
        button.setActionCommand("run");
        button.addActionListener(this);
        button1.setActionCommand("step");
        button1.addActionListener(this);
        wallChooser = new Choice();
        wallChooser.addItemListener(this);
        wallChooser.add("Select a house");
        wallChooser.add("House 1");
        wallChooser.add("House 2");
        wallChooser.add("Rectangle");
        wallChooser.add("Pentagon");
        wallChooser.add("Hexagon");
        wallChooser.add("Reflex 1");
        wallChooser.add("Reflex 2");
        wallChooser.add("House with garage");
        wallChooser.add("From tutorial 1");
        wallChooser.add("From tutorial 2");
        wallChooser.add("Special polygon");
        sCanvas = new SkeletonCanvas(this, 400, 400);
        statusBar1 = new Label("Draw a simple polygon or select one from the list.");
        statusBar2 = new Label("");
        Panel panel = new Panel();
        panel.setLayout(new GridLayout(0, 1));
        panel.add(new Label("The Roof Applet"));
        panel.add(statusBar1);
        panel.add(statusBar2);
        Panel panel1 = new Panel();
        panel1.setLayout(new GridLayout(0, 1));
        panel1.add(button);
        panel1.add(button1);
        panel1.add(button2);
        panel1.add(wallChooser);
        setLayout(new BorderLayout());
        add("North", panel);
        add("Center", sCanvas);
        add("East", panel1);
        add("South", msgBox);
    }

    public void printStatus1(String s)
    {
        statusBar1.setText(s);
    }

    public void printStatus2(String s)
    {
        statusBar2.setText(s);
    }

    public void printMessage(String s)
    {
        msgBox.append("\n" + s);
    }

    public void actionPerformed(ActionEvent actionevent)
    {
        String s = actionevent.getActionCommand();
        if(s.equals("reset"))
        {
            if(sim != null)
                sim.reset();
            sCanvas.reset();
            printStatus1("Draw a simple polygon or select one from the list.");
            printStatus2("");
            printMessage("-------------------");
        } else
        if(s.equals("run"))
        {
            if(sCanvas.isDone())
            {
                if(!running)
                {
                    System.out.println("poly is " + sCanvas.getInputPoly());
                    PolygonSet polygonset = sCanvas.createShrinkPoly();
                    sim = new Simulator(this, sCanvas, polygonset);
                }
                sim.run();
            }
        } else
        if(s.equals("step"))
        {
            if(sCanvas.isDone())
            {
                if(!running)
                {
                    System.out.println("poly is " + sCanvas.getInputPoly());
                    PolygonSet polygonset1 = sCanvas.createShrinkPoly();
                    sim = new Simulator(this, sCanvas, polygonset1);
                }
                running = true;
                sim.run(true);
            }
        } else
        if(s.equals("about"))
        {
            System.out.println("About");
            sCanvas.about = !sCanvas.about;
            sCanvas.repaint();
        }
    }

    public void itemStateChanged(ItemEvent itemevent)
    {
        int i = wallChooser.getSelectedIndex();
        if(i != 0)
        {
            printStatus1("Now click on RUN or on STEP.");
            printStatus2("");
            sCanvas.reset();
            sCanvas.realPoly = polySamples[i - 1];
            sCanvas.done = true;
            sCanvas.repaint();
        } else
        {
            printStatus1("Draw a simple polygon or select one from the list.");
            printStatus2("");
        }
    }

    private RealPolygon polySamples[];
    public boolean running;
    public static int bgColor = 0xffffe0;
    Simulator sim;
    SkeletonCanvas sCanvas;
    Label statusBar1;
    Label statusBar2;
    TextArea msgBox;
    Choice wallChooser;
    public static final String drawMsgStr = "Draw a simple polygon or select one from the list.";
    public static final String closeMsgStr = "Click on the red vertex to close the polygon.";
    public static final String clickRunMsgStr = "Now click on RUN or on STEP.";
    public static final String closedMsgStr = "Polygon is closed.  Now click on RUN or on STEP.";

}