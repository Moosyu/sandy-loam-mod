using veryawesomemod.Content.Projectiles;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace veryawesomemod.Content.Items.Weapons
{
	public class SandysLoamShovel : ModItem
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Sandy's Loam Shovel");
			Tooltip.SetDefault("You've never dug 30ft down");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}
		public override void SetDefaults() {
			Item.width = 16; 
			Item.height = 16; 
			Item.autoReuse = true;  
			Item.damage = 1000; 
			Item.knockBack = 2f; 
			Item.noMelee = true; 
			Item.rare = ItemRarityID.Yellow; 
			Item.shootSpeed = 30f; 
			Item.useAnimation = 10; 
			Item.useTime = 10; 
			Item.UseSound = SoundID.Item11; 
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.value = Item.buyPrice(gold: 1); 
			Item.shoot = ModContent.ProjectileType<SandyLoam>();
		}

	}
}
