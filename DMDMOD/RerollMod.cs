using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using MelonLoader;
using Death;
using Death.App;
using Harmony;
using UnityEngine.UI;
using UnityEngine.Events;
using Death.Run.Systems;
using TMPro;
using Death.Items;
using System.Runtime.Serialization;
using Death.Run.Behaviours;
using Death.Data;
using System.Data;
using Death.Utils;
using MelonLoader.TinyJSON;
using System.IO;
using Newtonsoft.Json;


namespace DMDMOD
{
    public class RerollMod : MelonMod
    {

        GameObject rerollButton;
        GameObject rerollText;
        int rerollPriceInit = 10000;
        int rerollPrice = 10000;
        int rerollPriceIncrease = 5000;


        float resetThreashold = 1200;

        System_TimeTracker timeTracker;

        float timer = 0;
        bool runStarted = false;

    

        void buildCustomEquipUI()
        {


 

            GameObject inputLabel = new GameObject();

            inputLabel.AddComponent<RectTransform>();
            inputLabel.AddComponent<CanvasRenderer>();
            inputLabel.AddComponent<Text>();

            inputLabel.transform.SetParent(input.transform, false);

            InputField inField = input.GetComponent<InputField>();

            inField.textComponent = inputLabel.GetComponent<Text>();


            GameObject label = new GameObject();



            //label.transform.SetParent(dropDownAffixOne.transform, false);




            label.AddComponent<RectTransform>();
            label.AddComponent<CanvasRenderer>();
            label.AddComponent<Text>();


            //dropDownAffixOne.GetComponent<Dropdown>().captionText = label.GetComponent<Text>();
            


            List<string> affixCodes = new List<string>() {};
            List<string> affixRelated = new List<string>();
            List<string> affixMaxVals = new List<string>();
            List<string> affixAbilities = new List<string>();

            List<ItemAffix> affixList = Database.ItemAffixes.All.ToList();



            foreach (ItemAffix affix in affixList)
            {
               // affixCodes.Add(affix.Code.ToString());
                
                affixCodes.Add(affix.Code.ToString());
                affixRelated.Add(affix.RelatedKeywords.ToString());
                affixAbilities.Add(affix.Abilities.ToString());

            }

            System.IO.File.WriteAllLines("Mods/RerollResources/affixCodes.txt", affixCodes.ToArray());
            System.IO.File.WriteAllLines("Mods/RerollResources/affixAbilities.txt", affixAbilities.ToArray());
            System.IO.File.WriteAllLines("Mods/RerollResources/affixRelatedKeywords.txt", affixRelated.ToArray());


            foreach (string affixCode in affixCodes)
            {
                //dropDownAffixOne.GetComponent<Dropdown>().options.Add(optionData(affixCode));
            }
     
        }


        Dropdown.OptionData optionData (string text)
        {
            Dropdown.OptionData oData = new Dropdown.OptionData();
            oData.text = text;

            return oData;
        }


        void resetRerollPrice()
        {
            rerollPrice = rerollPriceInit;
        }


        private Item.AffixReference getConvertedAffix(string code, int levels)
        {
            ItemSaveData.AffixReference stat = new ItemSaveData.AffixReference();
            stat.Code = code;
            stat.Levels = levels;
            Item.AffixReference convertedAffix = new Item.AffixReference(stat);
            
            return convertedAffix;


        }



        void SpawnItemFromJson()
        {



            string jstring = File.ReadAllText("mods/RerollResources/item.json");


            itemIn iI = JsonConvert.DeserializeObject<itemIn>(jstring);

            LoggerInstance.Msg("ID: " + iI.ID);
            LoggerInstance.Msg("Type: " + iI.type);
            LoggerInstance.Msg("Tier: " + iI.tier);
            LoggerInstance.Msg("Rarity: " + iI.rarity);
            LoggerInstance.Msg("IsUnique: " + iI.isUnique);


            List<Item.AffixReference> affixList = new List<Item.AffixReference>();

            foreach (affix aff in iI.affixes)
            {

                affixList.Add(getConvertedAffix(aff.code, aff.level));

                LoggerInstance.Msg(aff.code);
                LoggerInstance.Msg(aff.level);
            }

            TierId[] tiers = TierId.GetAll().ToArray();



            List<Item.AffixReference> list = new List<Item.AffixReference>();

            list.Add(getConvertedAffix("a", 120));
            list.Add(getConvertedAffix("pd%", 120));
            list.Add(getConvertedAffix("h", 120));
            list.Add(getConvertedAffix("msm%", 120));




            Item tempItem = new Item(iI.ID, ItemType.Head, ItemClass.FromCode("light"), ItemRarity.Mythic, tiers[1], iI.isUnique, "Head_Kabuto_02", "default", "default", affixList);

            ItemSpawner iSpawner = GameObject.Find("ItemSpawner").GetComponent<ItemSpawner>();


            Transform t = GameObject.Find("_S_Character_Fab_Light(Clone)").transform;


            if (t != null)
            {

                Vector2 coords = new Vector2(t.position.x, t.position.y);
                iSpawner.DropItem(coords, tempItem);
            }
            else
            {
                iSpawner.DropItem(new Vector2(0, 0), tempItem);
            }

        }

