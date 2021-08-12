using System;
using System.Collections.Generic;
using System.Text;

namespace Pacman.Simulator.Ghosts
{
	public class Blue : Ghost
	{
		public const int StartX = 95, StartY = 118;
		private const int firstWaitToEnter = 10, secondWaitToEnter = 20;

		public Blue(int x, int y, GameState gameState)
			: base(x, y, gameState) {
			this.name = "Blue";
			this.drawOffset = Height * 2;
			ResetPosition();
		}

		public override void PacmanDead() {
			waitToEnter = secondWaitToEnter;
			ResetPosition();
		}

		public override void ResetPosition() {
			x = StartX;
			y = StartY;
			waitToEnter = firstWaitToEnter;
			direction = Direction.Up;
			base.ResetPosition();
		}

		public override void Move() {
			if( Distance(gameState.Pacman) > randomMoveDist && GameState.Random.Next(0, randomMove) == 0 ) {
				MoveRandom();
			} else {
				// try to implement the last ... blue ... or, uhm, green. He looks pretty fucking blue to me			
				if( Distance(gameState.Red) < 50.0f ) { // estimate
					MoveAsRed(Direction.Right, Direction.Left, Direction.Down, Direction.Up);
				} else {
					// Always tries to minimize x till it is zero, the it moves in
					Direction preferredDirection = Direction;
					// minimize X
					if( Math.Abs(Node.X - gameState.Pacman.Node.X) != 0 ) {
						if( IsRight(gameState.Pacman) ) {
							preferredDirection = Direction.Right;
						} else {
							preferredDirection = Direction.Left;
						}
						if( !checkDirection(preferredDirection) || preferredDirection == InverseDirection(Direction) ) {
							if( IsAbove(gameState.Pacman) ) {
								preferredDirection = Direction.Up;
							} else {
								preferredDirection = Direction.Down;
							}
						}
					} else {
						if( IsBelow(gameState.Pacman) ) {
							preferredDirection = Direction.Down;
						} else {
							preferredDirection = Direction.Up;
						}
						if( !checkDirection(preferredDirection) || preferredDirection == InverseDirection(Direction) ) {
							if( IsLeft(gameState.Pacman) ) {
								preferredDirection = Direction.Left;
							} else {
								preferredDirection = Direction.Right;
							}
						}
					}
					if( preferredDirection == InverseDirection(Direction) ) {
						preferredDirection = Direction;
					}
					if( checkDirection(preferredDirection) ) {
						NextDirection = preferredDirection;
					} else {
						// just find something
						if( Direction != InverseDirection(Direction.Right) && checkDirection(Direction.Right) )
							NextDirection = Direction.Right;
						else if( Direction != InverseDirection(Direction.Left) && checkDirection(Direction.Left) )
							NextDirection = Direction.Left;
						else if( Direction != InverseDirection(Direction.Down) && checkDirection(Direction.Down) )
							NextDirection = Direction.Down;
						else if( Direction != InverseDirection(Direction.Up) && checkDirection(Direction.Up) )
							NextDirection = Direction.Up;
					}
				}
			}
			base.Move();
		}
	}
}
