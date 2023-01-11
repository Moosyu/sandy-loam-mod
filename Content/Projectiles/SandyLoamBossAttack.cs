using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace veryawesomemod.Content.Projectiles
{
	public class SandyLoamBossAttack : ModProjectile
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Sandy Loam"); 
		}

		public override void SetDefaults() {
			Projectile.width = 16;
			Projectile.height = 16;
			Projectile.aiStyle = 0;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.ignoreWater = true;
			Projectile.light = 1f;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 600;
			AIType = ProjectileID.WoodenArrowHostile;
		}

		// Custom AI
	}
}
