﻿using System;
using System.Collections.Generic;
using System.Drawing;
using HexagonalTest.PlayerAPI;
using HexagonalTest.Players;

namespace Hexagonal
{
    internal class Builder
    {
        public class BoardBuilder
        {
            //Required
            private int width;

            private int height;
            private int side;
            private HexagonalTest.DTOClass dataTransfer;

            //Optional
            private int? player = null;

            private List<IPlayerLogic> playerLogics = null;

            private int xOffset = 0;
            private int yOffset = 0;
            private BoardState boardState = new BoardStateBuilder().build();

            public BoardBuilder withWidht(int width)
            {
                if (width <= 0)
                {
                    throw new ArgumentException("Width must be greater than 0");
                }
                this.width = width;
                return this;
            }

            public BoardBuilder witHeight(int height)
            {
                if (height <= 0)
                {
                    throw new ArgumentException("Height must be greater than 0");
                }
                this.height = height;
                return this;
            }

            public BoardBuilder withSide(int side)
            {
                if (side <= 0)
                {
                    throw new ArgumentException("Side must be greater than 0");
                }
                this.side = side;
                return this;
            }

            public BoardBuilder withPlayer(int player)
            {
                if (player <= 1)
                {
                    throw new ArgumentException("There must be more than 1 Player");
                }
                this.player = player;
                return this;
            }

            public BoardBuilder withPlayerLogics(List<IPlayerLogic> playerLogics)
            {
                if (playerLogics.Count == 0)
                {
                    throw new ArgumentException("There must be more than 1 Player");
                }
                this.playerLogics = playerLogics;
                return this;
            }

            public BoardBuilder withXOffset(int xOffset)
            {
                this.xOffset = xOffset;
                return this;
            }

            public BoardBuilder withYOffset(int yOffset)
            {
                this.yOffset = yOffset;
                return this;
            }

            public BoardBuilder withBoardState(Hexagonal.BoardState boardState)
            {
                this.boardState = boardState;
                return this;
            }

            public BoardBuilder withDataTransfer(HexagonalTest.DTOClass data)
            {
                this.dataTransfer = data;
                return this;
            }

            public Board build()
            {
                if (this.width == 0 || this.height == 0 || this.side == 0)
                {
                    throw new ArgumentException("width, height and side must be set!");
                }
                List<Player> players = new List<Player>();
                if (playerLogics != null)
                {
                    for (int i = 0; i < playerLogics.Count; i++)
                    {
                        players.Add(new Player(i, PlayerColors.colors[i], playerLogics[i]));
                    }
                }
                else
                {
                    player = player ?? 2;
                    for (int i = 0; i < player; i++)
                    {
                        players.Add(new Player(i, PlayerColors.colors[i], new AlphaRandom()));
                    }
                }

                this.boardState.ActivePlayer = 0;
                return new Board(this.width, this.height, this.side, this.xOffset, this.yOffset, this.boardState, players, this.dataTransfer);
            }
        }

        public class BoardStateBuilder
        {
            private System.Drawing.Color backgroundColor = Color.White;
            private System.Drawing.Color gridColor = Color.Gray;
            private int gridPenWidth = 1;
            private System.Drawing.Color activeHexBorderColor = Color.Black;
            private int activeHexBorderWidth = 2;

            public BoardStateBuilder withBackgroundColour(Color backgroundColor)
            {
                this.backgroundColor = backgroundColor;
                return this;
            }

            public BoardStateBuilder withGridColor(Color gridColor)
            {
                this.gridColor = gridColor;
                return this;
            }

            public BoardStateBuilder withGridPenWidth(int gridPenWidth)
            {
                this.gridPenWidth = gridPenWidth;
                return this;
            }

            public BoardStateBuilder withActiveHexBorderColor(Color activeHexBorderColor)
            {
                this.activeHexBorderColor = activeHexBorderColor;
                return this;
            }

            public BoardStateBuilder withActiveGridPenWidth(int activeHexBorderWidth)
            {
                this.activeHexBorderWidth = activeHexBorderWidth;
                return this;
            }

            public BoardState build()
            {
                return new BoardState(this.backgroundColor, this.gridColor, this.gridPenWidth, this.activeHexBorderColor, this.activeHexBorderWidth);
            }
        }
    }
}