        void spawnItem()
        {

            List <ItemAffix> affixList = Database.ItemAffixes.All.ToList();


            foreach (ItemAffix affix in affixList) {

                LoggerInstance.Msg(affix.Code);
            }



            List<Item.AffixReference> list = new List<Item.AffixReference>();

           list.Add(getConvertedAffix("a", 120));
           list.Add(getConvertedAffix("pd%", 120));
           list.Add(getConvertedAffix("h", 120));


            TierId[] tiers = TierId.GetAll().ToArray();
         

            
            
            Item tempItem = new Item("Wood-was-here", ItemType.Head, ItemClass.FromCode("light"), ItemRarity.Mythic, tiers[1], false, "Head_Kabuto_02", "default", "default", list);

            ItemSpawner iSpawner = GameObject.Find("ItemSpawner").GetComponent<ItemSpawner>();


            Transform t = GameObject.Find("_S_Character_Fab_Light(Clone)").transform;


            if (t != null) {

                Vector2 coords = new Vector2(t.position.x, t.position.y);
                iSpawner.DropItem(coords, tempItem);
            }
            else
            {
               iSpawner.DropItem(new Vector2(0, 0), tempItem);
        }

        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {


            if (sceneName == "Scene_Run")
            {

                timeTracker = GameObject.Find("Facade").GetComponent<System_TimeTracker>();
                runStarted = true;


            }

            if (sceneName == "Scene_LobbyGUI")
            {
                GameObject guiPanel = GameObject.Find("GUI_Panel_Shop");

                rerollButton = new GameObject();

                rerollButton.name = "RerollButton";
                rerollButton.transform.SetParent(guiPanel.transform, false);

                rerollButton.AddComponent<CanvasRenderer>();
                rerollButton.AddComponent<RectTransform>();
                rerollButton.AddComponent<Image>();
                rerollButton.AddComponent<Button>();

                rerollButton.transform.position = new Vector3(730, 140, 0);
                
                rerollButton.GetComponent<Image>().sprite = loadSpriteFromPath("Mods/RerollResources/ROLL.png");

                rerollButton.GetComponent<Button>().onClick.AddListener(RerollShop);


                rerollText = new GameObject();
                rerollText.name = "RerollText";
                rerollText.transform.SetParent (rerollButton.transform, false);

                rerollText.AddComponent<RectTransform>();
                rerollText.AddComponent<CanvasRenderer>();
                rerollText.AddComponent<TextMeshProUGUI>();

                rerollText.transform.position = new Vector3(780, 208, 0);


                rerollText.GetComponent<TextMeshProUGUI>().text = rerollPrice.ToString();

                buildCustomEquipUI();



            }
        }



        public void jsontest()
        {


            string jstring = File.ReadAllText("mods/RerollResources/item.json");


            itemIn iI = JsonConvert.DeserializeObject<itemIn>(jstring);

    
            LoggerInstance.Msg("ID: " + iI.ID);
            LoggerInstance.Msg("Type: " + iI.type);
            LoggerInstance.Msg("Tier: " + iI.tier);
            LoggerInstance.Msg("Rarity: " + iI.rarity);
            LoggerInstance.Msg("IsUnique: " + iI.isUnique);

            List<Item.AffixReference> affixList = new List<Item.AffixReference>();

            foreach (affix aff in iI.affixes)
            {

                affixList.Add(getConvertedAffix(aff.code, aff.level));

                LoggerInstance.Msg(aff.code);
                LoggerInstance.Msg(aff.level);
            }
        }



        [System.Serializable]
        public class itemIn
        {
            public string ID;
            public string type;
            public int tier;
            public string rarity;
            public bool isUnique;

            public affix[] affixes;


        }

        [System.Serializable]
        public class affix
        {
            public string code;
            public int level;
        }


        Sprite loadSpriteFromPath(string path)
        {
            

            Texture2D tex;
            byte[] fileData;

            fileData = System.IO.File.ReadAllBytes(path);

            tex = new Texture2D(2, 2);
            ImageConversion.LoadImage(tex, fileData);

            tex.filterMode = FilterMode.Point;

            Sprite nSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0), 100, 0, SpriteMeshType.Tight);

            return nSprite;

        }


        
       

        public override void OnUpdate()
        {

            if(timeTracker != null) {

                timer = timeTracker.RunTimeSec;

            }


            if (timer > resetThreashold && runStarted == true)
            {

                resetRerollPrice();
                LoggerInstance.Msg("Resetted Prices");
                runStarted = false;

            }


            if(Input.GetKey(KeyCode.L)) {

                jsontest();
                SpawnItemFromJson();

            }



            if (Input.GetKeyDown(KeyCode.J))
            {

                spawnItem();


                /*
                GameManager gm = GameObject.Find("GameManager").GetComponent<GameManager>();

                Profile p = gm.ProfileManager.Active;

                p.Gold += 10000;

                */
            }

            
        }


        public void RerollShop()
        {
            GameManager gm = GameObject.Find("GameManager").GetComponent<GameManager>();

            Profile p = gm.ProfileManager.Active;



            if (p.Gold >= rerollPrice)
            {
                rerollButton.GetComponent<Image>().color = new Vector4(0, 153, 51, 255);
                p.ReGenerateShop();

                p.Gold -= rerollPrice;
                rerollPrice += rerollPriceIncrease;
            }
            else
            {
                rerollButton.GetComponent<Image>().color = new Vector4(128, 0, 0, 255);
            }

            rerollText.GetComponent<TextMeshProUGUI>().text = rerollPrice.ToString();


        }

    }
}
