﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019  Dirk Stolle

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using updater.data;
using updater.versions;

namespace updater.software
{
    /// <summary>
    /// Firefox Developer Edition (i.e. aurora channel)
    /// </summary>
    public class FirefoxAurora : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for FirefoxAurora class
        /// </summary>
        private static NLog.Logger logger = NLog.LogManager.GetLogger(typeof(FirefoxAurora).FullName);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "71.0b4";

        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox Developer Edition software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public FirefoxAurora(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException("langCode", "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var validCodes = validLanguageCodes();
            if (!validCodes.Contains<string>(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
            }
            //Do not set checksum explicitly, because aurora releases change too often.
            // Instead we try to get them on demand, when needed.
            checksum32Bit = knownChecksums32Bit()[langCode];
            checksum64Bit = knownChecksums64Bit()[langCode];
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/71.0b4/SHA512SUMS
            var result = new Dictionary<string, string>();

            result.Add("ach", "8e00aa343938340f01106c956b9193cb473d1c083851ffbf935b9f39a4421a639a92fb84263012cae174b7821e4c2ee6334132253566660885362327a6fdebff");
            result.Add("af", "d09c1ed7f68f24e65c058f788f1aa31d776926b4a1c04524d8465562fd60114b153847f0ff559217732ea21237207411ba2bdd5b56cbe97dbb7d3352ca8a3221");
            result.Add("an", "fb0510fc7c860825c5fb9fcb615a969d7dde37336a6ce5bea714f27b0db3142c33d87bca5e3213f8680913a327074388a4c0935d8a5eff93fd640dc76a3f8919");
            result.Add("ar", "d4f56668e4a72c4427d21d9fe71a2db51ddbfb7cebf702d801b3069ac00507aff66b16bc756ca983128b22339a44ea3b623465f70da5b745247af342e1fa62e0");
            result.Add("ast", "eaa5d7ee004bbbde56c38fa715ce6e18687b1bd170ce3ebfca79aaa907bfdab9502c4eba97563a78285af12bd2decbbf17190405a66374c5ea648199c30c6830");
            result.Add("az", "db19e421df0fe5b32fbdbef9ee084725a13d202155e352e574eec2cf4215738d8ec327cc269f3766a3d98f4f30373cc2839a9d33ec340659782545e6c6c1ca01");
            result.Add("be", "02f40193f573a41227521883fd3e33bebaac0e578c44b071bc01e196b7b277312a968bd845379c90c2ac2b2cc5d9188be00b81d20d2c48e26432366eeaf4d363");
            result.Add("bg", "96bd63bd354c5a967ae0bce413d57ea551025fe66e2aa712827ca1e7aa017ef5b77c6d642e8c35319f104aa8c047cd2becb1cfcb00bdb6067274688659d0217e");
            result.Add("bn", "fb499238dc2ac5fb3fef45a1351215407a6f3286f5f17063d16901e1ecad43181aade2993c76534f266ed94d2ff5b3c0f35aa4ca3d78584f48e696d56a568478");
            result.Add("br", "82ff71a11c5b267c7c42c4bde783c452c58726401e530ca1d95bd338f08acb84dd6aa50d9b00d7130cbc5cdd690e7c2ff1270286ef5a5e12fe00c8d6169fff64");
            result.Add("bs", "dd5a48782ef8351762074ffb95b7b7d63b0910f1f310eee52ea22ceaeae14839715952a6e79e200a9c3a8651b67f76a9b5addf9f585af36f703dc48c85d8bcd1");
            result.Add("ca", "ef3b6f1199d3369368782c7a39e3f48660ea40e5cf76e8791430146d149b27e2ab969e859f13d3338f8c6ad6d99cc7676f9032a4019a52fe04ba17dae54c1d75");
            result.Add("cak", "0c9c51c04d0614915fa601e8b4d6496add84147b765f353967671038a2e7d4ca1c28a660566830dc1902d0aaeffe5cd77c714c9787583b3d4ab6cd33059d4202");
            result.Add("cs", "d9b945f0e17138d38bddb040e0a0ee81e1062673088fd957edf4bf689b796d2a7efb5bee28f9ae5e8d4a3b1b9b0f63baa299eb259367f8332001306d3aa6ec61");
            result.Add("cy", "0e83588ccbb0385e16aae3e70bffa7a05e0206402e3eb2e1d4806b94ec368ca70966dfee03a15bd5b296a89b986351f888baf95e05a3bfb711d7c3e5d9e0ecfc");
            result.Add("da", "6b303a4fc9f5ed14bf5444619acd2f9775a6513874c5ce6518431a331bcb79407c7d1871534853d2366ecd91382f7aeac8bd4ff46d59c900dadd425bd4b297e2");
            result.Add("de", "3c3ef3ce80829f741faba7b4bd1b8d6ed9b8b6a0eca9e0ef8fd4e91c34281add554a543d576bd4cf350b588ed56bd75d89c5dab68d5cc5aabada9a783f509364");
            result.Add("dsb", "ce6ff43f4007c1ce8f44998ec98cf36c3c4958c6f82a92cbeb57b65234440dfae79e436f8ed99a8aa0a6e41ab45c6dec8b2e2a4aac8822f3b55b17a38e79b397");
            result.Add("el", "33a5609a4ede9d3d2f2aee6f88f6adcf42b7d8d154b83c5f91cfbb54f83ba075f6808d89209b4124197092017154562f17f00433f2b368b18e55dbff83ae6012");
            result.Add("en-CA", "6ee56fabbdadd03265e76f637cb76dcebcc9a7fa0eec651b00dcba0b8e0046b4c2071fe78e258cce73069d834cc5c964cabdb3fc45cac1579e50ea3c6bde9d14");
            result.Add("en-GB", "6bc04a2b48b90a4e9c8a22070edbe6105c0ee0ad5b43fd2ff5e8de75b54021823c6d8e674f35c2e7210224a93eb2117371b6336bdb4239bbd82d0b5e6184f563");
            result.Add("en-US", "13650cf2906d4f46fb19ba7fc5689edca9853af5d86e6c068cee4b91422d9fb64ef63c554b61d5b4f761e8f50061bdeace7fe613fdee1df3deaa3df488956d19");
            result.Add("eo", "4d4e2530a01cd4cf896f6a35c7d5ef291e30c4aa1fc2b8f23552976aaf07692e42b98e37305ba51187ead5ec1c1075913451a6ebb114326a9e13bcac314646a0");
            result.Add("es-AR", "e85c9673827828d2b1f16686f0fb518adfccaffb1e7883a7614a1e574587688af9e543371c3710f5de5c4c32a2828ccfb8ba8f1239fc9144930e02097e4d7577");
            result.Add("es-CL", "3ad43258ebe8e2be139a01bc31f83f2f8948ff682dafe62290e0388c10fa586d1fe7760ca7f0d0b09299692f5dcd2352aa1c10275cfb0996a55b2a17012b8def");
            result.Add("es-ES", "841b52ee48579ff6bde43da6c8a48f21c81df64fa6b53e83d9d560a5aed3d02c9ff65ce85adbab1182dd451b74158d57b903792f2bee3223f70d8e43413b70c7");
            result.Add("es-MX", "772ffbc1a028b0dd654313de5a0a26e6db240522bb58db5cdc213e331dba66c15d8d8a32ac4793ed94b3ba78bbf1710cdef737cf233c1b8e29a42fc3b4d782d8");
            result.Add("et", "0a0b394be15e9086ee8d6ef6e09d4119b1ee8b82aea44e9a491c9aee34acc44648d24eeef4cb876a1d6a350f6554dfc3023742ef726c54637dee1d48c851fdaf");
            result.Add("eu", "8e86ebef0ffa3d20aa34e31aeb6bee1a3b4c556ac878bfa24a3913280956220f3975ede5c903f9282a351c15322d81c842a7bef187402a3fbbb6933855a8d1ca");
            result.Add("fa", "63ef2b93a0792eaae9917b5a9b6cc16ef399821a05e08b61019b569f1e04db60a444c72ca1f6601fb5f4b7a0dd81418fe89dcfc3968eb3c1fb1cb095a3e6b60c");
            result.Add("ff", "73dd3404341e43b21ae13d66cf947992cde168d625d2d2dac7c2d5b586dcd1526ac98f88385679b65f71c0e89d56d4cc3ac3bcede9d039c211050e78fb83ae8a");
            result.Add("fi", "7ec579dddec365b5219ac54cd3591eecfed77d3963dae9d53f31ab47924aa689199cf0aa5ae0a1d32dad58d7da09628139f71e3bb192ee298c3bd3b2fff81bd4");
            result.Add("fr", "2899e50e6a5e1f7fb938f67111b4ac19e55a9a989db6b0a0269b834dc2a507dc8f46fb017f95564fc3add6b8861bb2385493fead32f58b0000c43d11a9b85dc8");
            result.Add("fy-NL", "1264f637971c95d5609ed8aa57c3c7044ac1179a08747d763fc02f0b2f9f2b436d8e065a1d662c82acd5f6ecc9e8ccf4b8f2e9e579519e22e2bbe4ef10282f7b");
            result.Add("ga-IE", "c5ee86e2618452e2d6a2416a7989d95a160c6442ca74efa156a101464f910460a28f4c580277e07a3bef523e6116ccfa9afd34ede63bb855ad86e10d0326ef6e");
            result.Add("gd", "9c2435c26c77464106c00d1c04e83f8824cd68c4c608edb28b898dc6a4588a3946cf1c493cdcd3d756fc969ad619e54f94a4cc14b398e1387e35a257425012c5");
            result.Add("gl", "3821b2b5a7dfd2b999f6acbd4c591f9982bfc79a130b5c91e3deef8bb91f72d528fbe4bf1cdfc111089f474c6e116d2e3e0c081def720bc345f3e3f79ce0a2e1");
            result.Add("gn", "d1ab67d7be828a0228728a9e039ea2b65b9eeffef1084fd5f5c7e9df824a9d6355ab310f8bebf31670ea01c5561d80ca318ac6577172317e065a80a5f7652704");
            result.Add("gu-IN", "870b6ed2e095186bf0eea290a2309406e3213dbea298deb79b79b611cacd9b126e45553de056eb08612b070c185854f2b016844a77435ebeea6c01f4ebdb957e");
            result.Add("he", "7910d7dd6ec14314a3f4dc90c4ddd8ef7d196c9d603c4083b48510a4341dcd7a8b7180bbfb7c6123cb860a2b8e67fd5beb00f891e92fe83fa74111b5401f0526");
            result.Add("hi-IN", "cb06bc5b305bc041cc8189af151c530b22f83af7be6f82b724f308c5ced390eecdfa7f6222d5b5225a5cc28ece891ebefa64e3df39c3b8982700b67742fa595f");
            result.Add("hr", "8eb9815a931fb6c72d0625b7bc87f30a480b35e9abe14c85d75bbfaf674eb8bec346efa09a471dd52dd2f2dfa6f5d9f6fad1233a6598421936d37c25cbe82b05");
            result.Add("hsb", "f4c7a2b1036d7b0cc31180d4b23d0c7d7236d845de27436d7b05cbe390318f2b15717ca0f0ded2ff8b62584c67caf4b46c974fdd1a8825dc40fe6a883b1ca8fb");
            result.Add("hu", "d844fec3e18813a53605d62bec01046186bb5e93b9ff7af2d39ba96da7ec9b5781586eca5868581567b5f4850ec6298789dd812b4183164826d3b2110a9a7188");
            result.Add("hy-AM", "8cda6e4176318d2364e63a6f258b025ce15d83afbc2a060f534a43ea31a7579cf1faf83644afb1f792223bdfd830c267f0d0ed6c8cef6a9d44f248bac0d51d5c");
            result.Add("ia", "192f57d004f5f9bc15aee1bd98eed26432ab545cabffb7999d8934441c2e4882d28c785ccfbfe7d30445fdb889139ce6cf7c35c4ceab5730792e8a2806ae2c58");
            result.Add("id", "bfc93533c44b5039fbe93c17098ef2c9c7a30e591aaceeceb3106a3754c4a090d984e5c0b7072ea25b72e7e19e10d5b21ba1a8951e9e92d6054f8d1090978926");
            result.Add("is", "f78f7e3e8616b2be84a8839cb2fc95bd4e8a07e09eb0a196b5f6ec9f533a0438baa00986e579a9522288730ef4867266d2c307c16a8a4a203cadc442ae463719");
            result.Add("it", "3a768db31f772bd4891881cc4a89b8bf4b683abc2a085ae06e714ad2f523f88df1192f61c902053d5cf79f9f6207727597b1ca8754126cc0437b47e89be4c006");
            result.Add("ja", "eadd1d47e1631645851218be4cbc47a5923370f2f29be614171f1e0db78b2705a354dae1dd809c3fa24cce1baf72a25eb144d495bacd234010fda2e1eda6d7ec");
            result.Add("ka", "15887ca19b46eb001bf9e5c63d86b7e73a305c851eb763e01b2a6699f103a27632aa0ec7af456301f85b4499b31114d5542a620965ad7a925864d2e9a75017dc");
            result.Add("kab", "ed243a4cf8fb75915c69cf7110d23a23531e93141a0af64ab8c61c57476880fdf35ed7bdb6f2b6a4dd27a0aca3fbea7246d99a116ba984e43bae97cbb151f448");
            result.Add("kk", "1fb0804af89a9c44bcbf12e392876de48af9e1d9e5e785865ee336a6f8785cab9d62fc7222f58ca2c0922b4527ca449f6c661f312622ffd3cbe5af18994256df");
            result.Add("km", "b1e0899d42aaee741de5081f5cfbe3e4797614a5e2707b01b6e49d496b314be8fc5a341ab5ae22218607d401bfb8e27a7e3058b893c11869f4d9daa4c356e3f9");
            result.Add("kn", "8a0867b741cca9d2fb3eb5c311bba0584905d586ca5a589490dab220db6ffaf2f4eb1026dbb44261c4d73f762a6cf69780f209c1823e646e10121ec3581f4bc3");
            result.Add("ko", "f36be6b49f19ccb2214dc837765a0e2cf7d5e1e697139b88a65122b54677776d09c72a307750dbfe91e7d547cb25e18e02fe1897b982a8b7e534355cb0cc3a02");
            result.Add("lij", "f8915527aab03064d786f086a01d4b104cf4a67194668570947d5a5d06e0ae84bc945bdaf4cc8f924f75329711b655f02f61bcdf778f6de33aaf8fbe5d51cfa0");
            result.Add("lt", "3106048b005271eb760e8bdaddf9141a6fd81af472852ae04da39f422188d43c39458349fd555cf5633786b885427e65f04fa5723ead44f19824936f9ce6bed6");
            result.Add("lv", "b5173682c68317399be52f563a6d7a45461c5dadb35f694b01cb1cf7c433b118f5d63464987c165069c2940e85bbe14ec7d0bdf7463186a9cb065df5c1dff0bc");
            result.Add("mk", "d798df940b59700fa0f3ee9d3803c0fc08ffd7dc0e3c2e9eaf4df781f40cb96eac643fff40ef5bf8df8f246cb1373db7638486157659564547167b7bb4a5f9fd");
            result.Add("mr", "8132b9c8fbf0a0a9c7a36ab048da26a75e43634dafb4313526b012f5cfa8840ba2b0f183fe8fc2c52a5d83b8149824e1d47d31b4e900c0f202caf332adf8464b");
            result.Add("ms", "40654b0200a1015151682a709061c128b815bc35dae39fac0893943824daa03f965ad5be0d5f8a59dd03eb3d67d43c524a583c1b613c4fa5425e82028af8f113");
            result.Add("my", "29758e793b5d4e947d72ee825001136759407ff3bf3f886b02b9baa493c30ab80e360bee29607255e58b18462c62e2716e7d73ba2f7a085dca087ca256dc728b");
            result.Add("nb-NO", "3322f8cb38285cc31842baafdfde09c0faa71ee075e52dd098eaf0b3f3bfd6a7d4ab5bab9f02560c1aa5e36dc82367343181e05b3b4fdcad65921775b921f5b5");
            result.Add("ne-NP", "286c7030718742e0726760f3fb06a39b6ee45c826bd7c24278005a979b683666d58afc1ac31481afecc412eb147b7d4f79f3ec384dabe8245c61ef50b591bd15");
            result.Add("nl", "30545831f6fe60c76cda26ad108a3e0ec3258a9c4681540e803c251c935179215f42fdb3b18bea062e4655f71b925637b52986e2c5e99a7ccb5ad05e5a2c5c0b");
            result.Add("nn-NO", "fa644fa353c32be151c64654bdb84bc42748a81204d11185ab3ba56e3f8a12fe2796f958f7875a9080b0627775f2cc213a8936a1bd7bece6d2f82b186087aa69");
            result.Add("oc", "7761108ef1e42b844e8cf63f259a4ff5b8a0c6e219e94a18c5fed678b25eec990fbc1107f2a9678bc195cd65074e332e6bd9bf1c11ca8f838021dbd63c08a223");
            result.Add("pa-IN", "06a4f6e1382f1f0150399e3e43994587b698214b82f7b2309a088fa60cc757b2f1baed9248475feee9d0f5f9a91aac5ce180793924c01f03506a54e22b5494c3");
            result.Add("pl", "372b0a71156ace9bd70eae7df48e6ae4633f7d3413d7ff0f58497b3ff8367f4898b3ab7c4afd01b9db145f0acd665be189f23484aa28a0ce02deb82172feae02");
            result.Add("pt-BR", "b241489b65eb98347df33e327e3efea5e3dada3dae8a7bbe35b303936d55955d2c82a4abb7a2a0b39d143449580e28b20cf46ae997632e17e3dc26ccc2d12ba9");
            result.Add("pt-PT", "4a82d101f788108000b4a2686ffd92e8cdf70af3c60e06a866630575c2f7b71d60bf6cd91fc24d1da4ecfeb2491891aadfa97ddcf79a296cd38146977e6392cf");
            result.Add("rm", "936a883cab13e5f766b49348e32c47525a944ea22f7af244266ee76a68c78d21b792b4dbe356e120ec720fcc83d19a481592d100ad94adafbc42f3cdb06a184b");
            result.Add("ro", "9cd77564c312229ee6ef180f9bb00f1211ba20dfcbd7a429d6c47652920d686fd1e88eea45ca1f127e0cc6b8dbaa201cf2696d89186e21e8a229dcc3c9be154f");
            result.Add("ru", "1b6735399a77426d7df0021ee9786a8aeb2997d7c2b9c5d2e37591b549bc4e4c5202bea1729edc7a5ed04a5b57b82600ef7bbafe6fd2eb2fef5b26c4f0cbc458");
            result.Add("si", "ee0461ed679c19b6d5e427d40ea0acb13c459676df11b6e81c2c99bd524b18ee28f54ca93aa1b4b7134581df75435598f47be9187546ae4da5bd7cf4d05fcd47");
            result.Add("sk", "d2debe750efd6643af9d6ccf4250f760b028899caab7ab9faf81f612dc1ac5c4b5684450fe4666d664115677d71fa6860ec57a9f6a0fdb06a2c57ec7b703b664");
            result.Add("sl", "f663c3b85a7ba9c25fe70557a6b972315e63c892bc7a45451737a40ba3e85f5a041a3961de631327dc2f2c12513e414d25dae74389e0520d60d1c0f4b4c402c0");
            result.Add("son", "67f72c660a6cdfb6c2181616a4aa090bc23bc203679af6cc957880b0648ad189a15dab92dbc6524dbaf0e8e4e6b422e8791af1b06bf2af45c8587a5561d0a3fc");
            result.Add("sq", "334d6b38868c44df76cfc5d74d94e1b1e1e3278614a2ecc8dfbd34caeac841d90c76a32db28c5c0700b48aadfb5807b925ecbf4b271954b3406636c20b83a75e");
            result.Add("sr", "61f5c124358a6d9d7f626fa90a1516ad9dc64e5a2d838a6a1503248ade1892dcbd039f4d5e4078f779aaecaa5e653b2d104270484835b9b36a6eddd6ca828fac");
            result.Add("sv-SE", "b937c876b18f53949d81b339a7df0b6f2a40dcf41cec330ec01b61a1406bf83db72e1f8b8f8c01df60047d925c5f2d9954c7107c34c652f0600672ad8c31ef5e");
            result.Add("ta", "3fef755ce6e66e1964b5234791853f455448642fbde00cf23c0ce3a6a510cc132f81d75594fdad8d3f9fd4ef7cb3220d915753b49b41f2fba13ef280da7941e1");
            result.Add("te", "0deacbd2131b4cbdf4c5282ea9887ab515d659040d64bf3f41e976c8f86fcb9e1d6f330dad0032f662080860bf9c7f5356d65527f9c6e6ddbe2347146de0b2e1");
            result.Add("th", "aed3c5dfba60b34bae2f92b2bf0fbfacba6ec3633f081f1166749a682e7675dfd4e8e3460f7ed1d9edd751b464a6b31a69ff27f23f4a08c1af7bdd54bb0fc3a2");
            result.Add("tl", "9bb053a1560167e1e4a3de9a6c93624cb5dc01595bf18ca4158a34f32d6f72a2df3be4bef4b64949f4aef410d1d99203461317a539362ecdbab0da458391bb28");
            result.Add("tr", "abcb3d9ca4cd54366e33e8c9b10d721baa45c21486dd868f6368749a6c75f0de64492f67089adb7a17d8a9b939383f715201021ac8633f15153440bebabc7371");
            result.Add("trs", "9f053fbe5fdd4ef710b98ecf3d923c915e6e6defa9d61214072a791d47dae9a7ccf5ffcf7d6dfbb9c8db8aebebb83c1afb601010c6294723364e6edb266ee480");
            result.Add("uk", "525a82e1d7c15627c070ff71d7cbfe9653432e8c0dff5e51663250bc0c96fc9624195f50fa7761872863821c1e855546ec8da9fd01493f92932e4cc7661dc702");
            result.Add("ur", "e05afb858cfbf65501fc472789065a52372cf0360842588d1a750b0e022d4a115a14f3e28f8efc9fd0c87eee14aa5a65a582034a34d4ae300174ec0b63b716cd");
            result.Add("uz", "3ad149ca81a3c566cf28ebb9318ecd825e403567816308d479fe8436f2abb54721b4f1aed7d3120acae612ed5576abddc3113a5e612f74506f848e9a6fd38632");
            result.Add("vi", "94314615ea7cfb6c878145ad30ac48ebae1680e6681b63b5ab48e41a95bf92ff3e5cc5b05c9dcb9b1a4eaef38f9b2fd666eb7f92f4dfb70328d72d52ee8d2247");
            result.Add("xh", "09ed000d251153447ee357f1c87efa5db05d6172edf0bb4a15a681ccfaf9939b25a471fde710567fea19ce4b1b251a430af329cf014721fcdfdcfca5c20ec111");
            result.Add("zh-CN", "165d9eccc92358e51e21ebd63ac6107c6914c8d6d9e82ccda1ff0edf1f17044c688173afed1da815322cd0fb9493cf5a87c82261fb5ce460013806cb8da32830");
            result.Add("zh-TW", "0d2a8fdd741fb53d8a2d0608905ac415ec60ab9caeeecf7aed7a65e77ee76b408760deb316bf172cee3c909a36c2b11feef2e65fac745674e991f7700ed6c4f4");

            return result;
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/71.0b4/SHA512SUMS
            var result = new Dictionary<string, string>();
            result.Add("ach", "780082e7faf1800811723379598ebbbc2f69d4df9083e09a8bfa8965df7af08864db53b48e0754ce8842af0aa028ad17b5c47033741efa9149ee8542922bea69");
            result.Add("af", "ad6a84bf100c32405cdcf4f1a74a70acbc6c4671d9f1c3853623e198df7b7bb53cd41f158918c05b405621d3b06ef3247e3d14fae61f8a5ad6d5f19080bf6b75");
            result.Add("an", "58821514c36bb7470fb94ba74d5ca80aaf4a46f0c5f41b754cd39ba3e57d1c32ac700266668acd4af72a1635dd177c6d4ba5dd97eee680e1a8cdcbfe409a639a");
            result.Add("ar", "e02d687ec73ea28f1a5b653c0e7b5e16176cbe1be5f0920f04d522ee2b165e27d30f1e945534bead3e6e164b17bd6cf94c16ca6ef675382d04691c7f140dd1e3");
            result.Add("ast", "261564e725c2b5c72caefd5b28077b24e980ef9461429fb199343991278716785b1cc244e195a31a19764ba2bef6f71100ec1454aef11264670b4ee39cade98d");
            result.Add("az", "6e41dba9b72ace876f04c5f3b57df0c06285aa8808adbdd495204c7cbd678cecda47423d97031fab3b49b2cf282e314bd5020e9bb6956a450de809dbe0f7b41d");
            result.Add("be", "c1cec1cfe405d69dcf264df305a3e7ce4a972699fcb9394b11b912daba70fb657567c19f49c777170f7aaec23267b285cd00a6df1d0ecbabce1da319f75ea570");
            result.Add("bg", "e08551463186ea810e22319743974f57d9dd864aeb249b7b77e36f33bc8a1605fcb5fdad8a2ddf6c76a56473c1aae83df3788f215da6f3cf5b77671bfe4bc365");
            result.Add("bn", "c03e42775cec7a5939dcb6b7cc4b3b2278b8ef7a59afe84c3efcf3cfd1b3b1e7a7fb8fa8bf20d099051bc2a4461785ac88a1686b14333285f5c269e160756e09");
            result.Add("br", "9fd69d0fff5f732ae593a9ed312110019fa1421a1486ea6285df016fe4c67d169e806bfe74692cf2f5c097e98718a760326e3a42e83bec72ee21335662aa8eeb");
            result.Add("bs", "8c1d1ff5b6462a2111ba85d9dc39b44b25c6a3d6fd8a35c8a5a6bb86e6ffba1a7b291dda1ed17982256416011cab10ec484a6bd5bb44db272617f2b110c7647f");
            result.Add("ca", "6d78cc79734ce2f34747fd2d864bdc52a95b00a9d5c0d08117ee1024f3216075e54261cc95244e64371e91088709f7188197b0057eaedf5fd87e2d7231760523");
            result.Add("cak", "cc2fdeeb5996299a5f6ef7ce44ec9feb75c267d87ffca3b9998421b8edc5de4737c875a84a07579e923eb16b3ab316fe111fa29ee3e0759a975fa5f99c3891f7");
            result.Add("cs", "06d7d2c03e8fde0da17d1cb03a2c385ab17c7b40bc0293f1b65b15214535076fc16cc120c2ce5e3774404b80159e2b455b48b25c558d9682dd098ffaf77951fa");
            result.Add("cy", "0c4fa48700d50b25f4111d5f79b3ddf46a237235071dec6a58c7f8de8f2061e676a89e35e1cc5da1ce39a9aec3f17ce238fd0ba5b7591da801d16f8572069a2f");
            result.Add("da", "d1b976b919484352debd734c4dfbe7f6641ce9faa8311ca583907ff2b4010cc6725b3f17d522f55d2bf7ddf5fd054e614ad54a38f819f12172ae83f5a1c47e81");
            result.Add("de", "1d0b8cc989f342079f0b074978569bbed6c83f381a9f7404f4fd3b5c058326de1e18eec3684efd13f92ee35a279ce6d9c70f993c12d17a18b190596a8c9303cb");
            result.Add("dsb", "e5f83c2198816751c6a0a3dffa968cf1185ced084269cec9bf85407bcf30b9f13f821adb335099f8bb586ae522dbd6d6af44667aaeb8eaaf4f44e7fc674ac719");
            result.Add("el", "9f38a189946de893ef4e1aafd6c86aa7904769e170370acd4c9288343f57a95e17252981bf6681721c196b63e5974539601339ad404e5f6f54b0e9d37fbbc6fe");
            result.Add("en-CA", "45a2d03547ea758a387d2f09282838a17795352f7693837899b1a22b84d064a381128f76a348c5b3e52ad3ee28f9f54a2b36b9d38b9d470769bb9e0eef891910");
            result.Add("en-GB", "cf3e808462ad62631ce797bee391309d66e627ac25f6cceacb7f24f119d75f2b3acbe247d24d9b3fb919b218458fcfbff505699aa9111b6ff6d4eb3bac3a4e82");
            result.Add("en-US", "bf663bb4f80bd9841593ced7217ffa7da9f8d675d52f692457e0f550f7d615b579d0c849c18975bc27b1572a0188c5b29a632dcfa39c55ea5f9879b3072acef3");
            result.Add("eo", "a2b8baf31dba7d638f83c59a505243bd352ae383774f478e9bd5ee325d13c2c3fbc6880f5b200b627722a6a5c411d4113526641fcd3de37348abb2338b5759aa");
            result.Add("es-AR", "2e62c974c38286f666d7374369325f31b1a0547f963f6e498bccb49a427302bfcba3cf92cbbe67d71855d269d8a039537e494f74c4f3307978e03fb3c2cb047b");
            result.Add("es-CL", "629bca7b5e69538fe360d4109d47455317429ded84769b31a32ba29d38f92eed017b7d240ad7a78bc09bf5d8e75444f5da7c9b13af75dbccf93af1435ee32488");
            result.Add("es-ES", "6683e5c5b3a0151bba1aa56d207b8112ad5ab8cfe855e73586fbcb368a6d45cd5f988a0733e4fe7799dd362348060220f8339640e5d13cc650709e795fc40a80");
            result.Add("es-MX", "9f89721c66a99e787a5e2593c7cfbe283a2db3e31c5b1605da3f0fbd6ccc115d4071291fc0e69766284a09a5411017b26cdd6baffcf745e5d75013d4afd6efaa");
            result.Add("et", "3f3126b61d4e325964c112d2c0bfbbde7ec59a0a3934a841c6abcbade970d0563272da65b5ef48d8b3dd1e9fdcd67764d89ccc660355979d77da6b39fc119d0b");
            result.Add("eu", "718f18660ab3c1c8e4ebe92e9c6b6ea37c3a63712e6627834580cc632eb6ca168015c51c585ee46d2f6d1e5b91dfdf4cce650cbdb28d37b2fe40742f0697fd42");
            result.Add("fa", "b3f80329c92785574ac9d9381a13fa094d11b0b820cc2918e11d578662b2ffe81b4d2c655123a181ae32ffe758f305b2096400be8ba3f91ae0dd31bffec442d4");
            result.Add("ff", "3e55ab87b76dd074be347ffc3a12cfe0bfd3277544cbdb09faaa64c34d9a4b2c83a37d5efc41736553f1de73b07f748a659eb4c948dd637491213bc385143bf4");
            result.Add("fi", "7fe103bda96d4bdea469b4a1bbe2e9fbbf1e69e8344cfcf9612d8db0b00221737102538589d49cba699a4dfac0698322f3887d1f39efcb664d6a92767036f6fe");
            result.Add("fr", "961dc441f084c7abfbf1676f4f1cd553f0382a56eb014cf770d3eec475c3a4a8ee44e145c0c1644091ca2a5111fd37f98c8a76521cd7f6fa92882558147a63bf");
            result.Add("fy-NL", "d08e13a4838219c95534eb58db4974a06c1a612829a814f656dc1babacb1c059f9fb1286f64e0c6b8ccee5244680958a4f9ffac2941d6ad13933a4bd1f74ec5b");
            result.Add("ga-IE", "2251625d419ad3119ba63db3af0664df3c8a3dbd22fe7c46b49810f843cfe0e3a57e091c540a2194a513056febaf2fe9e4ff9e8822f2513efedfc684c0365eda");
            result.Add("gd", "26c61ae4498e5fecb8fdc4546ad2f225f3775794866e0103ac6ea3d996d29f4ac6c5f64248f485535f3bc765706c560dfebdf4ebfc71d7f36fb672c833e56eb9");
            result.Add("gl", "fa7dbfe9d0751783393e9bfc183b60f96659e4741b7058997a744d42680fa21989811855c932faf2f3d305b229a6c17a0b3077cc38972ccfe56c3560a8012d74");
            result.Add("gn", "2716d6b8c7ef370abf29d110b73e53150181ee648d4b2850c42ae8a579cd7dfcf254f3e4d76ed8fd6885d9d195f8a1152e13956c62ad7d37953956aeb0a9b3de");
            result.Add("gu-IN", "94d45c6ba7214dd9a744302f35b2bcfa27c8d155200b9973da60ce135bf1f0275c9945d65a86c4f0c8ab55591ef2faaaf88bf2484f77a268a14e6b71a8b49252");
            result.Add("he", "67f0143f0735df0510ea43341cab9a4503abc10ffb8587d15e0bf6bbf4041528460fcb8d8ae882c472b2a927e5f87472ff81ab81e8f0622235aabc2674e56fb8");
            result.Add("hi-IN", "d02dcfa50da2583051108a8af2369e16a6e138ef2f19fdd14ccc24433e652c99696ed93582c621679305a2573bb041d350d99bf87bf6095be3a493a069976e04");
            result.Add("hr", "c68c4a2602dc18f5d25d1cd42551bf11fa820101059d69621dbfc1135b05d078ad03360752296475e71113358273008b1108a164e09f72439b94e9ac9be09e0b");
            result.Add("hsb", "33aa24023c198bd25ab5fa308f8f9a7b3ba80ae938df352d0a9065e5280b1d15c293eb4420414656bb557e2b8de356154f03db1a2de7b15b4003e252c9d08d58");
            result.Add("hu", "3da1daf3260c16c8bc47c03940d9bebfb654b4509ee41ce396d83c61f8c17e22434abae3879c9179729ccb0fbb80a7d5aa2021183ae2fcfd834da6ffe28206c2");
            result.Add("hy-AM", "659de5fe5da72f7ddcfbf0005362ec980ab92d79b58adfdd30b5b0edbde50d4390964c6dcf09b27638478f40c53989dedcdd99735a11dcac744df1d6e09942db");
            result.Add("ia", "8b24bc2ce43d955682ede67824b7549420b76a1c77cf7c8568653fe81b77f63d3ed2ca07c6954b5acd21b16b2de85bca6248a26ba6dfdc84d5ef74ec01f421d6");
            result.Add("id", "cd9b7fc1d6601ed415461676250a0ffe6ebe2f04743c6dd752453089fab782a47360d749306861946975d790faa5330752cab493ac756ce08745351821d38e19");
            result.Add("is", "3171532b4c4476e932395809713fee5293be49bfdd22f1f00b76dae5e8d59f3f769ac1a77c6773f9f8f352124f06f481e8a9d17edc7c798a7d011808e1f25c2a");
            result.Add("it", "9b9522b24f9bab3544066d81f0d12a59d19351958a42490f68c56343beb2bddd652cd213ec2fb972776077021eaa47c2c9a054819ec4b6095a32991559ec6b64");
            result.Add("ja", "b386a53f569cb1556b14c65726df8422b5d638eb48547ed77dfda3ee313c26b80d5176e47b2374979852bf6f44954b6b00bb4ca44afacb75f409c2982178ed38");
            result.Add("ka", "cc775de080b218453146e35fbb6cb776358944439c36e7966260d06355ace440d0fe5f63ce726797e3da349ee537a0cf0e33eb599087bf337b9ea504404eed4f");
            result.Add("kab", "712dd2beb24cccfd775779f4a395af50377c7c561344dae1a366ed9b486ed311763a8eed2451a6e685a9bcb4ed07a06f3d9ddbeaf61065530cca254b3c379e6c");
            result.Add("kk", "e0f40e30fb8d9796c1d29e60c92d1ba446cd95dbf29e0f5f47e470558a2bc24beb92cb4ce96165488dace9549d3d599fd480306b632076014a955844aceb6b9e");
            result.Add("km", "e6186c3f57becc7d2b4088c7dd902929744f2f58ca712a77704a6ea5c65184ce35a885c92ae4519d1facf3d4d477da759664ea55a7e6abe9f3e740126240ed6a");
            result.Add("kn", "e895d693062ebbeb2efdece171dd25a6f549ef846fc22d3bc04f4dfb8f128a44963df3092292f2115beebc1d4ac5499f685e8a2844248fe3acd8e0387be8a601");
            result.Add("ko", "0892d1386bbdf762a244d45f34c2a09819ce7892399a4bd83e28cae6998a1da8e0972d182935f6f25ffe320aee368de88b602d26b279aa9c901607d07fab8063");
            result.Add("lij", "895b37fa33d30fa677e0f6720fe5946f7966cb4e7bfab9240d7761d9581342d9e3cb0d8719c4f3ef2ad1db4043db52a78b06686bfe359c88c113e4302e0c21ad");
            result.Add("lt", "5a15b9ea2b47d80505bcc88d6a2c58ceccdf56bd652a38118f738cc42cfc78ab064576af3e0ba1316152341995af2da91338f593f92093832adc33396661e99b");
            result.Add("lv", "58d1e996a191f78442dab8978929f626b0447b40085c350a24e5374de84b9a05fdd5fbdfd16fa722a0b815f7761f3805a627b7531d2bb26e82270dade30b1603");
            result.Add("mk", "68959137a79d6ca713772e1b7f800543a49c0de45b78478d4f5b33079f01445461b87d2cca1074eec3a93c169b48a0400ad378507448fda0048f1e5d0033ae46");
            result.Add("mr", "cdede706d25890b722e8b4fc05dffdb784335a59916a35a6be6ebc75382539842bf83e2217ff2eeedee94af21b335fee42fba550c202c6145ef46721c1518ff7");
            result.Add("ms", "8d0dbbdc563d0b1298c289049ad287f2c78d17560b4610f430bebafc814ccacf2d8e406f2950ff67082a6ab6805d33a1c4be0c542ba7ca50678956cf80cb376e");
            result.Add("my", "b0f5e033f6d06b2be39cfa80b6f76fd0858ed5903afcd04194588185fbf2c61611a41c83bc4eb3860d43b63e194d2586182649bc447a936c8c716b729f7022bd");
            result.Add("nb-NO", "fc4a94913f3ead027698e2c169d0e651afbe282a5ccb3b55849dbda2b04c96edabe7a08632f2c79b256abbce00537c61d3e506aaa6a522b0cd906cc67cdf674f");
            result.Add("ne-NP", "b5f5b39b1972b7ef579587b7b570991a999723ae703f53fce69568ee100414be17c5cc22907b4c9d62e2bd593117d05fadc00eb7b27443eb8660344de6ff56bd");
            result.Add("nl", "2c100e7bf850ade378f3072eb7f282a94d7328aebc86a5469ae36d230e77048b4946c9fe9c70fa273e4b5c6898ab4dbe1254ea583745048877e5465cfafedc6c");
            result.Add("nn-NO", "9701b19c81ecd58ae1af769e16d3d975a5a2656c6c67c7ed6da275c268f9d19efb99766b73a045eb38f2d0f2588a4b1c69287f1217c40aa0fc5c6e6c22fc7c96");
            result.Add("oc", "581a7db9daf35155906ae766e2699b56c3f834a4b342d3ccba69660480fc19ea093f2029f7de7510e659168d53f32101ccad140a7476d15e101ec07a9d52c8b7");
            result.Add("pa-IN", "2810a6adcfaa59ea41610f80d756948448f502e0e8965cc689718ab34efc672dad345d1993e889227810de17a4719de50c6ef179dcceec0777cfbb432d379a06");
            result.Add("pl", "64846664759785578872fe4686a525d122cd90aa6ef809321cb74679fd2557d4c7f508b059d56b401894aa015c5fadbed102bac0ff3c5b55e98d23e4aa618e9d");
            result.Add("pt-BR", "0a55688501dc426683fc61512224ef21d239383db51ba57f4a509c4c9c3212e409984f8302115f57693391e3d399ae435ec97184bc5123aa8d0e63e42ae2aaaa");
            result.Add("pt-PT", "f8a6d168963c8c74cdb9c00d8691fe87c7394915613ed10ee6f561c0da6b85bb592b6c1bae1fc2c52ab290c6a0c8f2a846f3399c4d696ef8f39710c387f8364f");
            result.Add("rm", "6f407262192472aefc985a357caba742747078a76b236db20131ea8c2943dbeb090423740d81b36750dc057018bde8d03ba43b5898e30d00bc70ceba6b5bd820");
            result.Add("ro", "955d8a515eb2db6cab34181442242a474db9316062b9b880de086e6f6175422dbd58203c013c4ab37bc0b73887c655ca8dae71dfc44ca7cef146540aa774aabf");
            result.Add("ru", "cdac19785ccf9a7ff31b0dcfb64a2a6e6e8dcc9ced98cb27af1aa7b4fc8ae5b7e44a79aab3195f4e0427d90761c916ae68115371893eaf32514b31e18fa48ac2");
            result.Add("si", "a5dab57d4e760e9169b18123cd795e7dd4f3a7b4c6239562071710f6f4b54add2037fbc81362c252ed8a6f67dc2067efa137f69b9b11c9d61020a795df94b22c");
            result.Add("sk", "28e3c733598e63e523de48cf047a49dd9524614e078dcff9e69776e58bd685947cad42c5e715db03950df2b0a237a0b2f13ad7102af59a86ea601b2658e70d4c");
            result.Add("sl", "9e27560afe44be9c4803570696039b90bffd427daeebc06650493b9c7c5699e5f398902db49629fa5134b7b21244a63f2f0703246d3ca110ef1b9237c1e16cc5");
            result.Add("son", "a7563473b50559ed0b65184a2d22efd329a10439857262bcfe8cca6dc8859f64cbce4ac0f2f9069682c2f0ba1a22e53a53be013716a2d3aada53e8e6a4fd5b6f");
            result.Add("sq", "f339b3b94dd7b86aea8bff6f829c56a712401d2214f93977decb37c8763d586ee26aa4f1a60d06553ac7d91f95550b037c304f284b0f9281eb62ae4ca4fd7c0f");
            result.Add("sr", "e36a999b3ec91f2c61380fdbc00aff9ddea0a150420a1938adf1b78f1a86ef24b20ebc8ffd6c814558c7290a5d2dcaf4825053e292a0b62468ba9d5cc6f753b2");
            result.Add("sv-SE", "3f1c254860c020efa9723dcd402b4d1123657b08c0aa7a1e3cc83a9c7cb90d07947791a3b05ca2f09046fbb4f8fe6061e5faa5bd1279c5b76a34eca02fcdaf9e");
            result.Add("ta", "d088f93a10d59cb2a6e05dada085c1797992c9c22cc943b56afb54992dc9bb7e9c976f1ffce753619cf9caf7eeb1fdbf5563b6127968ad1bb345d67d2e83e6b2");
            result.Add("te", "53bc9e8be238b6be0d72e45f710e48bd4fe6917823f3ac62c5e33f06c3fa23c75b321dcffbd0cb6563542b815b536129870220079865c1cf03f6e59aba1dddc9");
            result.Add("th", "bf5b1999bb60b55fbbb285dcc634c73f4b11968c58e67547f259d271a66facb36308a86d705226e1cfa431f90107ab1ba7e432196adc802a65c5c54d1fc10464");
            result.Add("tl", "8fcf94f0a9e965cca0240d3a7a365a79a840d0c5bf55a6e32221ef46585faa0030cf07bfbd12fd5bb5073e2e8fd761fc7984a13b6b7cd693e8a5b9b899cdd463");
            result.Add("tr", "e52de0c7498ef47c2a33753358e102761e73c1f677f8493ff76fb0d14c4d6c088c13002d3e46b9ccfb8ed212a46a0a6a5d9d1faf1b61c3e365670a3b5b148f3d");
            result.Add("trs", "20fec01469b667ff30cf92a3adc31f73d9d9afed8659a4e9869bcb5535c00dbd0d8c3272d1979d3a12fd9d95d52c8cb2eb1bda964a1fdd53a0201d5896b14ff1");
            result.Add("uk", "98687b1bde1ed29139d0e81a2dc8d69188de62fe7c7ddae84fa4a478b56fdbd3e696822d07549df5621f32fb41def41b10761d5edfd983b6dc0fbca0b392e1ab");
            result.Add("ur", "b6555ecaf737aa53bfe04f0c6d5936d7b114d34cc9e2e7fe5420ab9c4482156e05dababa0bb5e02051b507e7a5f4245e75e89c0b10ca71b910dfe131663690f3");
            result.Add("uz", "4d85f12ba93ca315fefa0582a02d120730d685b6e4437a6606924d67ae8670c44b4622ff47b65e62759b7e5561d7e0b410234c15b8db633cd9115e26e0ebbe69");
            result.Add("vi", "43771be1cb4ce6863f10101da14c60a4958f6cab1f529312106c7442bf3dc25bf983f76a5b099772bd87df09b2943eb8e78a9afca0e79a6402fff69b7030aee4");
            result.Add("xh", "884a07c963dc00b394f6829fee83253c440c7debc8f517a35f1bf35f1dd92f76e736406212c94f2f8f4b21c416b0e5e7e77422b9aa2283a2f9a0bcf6d0272bf7");
            result.Add("zh-CN", "9d0c9092ca44e275b046b3c196b9ec63a2d60d3f01b050b026b49d7f1e03c8b532424266eb45defb4f2034c7c6da47cacbd38db60f40f99501fd6d2d2a7856f1");
            result.Add("zh-TW", "9c82fbe781f7372f7fe97b8b89a0ced71e8c64f15909273d7ee6093c12d82178be5d278ef3bd02a4a69ef4c10a64aee3530001812852f9ba888cbc3098f03201");

            return result;
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            return knownChecksums32Bit().Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            return new AvailableSoftware("Firefox Developer Edition (" + languageCode + ")",
                currentVersion,
                "^Firefox Developer Edition [0-9]{2}\\.[0-9]([a-z][0-9])? \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Firefox Developer Edition [0-9]{2}\\.[0-9]([a-z][0-9])? \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    null,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win64/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    null,
                    "-ms -ma")
                    );
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "firefox-aurora", "firefox-aurora-" + languageCode.ToLower() };
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox Developer Edition.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://ftp.mozilla.org/pub/devedition/releases/";

            string htmlContent = null;
            using (var client = new WebClient())
            {
                try
                {
                    htmlContent = client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    logger.Warn("Error while looking for newer Firefox Developer Edition version: " + ex.Message);
                    return null;
                }
                client.Dispose();
            } // using

            // HTML source contains something like "<a href="/pub/devedition/releases/54.0b11/">54.0b11/</a>"
            // for every version. We just collect them all and look for the newest version.
            List<QuartetAurora> versions = new List<QuartetAurora>();
            Regex regEx = new Regex("<a href=\"/pub/devedition/releases/([0-9]+\\.[0-9]+[a-z][0-9]+)/\">([0-9]+\\.[0-9]+[a-z][0-9]+)/</a>");
            MatchCollection matches = regEx.Matches(htmlContent);
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    versions.Add(new QuartetAurora(match.Groups[1].Value));
                }
            } // foreach
            versions.Sort();
            if (versions.Count > 0)
            {
                return versions[versions.Count - 1].full();
            }
            else
                return null;
        }


        /// <summary>
        /// Tries to get the checksums of the newer version.
        /// </summary>
        /// <returns>Returns a string array containing the checksums for 32 bit an 64 bit (in that order), if successfull.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/devedition/releases/60.0b9/SHA512SUMS
             * Common lines look like
             * "7d2caf5e18....2aa76f2  win64/en-GB/Firefox Setup 60.0b9.exe"
             */

            logger.Debug("Determining newest checksums of Firefox Developer Edition (" + languageCode + ")...");
            string sha512SumsContent = null;
            if (!string.IsNullOrWhiteSpace(checksumsText) && (newerVersion == currentVersion))
            {
                // Use text from earlier request.
                sha512SumsContent = checksumsText;
            }
            else
            {
                // Get file content from Mozilla server.
                string url = "https://ftp.mozilla.org/pub/devedition/releases/" + newerVersion + "/SHA512SUMS";
                using (var client = new WebClient())
                {
                    try
                    {
                        sha512SumsContent = client.DownloadString(url);
                        if (newerVersion == currentVersion)
                        {
                            checksumsText = sha512SumsContent;
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Warn("Exception occurred while checking for newer"
                            + " version of Firefox Developer Edition (" + languageCode + "): " + ex.Message);
                        return null;
                    }
                    client.Dispose();
                } // using
            } // else
            if (newerVersion == currentVersion)
            {
                if (cs64 == null || cs32 == null)
                {
                    fillChecksumDictionaries();
                }
                if (cs64 != null && cs32 != null && cs32.ContainsKey(languageCode) && cs64.ContainsKey(languageCode))
                {
                    return new string[2] { cs32[languageCode], cs64[languageCode] };
                }
            }
            var sums = new List<string>();
            foreach (var bits in new string[] { "32", "64" })
            {
                // look for line with the correct data
                Regex reChecksum = new Regex("[0-9a-f]{128}  win" + bits + "/" + languageCode.Replace("-", "\\-")
                    + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
                Match matchChecksum = reChecksum.Match(sha512SumsContent);
                if (!matchChecksum.Success)
                    return null;
                // checksum is the first 128 characters of the match
                sums.Add(matchChecksum.Value.Substring(0, 128));
            } // foreach
            // return list as array
            return sums.ToArray();
        }


        /// <summary>
        /// Takes the plain text from the checksum file (if already present) and extracts checksums from that file into a dictionary.
        /// </summary>
        private void fillChecksumDictionaries()
        {
            if (!string.IsNullOrWhiteSpace(checksumsText))
            {
                if ((null == cs32) || (cs32.Count == 0))
                {
                    // look for lines with language code and version for 32 bit
                    Regex reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs32 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum32Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value.Substring(136).Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value.Substring(0, 128));
                    } //for
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    //look for line with the correct language code and version for 64 bit
                    Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum64Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value.Substring(136).Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs64.Add(language, matches[i].Value.Substring(0, 128));
                    } //for
                }
            }
        }

        /// <summary>
        /// Determines whether or not the method searchForNewer() is implemented.
        /// </summary>
        /// <returns>Returns true, if searchForNewer() is implemented for that
        /// class. Returns false, if not. Calling searchForNewer() may throw an
        /// exception in the later case.</returns>
        public override bool implementsSearchForNewer()
        {
            return true;
        }


        /// <summary>
        /// Looks for newer versions of the software than the currently known version.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the information
        /// that was retrieved from the net.</returns>
        public override AvailableSoftware searchForNewer()
        {
            logger.Debug("Searching for newer version of Firefox Developer Edition (" + languageCode + ")...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            // If versions match, we can return the current information.
            var currentInfo = knownInfo();
            if (newerVersion == currentInfo.newestVersion)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if ((null == newerChecksums) || (newerChecksums.Length != 2)
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
                // fallback to known information
                return null;
            // replace all stuff
            string oldVersion = currentInfo.newestVersion;
            currentInfo.newestVersion = newerVersion;
            currentInfo.install32Bit.downloadUrl = currentInfo.install32Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install32Bit.checksum = newerChecksums[0];
            currentInfo.install64Bit.downloadUrl = currentInfo.install64Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install64Bit.checksum = newerChecksums[1];
            return currentInfo;
        }


        /// <summary>
        /// Lists names of processes that might block an update, e.g. because
        /// the application cannot be update while it is running.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a list of process names that block the upgrade.</returns>
        public override List<string> blockerProcesses(DetectedSoftware detected)
        {
            return new List<string>();
        }


        /// <summary>
        /// language code for the Firefox Developer Edition version
        /// </summary>
        private string languageCode;


        /// <summary>
        /// checksum for the 32 bit installer
        /// </summary>
        private string checksum32Bit;


        /// <summary>
        /// checksum for the 64 bit installer
        /// </summary>
        private string checksum64Bit;


        /// <summary>
        /// static variable that contains the text from the checksums file
        /// </summary>
        private static string checksumsText = null;

        /// <summary>
        /// dictionary of known checksums for 32 bit versions (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs32 = null;

        /// <summary>
        /// dictionary of known checksums for 64 bit version (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs64 = null;
    } // class
} // namespace
