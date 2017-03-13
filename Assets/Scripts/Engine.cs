using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine  {

    static float[][] intensite; //ordonnée paire = courants verticaux
    static float[][] pression;
    const float alpha = 0.1f;


    public static void initialize_p_i(int N,int M)
    {
        intensite = new float[N - 1][];
        pression = new float[N - 1][];
 

        for (int k = 0; k < N - 1; k++)
        {
            intensite[k] = new float[2 * (M - 2) + 1];
            pression[k] = new float[2 * (M - 2) + 1];

            for (int l = 0; l < 2 * (M - 2) + 1; l++)
                intensite[k][l] = pression[k][l] = 0;
        }
    }

    public static void rotate_currant(int dir, float[] pc, float[] ic)
    { // permutation circulaire des courrants  
        float i, p;

        switch (dir)
        {
            case 0:
                break;
            case 1:
                p = pc[0]; i = ic[0];
                for (int k = 0; k < 3; k++)
                {
                    pc[k] = pc[k + 1]; ic[k] = ic[k + 1];
                }
                pc[3] = p; ic[3] = i;
                break;
            case 2:
                p = pc[0]; i = ic[0];
                pc[0] = pc[2]; ic[0] = ic[2];
                pc[2] = p; ic[2] = i;
                p = pc[1]; i = ic[1];
                pc[1] = pc[3]; ic[1] = ic[3];
                pc[3] = p; ic[3] = i;
                break;
            case 3:
                p = pc[3]; i = ic[3];
                for (int k = 3; k > 0; k--)
                {
                    pc[k] = pc[k - 1]; ic[k] = ic[k - 1];
                }
                pc[0] = p; ic[0] = i;
                break;
        }

    }


    public static void currant_update(int x, int y, float[][] p, float[][] i, float[] pc, float[] ic, float alpha)
    {
        p[x + 1][2 * y + 1] = (1 - alpha) * p[x + 1][2 * y + 1] + alpha * pc[0];  //à droite
        i[x + 1][2 * y + 1] = (1 - alpha) * i[x + 1][2 * y + 1] + alpha * -ic[0];

        p[x][2 * y] = (1 - alpha) * p[x][2 * y] + alpha * pc[1];  //en haut
        i[x][2 * y] = (1 - alpha) * i[x][2 * y] + alpha * ic[1];

        p[x][2 * y + 1] = (1 - alpha) * p[x][2 * y + 1] + alpha * pc[2];  //à gauche
        i[x][2 * y + 1] = (1 - alpha) * i[x][2 * y + 1] + alpha * ic[2];

        p[x][2 * y + 2] = (1 - alpha) * p[x][2 * y + 2] + alpha * pc[3];  //en bas
        i[x][2 * y + 2] = (1 - alpha) * i[x][2 * y + 2] + alpha * -ic[3];

    }


    public static void currant_in(int x, int y, float[][] p, float[][] i, float[] pc, float[] ic)
    { //selectionne les 4 courants en x et y entrant et 4 pressions avec la convention "in" pour les courants

        pc[0] = p[x + 1][2 * y + 1];  //à droite
        ic[0] = -i[x + 1][2 * y + 1];

        pc[1] = p[x][2 * y];  //en haut
        ic[1] = i[x][2 * y];

        pc[2] = p[x][2 * y + 1];  //à gauche
        ic[2] = i[x][2 * y + 1];

        pc[3] = p[x][2 * y + 2];  //en bas
        ic[3] = -i[x][2 * y + 2];
    }

    static float time;
    static float[] pp = new float[4];
    static float[] ii = new float[4];


    public static float oneStep(BaseComponent[][] composants)
    {
        time++;

        int N = composants.Length;
        int M = composants[0].Length;


        for (int k = 1; k < N - 1; k++) //Border UP condition
        {

            pp[0] = pression[k - 1][0];
            ii[0] = (-intensite[k - 1][0]);
            composants[k][0].calcule_i_p(pp, ii);
            pression[k - 1][0] = (1 - alpha) * pression[k - 1][0] + alpha * pp[0];
            intensite[k - 1][0] = (1 - alpha) * intensite[k - 1][0] + alpha * (-ii[0]);
        }

        for (int k = 1; k < N - 1; k++) //Border DOWN condition
        {
            pp[0] = pression[k - 1][2 * M - 4];
            ii[0] = intensite[k - 1][2 * M - 4];
            composants[k][M - 1].calcule_i_p(pp, ii);

            pression[k - 1][2 * M - 4] = (1 - alpha) * pression[k - 1][2 * M - 4] + alpha * pp[0];
            intensite[k - 1][2 * M - 4] = (1 - alpha) * intensite[k - 1][2 * M - 4] + alpha * ii[0];
        }

        for (int k = 1; k < M - 1; k++) //Border RIGHT condition
        {

            pp[0] = pression[N - 2][2 * (k - 1) + 1];
            ii[0] = (intensite[N - 2][2 * (k - 1) + 1]);
            composants[N - 1][k].calcule_i_p(pp, ii);
            pression[N - 2][2 * (k - 1) + 1] = (1 - alpha) * pression[N - 2][2 * (k - 1) + 1] + alpha * pp[0];
            intensite[N - 2][2 * (k - 1) + 1] = (1 - alpha) * intensite[N - 2][2 * (k - 1) + 1] + alpha * (ii[0]);
        }

        for (int k = 1; k < M - 1; k++) //Border LEFT condition
        {
            pp[0] = pression[0][2 * (k - 1) + 1];
            ii[0] = (-intensite[0][2 * (k - 1) + 1]);
            composants[0][k].calcule_i_p(pp, ii);
            pression[0][2 * (k - 1) + 1] = (1 - alpha) * pression[0][2 * (k - 1) + 1] + alpha * pp[0];
            intensite[0][2 * (k - 1) + 1] = (1 - alpha) * intensite[0][2 * (k - 1) + 1] + alpha * (-ii[0]);
        }

        float success = 1;

        for (int k = 1; k < N - 1; k++)
        {
            for (int l = 1; l < M - 1; l++)
            {
                Engine.currant_in(k - 1, l - 1, pression, intensite, pp, ii);
                Engine.rotate_currant(composants[k][l].dir, pp, ii);
                composants[k][l].set_i_p(pp, ii);
                composants[k][l].calcule_i_p(pp, ii);
                Engine.rotate_currant((4 - composants[k][l].dir) % 4, pp, ii);
                Engine.currant_update(k - 1, l - 1, pression, intensite, pp, ii, alpha);

                success = success * composants[k][l].success;
            }
        }


        return success;
        

    }

    public static void update_composant_p_i(BaseComponent[][] composants)
    {
        int N = composants.Length;
        int M = composants[0].Length;

        for (int k = 0; k < N ; k++)
        {
            for (int l = 0; l < M; l++)
            {
                if (composants[k][l] == null) continue;  // Corner

                if (l == 0)
                {
                    if (k > 0 && k < N - 1) pp[0] = pression[k - 1][0];
                }
                else if (l == M - 1)
                {
                    if (k > 0 && k < N - 1) pp[0] = pression[k - 1][2 * M - 4];
                }
                else if (k == 0)
                    pp[0] = pression[0][2 * (l - 1) + 1];
                else if (k == N - 1)
                    pp[0] = pression[N - 2][2 * (l - 1) + 1];
                else
                {
                    Engine.currant_in(k - 1, l - 1, pression, intensite, pp, ii);
                    Engine.rotate_currant(composants[k][l].dir, pp, ii);
                }
                //Debug.Log("composant " + k + "   " + l);
                composants[k][l].set_i_p(pp, ii);
            }
        }
    }


}
