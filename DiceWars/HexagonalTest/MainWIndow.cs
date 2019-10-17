﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using HexagonalTest.PlayerAPI;
using HexagonalTest.Players;

namespace HexagonalTest
{
    public partial class MainWIndow : Form
    {
        private int numberOfPlayers = 0;
        private int selectSizeIndex = 0;
        private int sizeOfBoard = 0;

        private Hexagonal.Board board;

        public MainWIndow()
        {
            InitializeComponent();

            numberOfPlayers = 3;
            comboBoxPlayer.SelectedIndex = 1;

            sizeOfBoard = 8;
            comboBoxSize.SelectedIndex = 1;
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            if (numberOfPlayers == 0 || (comboBoxSize.SelectedIndex == -1))
            {
                //error
                System.Windows.Forms.MessageBox.Show("Select Player and size!");
            }
            //normal way
            else
            {
                DTOClass transferObject = new DTOClass();

                Hexagonal.BoardState state = new Hexagonal.Builder.BoardStateBuilder()
               .withGridPenWidth(2)
               .withActiveHexBorderColor(Color.Red)
               .withActiveGridPenWidth(2)
               .build();

                this.board = new Hexagonal.Builder.BoardBuilder()
                    .witHeight(sizeOfBoard)
                    .withWidht(sizeOfBoard)
                    .withSide(25)
                    .withPlayerLogics(new List<IPlayerLogic>
                    {
                        new UserPlayer(),
                        new BlockchainPrepper(),
                        new AlphaRandom(),
                        new DeepRandom(),
                        new QuantumRevenge()
                    })
                    .withBoardState(state)
                    .withDataTransfer(transferObject)
                    .build();

                HexagonalTest.Fight gameForm = new HexagonalTest.Fight(board, sizeOfBoard, transferObject);

                //prepare the DTO Object for the database
                //set the fieldsize and the enemycounter and start the playtimer
                transferObject.setEnemyCount(board.Players.Count);
                transferObject.setFieldSize(comboBoxSize.GetItemText(comboBoxSize.SelectedItem));
                transferObject.startTimerGo();

                //Start the FightForm
                gameForm.Show();

                this.Hide();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            numberOfPlayers = comboBoxPlayer.SelectedIndex;
            numberOfPlayers += 2;
        }

        private void comboBoxSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectSizeIndex = comboBoxSize.SelectedIndex;
            switch (selectSizeIndex)
            {
                case 0:
                    sizeOfBoard = 5;
                    break;

                case 1:
                    sizeOfBoard = 8;
                    break;

                case 2:
                    sizeOfBoard = 11;
                    break;

                case 3:
                    sizeOfBoard = 14;
                    break;

                case 4:
                    sizeOfBoard = 18;
                    break;

                default:
                    break;
            }
        }

        //Make comboBox readonly
        private void comboBoxPlayer_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.KeyChar = (char)Keys.None;
        }

        private void comboBoxSize_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void comboBoxSize_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.KeyChar = (char)Keys.None;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            /* Datenbank.SqliteDatabase database = new Datenbank.SqliteDatabase();
             database.connectDB();

             database.writeData( "Jan", "01:01:11", 1, "very small");
             database.writeData("Jan2", "01:01:12", 2, "very big");
             database.writeData( "Jan3", "01:01:13", 3, "normal");
             database.writeData( "Jan4", "01:01:14", 1, "small"); */

            HexagonalTest.Stats statsForm = new HexagonalTest.Stats();

            //Try to open the statistic form, but check if data exists in the Database or not!
            try
            {
                statsForm.Show();
            }
            catch (Exception error)
            {
                Console.WriteLine(error.ToString());
            }
        }
    }
}