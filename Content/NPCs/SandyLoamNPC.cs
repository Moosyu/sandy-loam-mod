using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using veryawesomemod.Content.BossBars;
using veryawesomemod.Content.Projectiles;
using Terraria.GameContent.ItemDropRules;
using veryawesomemod.Content.Items.Weapons;
using veryawesomemod.Content.Pets.LoamGF;

namespace veryawesomemod.Content.NPCs
{
	// This ModNPC serves as an example of a completely custom AI.
	[AutoloadBossHead]
	public class SandyLoamNPC : ModNPC
	{
		// Here we define an enum we will use with the State slot. Using an ai slot as a means to store "state" can simplify things greatly. Think flowchart.
		private enum ActionState
		{
			Start,
			Attack1,
			Attack2,
			Fall
		}

		// These are reference properties. One, for example, lets us write AI_State as if it's NPC.ai[0], essentially giving the index zero our own name.
		// Here they help to keep our AI code clear of clutter. Without them, every instance of "AI_State" in the AI code below would be "npc.ai[0]", which is quite hard to read.
		// This is all to just make beautiful, manageable, and clean code.
		public ref float AI_State => ref NPC.ai[0];
		public ref float AI_Timer => ref NPC.ai[1];

		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Sandy Loam"); // Automatic from localization files
			
			Main.npcFrameCount[NPC.type] = 1; // make sure to set this for your modnpcs.

			// Specify the debuffs it is immune to
			NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData {
				SpecificallyImmuneTo = new int[] {
					BuffID.Poisoned,
                    BuffID.Ichor
                    
				}
			});
		}

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {

            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SandysLoamShovel>(), 1));

            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<LoamPetItem>(), 1));

        }

		public override void SetDefaults() {
			NPC.width = 32; // The width of the npc's hitbox (in pixels)
			NPC.height = 64; // The height of the npc's hitbox (in pixels)
			NPC.aiStyle = -1; // This npc has a completely unique AI, so we set this to -1. The default aiStyle 0 will face the player, which might conflict with custom AI code.
			NPC.damage = 350; // The amount of damage that this npc deals
			NPC.defense = 120; // The amount of defense that this npc has
			NPC.lifeMax = 155000; // The amount of health that this npc has
			NPC.HitSound = SoundID.Thunder; // The sound the NPC will make when being hit.
			NPC.DeathSound = SoundID.MoonLord; // The sound the NPC will make when it dies.
			NPC.value = 25f; // How many copper coins the NPC will drop when killed.
			NPC.boss = true;		
			NPC.BossBar = ModContent.GetInstance<SandyLoamBossBar>();
			NPC.knockBackResist = 0f;
		}

		private int shotDamage = 35;
		public override float SpawnChance(NPCSpawnInfo spawnInfo) {
			// we would like this npc to spawn in the overworld.
            
			return SpawnCondition.Cavern.Chance;
		}
		float Boss_Fight_Time;
		// Our AI here makes our NPC sit waiting for a player to enter range, jumps to attack, flutter mid-fall to stay afloat a little longer, then falls to the ground. Note that animation should happen in FindFrame
		public override void AI() {
			Player player = Main.player[NPC.target];
			Boss_Fight_Time++;
			
			// The npc starts in the asleep state, waiting for a player to enter range
			switch (AI_State) {
				case (float)ActionState.Start:
					Main.NewText("Sandy Loam, who is she?");
					AI_State = (float)ActionState.Attack1;
					break; 
				case (float)ActionState.Attack1:
					Attack1();
					break; 
				case (float)ActionState.Attack2:
					Attack2();
					break; 
				case (float)ActionState.Fall:
					if (NPC.velocity.Y == 0) {
						NPC.velocity.X = 0;
						AI_State = (float)ActionState.Attack1;
						AI_Timer = 0;
					}

					break;
			}
		}


		// Here, because we use custom AI (aiStyle not set to a suitable vanilla value), we should manually decide when Flutter Slime can fall through platforms
		public override bool? CanFallThroughPlatforms() {
			if (AI_State == (float)ActionState.Fall && NPC.HasValidTarget && Main.player[NPC.target].Top.Y > NPC.Bottom.Y) {
				// If Flutter Slime is currently falling, we want it to keep falling through platforms as long as it's above the player
				return true;
			}

			return false;
			// You could also return null here to apply vanilla behavior (which is the same as false for custom AI)
		}
		
		float Shooting_Timer;
		
		private void Attack1() {
			AI_Timer++;
            Shooting_Timer++;

			if (Shooting_Timer == 15) {
				NPC.TargetClosest();
				Player player = Main.player[NPC.target];
				if (NPC.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient) {
					NPC.position.X = player.position.X - 300;
					NPC.position.Y = player.position.Y - 300; 
					Vector2 position = NPC.Center;
					Vector2 targetPosition = Main.player[NPC.target].Center;
    				Vector2 direction = targetPosition - position;
    				float speed = 10f;
					NPC.noGravity = true;
    				Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, direction * speed, ProjectileID.WoodenArrowHostile, NPC.damage, 0f, Main.myPlayer, NPC.whoAmI);
				}
				Shooting_Timer = 0;
			}
			else if (AI_Timer > 1000 && Main.netMode != NetmodeID.MultiplayerClient) {
				AI_State = (float)ActionState.Attack2;
				AI_Timer = 0;
			}
		}

		private void Attack2() {
			AI_Timer++;
            Shooting_Timer++;
			if (Shooting_Timer == 20) {
				NPC.TargetClosest();
				
				if (NPC.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient) {
					NPC.noGravity = false;
    				var source = NPC.GetSource_FromAI();
    				Vector2 position = NPC.Center;
    				Vector2 targetPosition = Main.player[NPC.target].Center;
    				Vector2 direction = targetPosition - position;
    				direction.Normalize();
    				float speed = 10f;
					int type = ModContent.ProjectileType<SandyLoamBossAttack>();
    				int damage = NPC.damage; 
    				Projectile.NewProjectile(source, position, direction * speed, type, damage, 0f, Main.myPlayer);
				}
				Shooting_Timer = 0;
			}
			else if (AI_Timer > 1000 && Main.netMode != NetmodeID.MultiplayerClient) {
				AI_State = (float)ActionState.Attack1;
				AI_Timer = 0;
			}

		}

	}
}