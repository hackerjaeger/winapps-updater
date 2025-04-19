﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017 - 2025  Dirk Stolle

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
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(FirefoxAurora).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox Aurora
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=San Francisco, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2027, 6, 18, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "138.0b9";


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox Developer Edition software,
        /// e.g. "de" for German, "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public FirefoxAurora(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var validCodes = validLanguageCodes();
            if (!validCodes.Contains(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = knownChecksums32Bit()[langCode];
            checksum64Bit = knownChecksums64Bit()[langCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/138.0b9/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "2348701b322d06a2d1669218fcc1e58d398583a52c647150cd0731b5e74c9b23f6482d2ae8bc6e7f9d393624c97c8099b6f55b339a89fe10267e519c925b5d6c" },
                { "af", "55a34758540a53c73e998d605a6e57972df37c9cec864cf942ff8478a3d35c1319b47cad9b4411ea17954d713b65549fa3925070ce3aced6d290c26b93c3469d" },
                { "an", "f98688e204cf13ffb556d230d426272b56e0ec866b14735c709961e521ab69a9538aa82a43840c78fe75679fcb4ea8611cdd4905eff7c674b1476ebd4164ef9c" },
                { "ar", "243d9b34778e0f9985b26ede293f1d5b3ce9fcaa015f43072999b17102aac873a5faaa93a4c4f2e50d853878d5e53835f1960b6749e725e3bfd034f339e2b11c" },
                { "ast", "0f51e8edbea76b84f820af7644740f03ae7aa91343526df42562553b9415edb7fc820f39d528b26095c8cdec324535ce2dd620415411ad17a5ad1e9756334367" },
                { "az", "6e81004c6ff0319bdc1f314ad0a157b6fb630e10be2b89868d2ef20321237b66745c17214ce8a5a89cedeac304206e93af84ba4da1071e917783bcf13b9e3856" },
                { "be", "85cf4e6618eaec7231f9e5a13e89edcbeba4a78f7fcac65ed4bc4eea3411f3d658ea15e3e50e5ed5729a8d959aa711d7f2ae63918918fc57e279ef3fd44be232" },
                { "bg", "e3dcac9e88ab441d935fb477f9051469d21149eace920c58ec24cdd23e4220de205b44d41144611b4c6b23b78a1baccdf9a19486d3d7d176ebf0d0792cd4649f" },
                { "bn", "c96dadb61e1377a9bf9035f1838fcaed3a096f8f236da8ae2ba5c9da9fe6c4814c1cf2a4e364bc6014f6d0f4597f7c89bac1e97fcff72f152d904763832e2a87" },
                { "br", "340e031e51ee1e554de34a7047da0cee6dc09d636f85a6e20c912d2c92f036c05b2821a270206a3beab5c025ec298fb75f84a6938a563412d522008979366458" },
                { "bs", "141c72bc3e1fe2fb31fb997ccdc8574a95cf783fe6e9d8c0c6dc902a2f50277d20c1ccd9530f22066cd02a12989333ffedc210e9544e986f4918af6e73bc9fd7" },
                { "ca", "e44abecbb792db698c60c95242ddfd8adc3716eb71f974df25921009422e84f492be9e94faaff65d074b6f3428eefbc24b9516bc529abaa2d4f01f03737a85bc" },
                { "cak", "341061af62827f711b785e5735252808afc76acb2c8fd534f79a5479b5175b7ce1a7b6f9e085e44d3fe1c752501883fcb2ae6ba880136a00096e68a6cbe1da32" },
                { "cs", "d0c4d1adb7b2261b405bb21e0b528c09997352a9bfa357aa5b7ec56b8c223b8564342a1f96103820383d89a08249705ea8049cc6190b4ea7baf340814f49c90e" },
                { "cy", "d5b75542428ab591abfbf2ebea01e2474f0d316bc1ab84483b41bf73305525ffa9d5481b2533e93cb7f556b9efda193427726178fd183c705d81fdb31f272789" },
                { "da", "77cbad857bee9cb7ce794e6ecd8962df7fd0882ba4042add800ca54809af4b3124c94b006790c4396fa4595bc18ce8b150a20536944cf5305686cd0c6e376bb8" },
                { "de", "78a8bd4ab280af05fe6babfb8c30515ba7516e6e891ed310c7246f5086049ef9192d0757fe2c5fd033e044d4cc4b91fe807d90f8ab2df15885cae1543eabdf29" },
                { "dsb", "e6215cb5a5664278710d4c5b099b1314a66e0cefde6a31c28e253cc67d6dff53f0a9b30c6932ff8343109d1e58266b2af4bf636a9c4f379bc31c69d9625bca7b" },
                { "el", "00123c663b95ab1032cb3e1db69284222de2284c0360364544f2c383085a36e84ac0cb3b4dd507fe57b637fcc864d4dc5fdd1260c82949569346aad29c20a0d9" },
                { "en-CA", "c2045b1af5eb9036db14491fc95c1a9ee67b90b7550810af3ec43ea72d829498c2e686fc4117f51b39d1d788853dfd553aa32ee631f3eecd3acd97a5803cad44" },
                { "en-GB", "ae3787bd4ea09819c0595f63b5a25236f9f87c2f8df90682a967cb53ea4a49dba6fadd1932bcf4f7cee1aeb728846b98149caa2721a4c6abde7f7ff8d23e85a6" },
                { "en-US", "dc0d93c8a9f62b845ee1a6473b0f68dd6886474d39dc0d8eb600b7b183b7a42393911616f139d13f68bcc6605a33c9be315ef9e33bf601891f6a0f0874ddbf1c" },
                { "eo", "5f1878b06d933df60af37e431592fc6b43377a3583d190bbec0a9cfefdb561ec4e9e203d04002813eb3406bc2f06cb8ca1270e6a39bed1ade7d4728033dfa640" },
                { "es-AR", "a2cb79ab9093a97094f914b42429378bda1e09a2db777e8ea71c1a26b8f229a5f84741240549bea4337df8d55e1bea9adf30d6ed55d791f5f1afa1866b4537d7" },
                { "es-CL", "7908fdf3f212315e8226bd36bdd69e25e4d80d243fe2593cb16b492bec84f108002bd8320e434866c8bd608354ca6740426d1a6522cbf8cbff072bed4a864156" },
                { "es-ES", "206f0fb724d14ae1664d6d83096ac8cffeb82a5536efce27dd118ffb734dfee7f652678a66750d43403fae97f1965811784050ebcd76abc78ab0cac99f5d8970" },
                { "es-MX", "d3295a309fb138627ca71985e6daa2e4d61e7fc21cc505d02ab71bfba46435b726d711eba2f8697892a22ab609198f74cfce25ac5c53e97556b4233d5c7fa155" },
                { "et", "6bd24cd92017d7e74ac540f4558c38cba3143c4a1f69fc37990f0ae96bc6711945cb9be85325fd9a59f55902b4035888b5c4eb5560397a25c0e1158ce1f4c717" },
                { "eu", "79f22b2652ab96f2d242fccd01a904a8ddcb97e204507a5e33e1b28aa969c11d67f01e3b6705374d219f1eaf47c8bb7a15f0f0550bae61aea543b234c9b5a825" },
                { "fa", "22ec1f3e69188beb5a0f4fbf8effa0971e96817a1b4c444373784287076c03b85b1359fe5f8a1163d575853118a37fcc75630b3f35ca27108aa7fefc7e917d1b" },
                { "ff", "a2669c5c3ad9c3f48e0feb86bc158b1347949e65b8dcbaa830461ef7a764ac85187f8625c2c75280fd861592af15750fc01c45f7f7d9d4d96b38f5cc69c25468" },
                { "fi", "3d1392c45e3b5c4b45369f5a6042498e21ef2650c0417318f83ff64e88a8fe801bfecb0ef36951c21f23f3f82f9e0c7f3dc21985e083a7ba0a6f8f185d1cbf5c" },
                { "fr", "7db2f1390db97fb5dfd1909e274ca10163f7a302abc156c40e5a6c13fdfe61e62b29b54f1f100f42f3b06478c98bb342ea51563fd6a9741b81094c87b39d7978" },
                { "fur", "553607085b09eeb8adef1adb608053b0db33ece520edc1b4a228b9d3eeeb2cf6bf468fe0624ef5cfe1de9acd102618ab96fbfbf562ec96dc266c1798e95c7a4e" },
                { "fy-NL", "31106a14425bbd2b3a05fb4db1c053d9a8dcc767cef908aa0dcb3116d3a5c5d592142d000df1af25849d37b3ee615de62899cc4cad8103ce612e2caf94513732" },
                { "ga-IE", "955a9947e1b6613575a41753c796d2c13fbf1486718c10a651cb75128931e13d5184c070c0735dbd7b8cbc585eacd0d0651026589ea229aea99428c1d2f5ffd5" },
                { "gd", "7e15b2a52323163fd15f78ab44c281bb9aa21559803face9a20579ff9e96d4cf9fae7bb6019a29feb3b2f1cc0e23fb285f312286adeb9a31107178ea00ea4209" },
                { "gl", "5a7709fb8c6ca9b08420c4df31d957cedc38fd7849fb0a914ecaf4d78aeb9ef85450c8b7817bd1423e54f208f6b349303c509b4f3d315b7ea5c28e7e610513f7" },
                { "gn", "12431cfe912e26d72a99853b6828df10a419668f92ff3fe6551c786e4d469f2cae89d11764341a9b4afdbcd87bd394bdcc6d095d6f60227316e2f16ac1c9b0cc" },
                { "gu-IN", "b11fbdf8d5dc8939ea76d04c75e6abae63b636882f4c6601ea545a7713f68317177549503151c9a10d72e3e27fcc79b3a1e21a645310f4f39978f54e4d68c9e5" },
                { "he", "f117c7704b3c917aa6784892ba16acc8a378fdb9201cff701aa29dd895e310502de309741d236e90837af1f2486ac5cbf6b16d606f2cf1ee2b6212f1402fab13" },
                { "hi-IN", "8ffb8bd259390c4481b02b31b538001fec87bc9591dbb3ca1177ea943b7769994e9f7d8f692a8aa75f341c1994bbd7544e71492b08bb98b8f16a5c27d63839e1" },
                { "hr", "b5eb37c20002debe7761cc946f1ce7e6edd20d92e4c0b37db9578b04fcca7dbee74f5d12d337949ea5f75884f5364eeabca7bf1102946dcc9e575bb1ffba744d" },
                { "hsb", "eadac9a7a14191b7576c45136bd0d8386dc699fe9e5a849f3de03ab40e475529bad7ed65be381269f6042dc3d5e24d958c6567a7dd8da080bbb251d7dd95a6f7" },
                { "hu", "8ee6926633fad882206a3f0a3dde87d08b810c8a45e662a32b8670e22d3b372449f33cde4b2dcaddd8037ef2a0ad9c6f7d92d84683c4a1ba5d06e332ea2e02fc" },
                { "hy-AM", "45141d5ff649cf715124e879f707ca9801b00894ae48c0b824f257cc4e125df83b421a2b4ae05c78bf75d7bbacb82c81aea28369e9fcd839f34752205291c45a" },
                { "ia", "42fd97f26613c602630bd205bc3d6df66431a6454736121ccdbc10a173f22dee98ecc3fb37d6b0b46373f54bc434bb1e24a3e89a85ff3f951b06146de5af6f87" },
                { "id", "96c6b56b8ce2bfdabf95f5b534b61b718f8931ff751cef29bc3449b0449c5ccbbab93c11b45573fdc84556c42c0da7c1327a4ce9f6ca2d41fde35d53fa53f2d0" },
                { "is", "86d0f9991134e9185f284802e59614efc9eb3e0c3e9e9249cd8b091103daacf435e9065ad511bbceb174587519418bb17a016f62f9ce955b695e5a129af8cd1b" },
                { "it", "1c4704564c6a3461521be1cdd2d7082c55a489fd012533bfbfd484dc29984d0597ccea98bf0339fbb98beae1ac7813a86835f7a70e545b783cf739604ab80a0c" },
                { "ja", "cb2cff7a7e0dc5d8096d3ed9830b420a9f61bdd5f117eecc4c037faae85de2b5825954e0481a885c33ad424599b3a000f8d8648c06785ed92424f7ffb0180ee9" },
                { "ka", "4838c855d62c00815d3d8ab00536ff000b4e181977328cac613d6216fcb3653b21143966112dd7c3abafce3f5b1f2b8e0490f9abb67c5b50050261cf4b5fd754" },
                { "kab", "8d9abe7ba71c893b0b57668d3d12fcc5bb3b9f1070561678e53d7d3a4742ae12b4a747780d7da41a02ab367bca157a9a7ad9acbec09fa42beca1c6670cade14e" },
                { "kk", "af28cab40076648fbb1961938fb1c2a27605860669055279132807c501969253deb8cabe824826eb559f2f678c58d8a1d3c080710285df2751db3e3e04d887a0" },
                { "km", "2a4dbc54f687e1a028a3d43a63ab34d67d7bff2e0be14afb279bfa03a0b646b32c8eb9fe2d979c2a673bc65342283fdb0e1dae8e577b0a510daae1466214f829" },
                { "kn", "94915d15e552d75a30e6bf2c33e0ddd0674f8d7182fdb00aada94f17b095d6e036ef89fe2aa22c7127795626ab485f86418d94e28613f0433b67867fbac5d82a" },
                { "ko", "706338a5e82cc496b6d6d9727f1bd387d76f55dd6c0815c096f6fc586997e738c1c4beef3657a029540720b5a94555a84d5d6b713b678dcc88b2c4dbe8d6f9e1" },
                { "lij", "bf36579fbdfde1e037671d33ff3ae302022b55d69cc151148b73ad6f824100aee76f388ff8d27579eeb47fb93d1058a69bdd3f5dca44ab02f8d4e686459affc6" },
                { "lt", "2e2a9ec97a192c8271d9518e097fe17f53377f9b0570aaef5e8c0ff8e7227a62b99d29e64f7946c47080240f775964f4136b43b868d2a078ae706328bc1bd9af" },
                { "lv", "634e1ac55c08cbfad64d25b171a1f6f6b9e5dec62bb0e2497f0a548e63081b6053a81bd6a2e965f205aa8e14efbe409e73091682a57c828ef205d4e41c2de9c7" },
                { "mk", "1841071f068f2c8b384e1c7cf3a275d4bd04cba8e056a821f60227fd4960e03500e8fa00514025d703d92a6269febd764989d246039a9575b1b7eb25c8298111" },
                { "mr", "ac478153108ebbdaf0a00a82dd91bfa1c68d63366635a6663a5553e80deacc8b7fc78fff0103a68a153b9b36e02e817a60de4ec21271eedc8dfea57145c9687d" },
                { "ms", "81d90e3914bf2dc08d72c7000cf3e00085fa7337681d3d99e1a59855c6f502b60a0f6cf3c7bcb3701da47cfa67a2ea41814bbd2aa3f8f7b2108f374ae8c420ea" },
                { "my", "5e32749db6bc8c78dacc7dc3d6d716a2a43bbdab89fcf984ec87732b16b50111c875f268ce18307857ee2f7f2471d76d63148c2c1f3349a654a7116ddfe75c20" },
                { "nb-NO", "8d166d10d7c1b461820992510fe69db530c524f6d4c8fe151f0cbf1caa0368ed1d501780e535112d9d48629168437f5e0fa126462b4be61ac3cff8e1c9d7f3a1" },
                { "ne-NP", "2b127e443b5aa705bc18f76c9d6a084ae300fb01528b597e194894b1d09e4e8681f3a0b900ad08e8a84d4c61f69dc772f36602a3d21ac6c0e75cb42200167e2b" },
                { "nl", "3ece14e2e7413e2e8d7a7b47fe68c1c8bc5866769697eadff1e9ba4dded6e9841deb8ff8be144acb739983b1e073e6750651786549e6b9f7be3ee3cfea13ddf0" },
                { "nn-NO", "3a440414b9479dac1e624d7402b4ecd4a81a33aaacb2a8541801f0db757dcb0da047807e74b6cb135704ff7ee536ec7bcae9f2cc7f895136595b8f718a6ba261" },
                { "oc", "15637652abdbae99f2e52d5d347cac13ee17cf5a6075faa677cd539a9a5fdddf3d513972e2ffe58b8551a2c389e038c0840d694fea6f473f9376ea42da559fd9" },
                { "pa-IN", "5e7b9006f196288ac55ebfd84f96322e21fd46aa3a3ceb8d876810c049d6848ec9d2bb51c44b7a3dfd0845a708595ba08e681dbfe411a268af27d60291e146e4" },
                { "pl", "a1237f6ffe54c3b2accadac17b8c9d77b40f8558adf73dc2add24f54006cad0606aed9ef9cf84468d48ba8bd760d0f44a5ce380bb363477ce0a9b3c16c385209" },
                { "pt-BR", "71d8500cf738b5e70a0852d877c7eed5576acea0872988045143b19537f012308a8616e43f6083105eb0372e83f5b486baa43bc3035f44b0e5bd1c828a8d8fd5" },
                { "pt-PT", "e6e2216e2f777e442dddc3912521d10e4d0a0f54262b6e80970f8da49805c8a51f297a66bd2d153ad06bcd15d2083d3f18ecc31bb99a1d838365a7f62915f305" },
                { "rm", "5020536e3a4dec304390b955985215c1432959f201509ce50e76e19da577f8ccb7aeeb1417b34b55d9bbaf21e119ae490e05c1f0c47b87d4fb6a507a7b0cd36d" },
                { "ro", "ed3a42bf2b964141232567d3eba95f98ac63f840d8316c57ef1a0adf6f5e3c768024026b757fcc0f0dc3264e8ac3dc080f1982a38bba348fa8e7d1bbefa92ca4" },
                { "ru", "69d05f17100247382040a84bcd12b51f51b347f1cda6d18ae9e1573de71c027e5d99395a7b87bb32ba21b14dfb5278a4cb1dc3474d10a51c1395e468ff55a2a3" },
                { "sat", "c26e1d2be5f95a0d5339d6d6cd6504e8f96ecd3df80d252842b120b8ddc8afc73cffaf69297a2516a6e76842194e77818355fd4dc8ca2a14eae3b6be5296f37b" },
                { "sc", "1375bf6c89a41d9711b2cff8778e7d7d72fe698600d882de1ec960047370a650035ae9d212afe97585bdb384f8a603fae6d28f2736529a557d3428c1611e56f9" },
                { "sco", "9068b0e7d1d7f66217c35599ce246657c40431e18a4d25947383c8e7a0cc187530b6225b398a447081757d154c60a9b6e9127d298d4da0cc69149d472670e29e" },
                { "si", "e9ef37a59357e30ad0a3cdcce200f6805f32d90d71a16f4ec423e7e7641dbce7809c935dee704b5cea69a11496f0b85a228ff93503e4fea73b0fc3ddaa12b58b" },
                { "sk", "c0f38655ea40c7df2130edabfd2e682b9568b83f72ceed6673900fdf07153928256acc1dac582bee160f46c8057171e36c6aa235eaccf86a90cfa559b76a3182" },
                { "skr", "49246f04ea4097e85205c6dbd2b5527cec6990e0014646386901eee39f8276cc33bd9d4efa9de0d46789f6871c7435435b28ff5c42bc4aeae5b192263b7bc85c" },
                { "sl", "13256fadb3cbde22f8893e67f2e52dae70ee720269ce5aa7a693502eb4e072082bd862a684f8971292d1ca1b4819ec9ddbfe451f94cfd966902390c5ae84eaef" },
                { "son", "1a758641c09f121b83046fdbf68282a8844317e4bfe8cdf5989af9b974cab1d1cfdde1dc1a2d2fca8d1f2b9aba68fa8d21a61f1763c3b201dacdd8c604961def" },
                { "sq", "cb8be4842227718d32f58fc76d86a524f32ae4eb9c9cc58f5ee9b8c0dc3f03c5e6c724d56b186a0b5476951983441d9ea05c61c364c2ebdd8592839d0d200346" },
                { "sr", "9bdbe79e1830a906f6099308141b08a256025e8e1567fcdd7deffb768a6695cadfb2fb35aabf59673869f348d6a4fdba0640903ae50df6d5e6da87db3a34195d" },
                { "sv-SE", "191603a548db571f891a152f9260ceac5c0df0200d5bd5beea0583e7085815f52e42bd1dd4af0b865119fe2e229c963768a2c90300d615bfb5676030de828dbb" },
                { "szl", "9db17cc08e55b5a5970b9f55297d99bb68e9b4d4bcaaf947ad4144ece889e67d373589513d85d2348d7846ab03f08afa9dacd3383a9c44ae396c274b2f899c8e" },
                { "ta", "790790b8c25f1e6e5af786fe4d7bb3eff5b49409b7eaaa02f1a99522a3d6e4531a3c94133ffccc471733f3e4a487090384525d0f49759ce0a98f238eede2ec80" },
                { "te", "3e9a2c1c9f00b906c5d8a83d9485d8fe5a86e12badfc4b301d3d8d464dbc5ccd06bbb72e9e53dd321456b723c41845700edf30e0f2d9d8486785cd38247b7c4f" },
                { "tg", "0a3b38be13af39aea6f1b1a72f9d8789d454b4ecafa1227e45f5342596b012af195ea9cb15d35457b37a5458afc89ad168792227fc3477c62f4ef36e81932773" },
                { "th", "36541418bac509d81095b2c4112fc756d40a03e255a508ef3b078423abd5a5e2420d8bec9dd8f08bbe272c846501c91557a2adfea845c5226c36a3cc02a77288" },
                { "tl", "f7dd84574bd008a89ced74a85ffa474bbe4fddb6ff04f7ed6d39cd6c3ef29ba28f247446544672e1c0fa13bb63cd5fbaa3abdae4ab6a25ff7a01fd0c80ead777" },
                { "tr", "b909aaba3343b94c20caaa9f68dc9026c07819fd2db83de87c12645b8cc1ecf8999abc70c678ebfe91b47c270b6e41f35cffa76f387492614578bec5cb9f0ce7" },
                { "trs", "0945d3ac14098e00978b543e03034b8f1ca3a413366aced862121e0ab98cc01114a34d20b8cb0f85d18dbca6c82eb3884b4e2fa1fe932f5f3d7c7d4dc4b7f9f7" },
                { "uk", "6559f58e1d0e0c1b7d2093d844cd4fdb9ccaabe13d97c51ea13b4b3e6e0bd1da61c0e097e33ecc4942177dd76c908f18f7993c0e4e24f0dfa6a7e588c7c148cf" },
                { "ur", "cc381b079374003f1a193467a21f3accc77a62db0c3c68556ebb9a72125ad913e3489a571068e12bc6f360c157e79f1e83adc5e9e6f45823fbcc31643d577f49" },
                { "uz", "be5a6a2f1546e5249851b2f2031d2e16e826494bf9312af4a62690adaaece3d60194df90d7202f5a6e8d9e5a97bafac88290c0839fb634d27fabbafa7cd1606d" },
                { "vi", "3e24283a779a8a2b74376bab0905b4fe3401c9c681f0486ce26866c3cb8258ba07224efb476176faa2587a3a048c3f57c5de6362d7193750645784adef432624" },
                { "xh", "891c827e5d049f333021b8568935ccb16f495fd41db93fceefe3521c896becd2134dd7cdf0441e125ae2dd0bd6a57de1f6a95b53df9a87843d729839bcb9b6fb" },
                { "zh-CN", "ca85fe041ce215b959bab5c04aa008e820f2da851ec3f4b4b69d98f61752d05074dfb6689655535eae3bd2a7cde4e351b0dd2d986bb96810ef3beac9cd35e914" },
                { "zh-TW", "8243e957c6b1b796086766b66b86214e0cd58ae359602707ae98226fa26eb12ab534b6c85bed58e83cb11e2e113ac78f5f60e6673bb29501764de5d4abdabbc4" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/138.0b9/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "52ca6eea9f46497429b9c3a33170d42f2264341f7e31d6266550c770b7d5b6e4d8f03813d17d7e6384b97903d3a75fa110e0ee4e101582eedeea8af069921426" },
                { "af", "e323ae4676b29bdb1d8403bedd55b0abbee6d4ae786a3d7e5a965b9ce0685051d486c3c88afbd9c884a80f4ffa6b7e856319545cda2101a4e65848a6c7994024" },
                { "an", "ef781341d689b991225808d55a43483d9af25259e44a3be693140ae1e85c0bd646feb3c4cd12679b1470caf1af8e876d0676860665bc97744d7e6cea3cbdfeea" },
                { "ar", "3e4cdd7fb00cafe5e655895a245f7f5651f3dc9a281642fa69329bf5d61ae46fd977eb8a92a5de70601f6d26e0256780a94ef120da23d3c53880db681417a841" },
                { "ast", "a3d2dd9a1c164a7f1952569f5de12ec4f466a9a8b903be1d5e0e0eaa30080898ae214d082027caa57cb75448c3e2d2d9c464b3deb48d25a5a3817733eecae038" },
                { "az", "2a60b7f49cd875530634cf380e03559d09c6cdd8f46487120f8626887367fba37ca152e55558f5a475635063fc9b0a489b071e56c831a0fa9bd8a5e9e09ed11b" },
                { "be", "1553f248b851fd6e4dd51a56435ea86148a784467555ae12c0a8aefc6fa33b3a0bb25ab702f975e3bbbea2ad8cb569682f37fe9617bb9dcc05440b92170236b8" },
                { "bg", "d18013c471ebb70a53e5fe209ed8c5c348c1efb71c9c37ffacdae0323c362812d05b8139009133b5c7c2d75b2e2fa1d7684fa169e0429d0cafc253bd1caa7b5a" },
                { "bn", "052c2cc0260f7be1674c8e46f0ba58137ec1fc9b914a251561d8fe7b7668ff93465b1551d00bc75e3b91a88804b4e946520f627838e76bc77478cddf1a61ef61" },
                { "br", "cc502dda4825cfefddeb5bcbf0c62ef47b0f10674a8bcb94b5f4afa82b79cec138bea128c110530cdd533a699e4460ad4a962864a7e92932f31d4a496aa694b8" },
                { "bs", "3cc9a89d44ed1df56dcc3c974aeeb5c24ee43ff72bb9a82f78b0f322a0ff3d3db14f29fd90c92445b8a104ef4f3a94f016705bf07b3fc23efab673a913dc0c31" },
                { "ca", "85631d8e40f54802f96970dfdbfd521b6ba017e01519dbe8d2555fce745b7f6dd3d3342314226acd9a76fc49bff11ba6646bce7f7fa76daea87d6cd9ac538800" },
                { "cak", "8c0ff03f667695a953fced87f49b52ea428a68b000bc21341b6e868b2e7dbdcc173a610579c37b13ac467d448b9a0fce4eda7c7e4cc218acc18802f2bf6120e8" },
                { "cs", "5baa0c571d178d946c2feeca69e41f42730acee08faa82980405cf598d4a6dc82c6e26af54861bed453e0ef3f0d8bd4161d101bc6a3bc8489003fa8620b561d6" },
                { "cy", "2beccd9062189e9c2c6be8db500f126015a31d7ce4420f9708b6564f300c904e613acf62bd1bdc2a2d20bc55adc97e62540bc8cca5bfc2fae686d01320c8530f" },
                { "da", "5449612f389e84188f85cea3b1072a98e17fd7830f9b60f3eaf0d8aa25c9132cea9abd68fe2f445bba8d813c4585aac1a449c53c63e4919a97700602ded6b40e" },
                { "de", "5494f28325000541b7273bef64765d4b93cfb7c3aafca87daf6f307fe48a83bb6ab4884ac27a5aa58f19f9bf739ed291f8d3ea3278d1e55ddf918bde1d8029c7" },
                { "dsb", "8691f196f3e6a67d55213aa458355cb6dafc8c9e5b777a0472bbd708905fc0205ea904630e61e9b9140cb657f5a2aed1f9022ce10b0678fa8cf7293d358cedcc" },
                { "el", "34361d5a9f5cff6a722acad089a9d69906105f1b4ea87a337140c3682e4979f9d24f569504b85fe28ca6ce437e18dc816bd7254e6c556bc558d0cacc22ab3070" },
                { "en-CA", "54552151d15a626f17f760cda325b976b0fd8effc5c6c1f570a6b478497498ccdd9c86379a9edb1391bae12698c85342f25312fede6f278d0475b65efea87cf2" },
                { "en-GB", "0339dd6573c6fd3778794d7bd2457719b2d32237b2889079c740c0c9bdfe21e2e579ef5e4eed7ab9ae832fd19862a20a8f1840c14036f043295fa83e531ad54f" },
                { "en-US", "3c8bb5540954ef0e8cd5d04d4cceeed005ce68c9dad20d65ab0d3dd362900da4d019496d5d9c777bf2d3f94eab87944a7448ccddee56cadd47835546585a34c5" },
                { "eo", "fa75a406c722cf4b1a8de775dd2c9e8f3844665a5da9cb32c571eba4c4f1cf6d499532d30684c5daa8a9efffd3e7846f063fe551eac1976135ca5d4679d661ac" },
                { "es-AR", "299021693cb3f138e2f4c62f5f499cb22224dbca01db506439a65cc9acf6cedf16f6cf0b5622e6a407ab7d5c028d03c7f6bdb090382dd179d4235e314809c135" },
                { "es-CL", "ef9c6401586a4c22c5ce3456fa358adbf28ade190d5cff86ca16ddda87436c34e34041da4c5443fc2f6aa0331fd9e99d478da8ad6291b6a10b136fedb8a811e9" },
                { "es-ES", "1d4f2aba4d30cebbc38117809f2a42af4bdb2c06370cec199206b7384f2654fdf3c9d76cd3dd32814e2208610fed509d46dbd2abac8291633e04bc9a38382c4c" },
                { "es-MX", "c649c9c43042c35c993616e8f38ea666afa03efb6f4f03ad34ab1f0ee74de8903a554773f4652516cb474dd871709173cd5b90c95642c90933677a560bf238e3" },
                { "et", "b57e1e12ed89612f3ffbd8534d139f5550f84068f306a7bb23ff459da618c841eff65b6a6c1acdfe139b99130d1db323285ed5eea7372d105f115044bd2b84cb" },
                { "eu", "2242e0eca06f11625495bd220c4b0ceb28fd7843cf44002000f7dc7f58181feb85c2bb0c7cc5f07a71f0c0037acb7fc0539717fea8bbaf468773eced45d80d0e" },
                { "fa", "a80e04fc73f516d18a6c9b35f1c66c56dea4995234297bb373e17398dc3ebd0d385ba92d9d79be3517c0f3b14f3a0e3f759753c7b9f3e81ed98e6a89dddcd149" },
                { "ff", "ecced3f5d5a36acf0d914c049c2fb681faa21aa8290d04ddc3f9f36ed454fd67ebe929277f7d99303f7d5291e678aff92b298e65a7a8a67c3ca9eddc9321f3c3" },
                { "fi", "579104ead7bdd52389ce7e4b8aa8500d6f375d856ea4ab8bdc41a6d5d022e2c9ba3c1ffad7ddb399adddbf2b2257d8bb78aa2bf5adf072f08ace2d068dff658f" },
                { "fr", "d5d13b3a542fcc6546c24fc97508158fb04b20850824e6939c2efde8fbd55f58e5c68aee466267f6c9ec1eee691ef7cfd21ef59ea3af3ada887f82be7f30815c" },
                { "fur", "eb80668870ab8c6f04520685b25abc5353eca18dc19863ac781f101463cde93a1284889c0b8031834faff95e8fa9c33b8b59ed5fbf86bc9185b293b3f068808f" },
                { "fy-NL", "9d0da39c3df6d65ce75dbb018ffd7db02a3e347482f57d419f1c86be43d5fff6ac213ceb6c2f9497da0219f7066ecf0b56236d2bdc39d7027e79f6e5678e2705" },
                { "ga-IE", "50101daea7a242a9f735801758add2500395373bd2af3d39e60b845a6e177fc416bc300861e167e2c2791050523111dd9e60274cafd98375ad69f00327546982" },
                { "gd", "7c74a33addded4d187936db90e5337d8e581649b9a4dd568a9ddcee54a5d4b5dd2152a079679a55b6f4a2b515a06c0e19652e917a95bf8bf707f3d6a6efb4f38" },
                { "gl", "3f5bd1e7ad82439741dbcbbfeacbeec6065945773c120ddbb129d0a5250c3c50045419ca8ec43e532d99fa327a064137f9775c6b665cc2c01d074b9b2cbafc2e" },
                { "gn", "bf20d8226835961e060be048cf6a803295c48ef6aab72ea258c761fb9712d2313e06d35cf0e4373487d8ef828e10990aa05ee71c2c33ddf7061dc7a25ca6fd9b" },
                { "gu-IN", "b4fae41e9e99eaf23e82bfa861d59707f2d27b7a0eaf71c5407ce957f91c02907ff61d115d18c19dd66612976a27ad6961e382ea8b88472830c376d30000b605" },
                { "he", "0e1053a760b0534a58d07926e0c11adb7ae03a791d6bf22609e32a1ec845052564da3d3d6bae22d6b24712610aa551ea51631be16b6e63e8fce118d232801d32" },
                { "hi-IN", "97795c74e49edd0f117dc1f1b3a98813be082f48d8eb2994696c3f71fe7e708a624e5dd3f1b902f0fc6ecac33ba447c0313f0558075f6a0605e89b8269567d07" },
                { "hr", "6b33f04662403d4362aa8fc59ec76fa1af65532f02b03780298f7ca705e3e20c2cca606197b91d0f1114c87b4437f28331812551f79537bbaafe5adf1f966fde" },
                { "hsb", "42b64b77332f3d5f378228aa8fec7a628c7c88db0affb832c23e511a13a11edc5f6b1fbb173c4585af5993694dc880e43ac9dfccd636996c951362b74949bb3b" },
                { "hu", "09c48de62606c9fb990ca0a3b8c6c676c9265b29f241e0cb4d77b363dfaf6cff0cd58052fde319eb71c54d6f5937516e8c62b544acd4010bb6e73f9c310e1e6d" },
                { "hy-AM", "b560728ea06ef4cc1eb7f7add7ed1fbe61f1fc067e0562ce4421561ecc0a11e774f0d028b20584731e2dd715ffadf36801d2fd34a88fe12364da34eb4eea4aaa" },
                { "ia", "d8f7b524797d04ec148cb9d7601559ef9ccf68366abbceacbf4ed4a36dca73390e293d4f2f7b4fbd64c4bd16663e134dcdf692a5478612e7f96e07fbf2dc727b" },
                { "id", "2dc607ca86b9cdc1b6381bf4153b9c89800bbc5c08751a807d5be2b84a1ddd8795d1886a773669a652b1d3ff7e7eb999a207baa83046641224acf538d11e09ed" },
                { "is", "c117a7b4c36c3aa7f2d9f2ffde1ee9a0ba5e23ee442ac3c270c478f9aefdf4d38040d952f0242b33c5c23f644173e47f4aff64d6cba9dba65a9fa4d288739318" },
                { "it", "068e3a77a4e76977bbd271c7dbf9d75d630c6a108548925ebbeea2654b788ce7c1eb36467846a789de30af32da86563f6f77bf6024f9536d96dce61075027f76" },
                { "ja", "6512fb0c3a9219528bebb6f83970e3ad5493666c4fd922b4d30f750a74f7415f30a84fe787c99f35969b56efef97a66293c44b76af5b0b9d4b438ee521175d1b" },
                { "ka", "19df83e21e4b80e93256a3d9ef4ab61c08f9e19b020e45d40d7be12279d58f918fa9213c5a2af366dbbbb8eaeb291c0b6321c0d7136e00251bb50495951b795b" },
                { "kab", "bc1c3cf959d2dae3e80f7cf90744b1f34872c8c1f78014a425e7e7ab5c18686032305967be0e5f3b3f29e3c54ff8848dc3ad6321ce46c7ddcf4ae57cfc61993d" },
                { "kk", "d496fe8ffce48a8a5c113ad61e6098f9d0d4a2610d2989e6381df8860b4ed2469fc9a28974a96fde2afc6ac3f636efb35f0f8c12f2f4ba57b3efd616f0410636" },
                { "km", "6d7205e4205c5ce228ff71cfac07e52da1006195c8f2007aa229fe2a8bcc4ead0688681b0d8ce5f1403c3fce59371bea0a2a544115509db672d45884097cbdab" },
                { "kn", "68e9dbb1f551a59dc3f0ea9e1badaaad0432de048212d136ea35c9ba8af14735af981ea8bf806910b065b0f966cefedde5fce40af454ac1cb0a67f985dae6fae" },
                { "ko", "0d63319c9db4be1896989e28a32ccc0e93b8dc14fb0de12a9f8d77286d8c82ef49f6b0185196afccaca79fdccefb6136408054a9ca6d5ca9c6eff955a6ef0b12" },
                { "lij", "2058c88e3c3d107a209f98b4af962da1496a2fd72ba99944bc1df2da76c131e4295eb4d03e4c2182004176b59c064976e9e594119bebacdaf04ef4637c817cb1" },
                { "lt", "14a685c02762552cc943988ab22e2ec261ce55a11cb4a0385e577e181f7933aef47ec8420f0d0676da8585b4773f69966d6933a17c48f77f6aedece56a55b540" },
                { "lv", "e248a789ea4f29e3292b4c7f26a63638ec9309ffce17284cc9ac12fbd638fb14469f078cb54c1e65a826b4b2c4677021f8478290abe791c0c7ff4fdd27b0af2f" },
                { "mk", "0db27d4a759a4839c589b92ed67d122c565f73540aa7a8bffb6f4589265a46f3557ef63ad20c57113eb7c5101e202524fb1c54d6bf9ea1d986c9f9a72329397b" },
                { "mr", "c216b2090b91749663239d2edc7d33ab8953c4b606e708a6baf3a92c2a6bc828d82047963ba9abfc212d14d925056e9ef40bb846f3b408eff3b6990081d94963" },
                { "ms", "384ad7e50fcb8f2397c85f3278fbbfbf4eadecce9a293d39935b51f9a568b1248d3d7bba209c0405575726fb6cb0f1ba4b6744682f17febdf83b6a82ec9f628e" },
                { "my", "73e953b962d18d70374267d98495030645a97c61e8b97450d749745c972790d656364cb524f53ad548853c5b7bc50cf547e51362e2ab7ead44d68d0460248625" },
                { "nb-NO", "de9fb013d0b8bb7f4d10fbff45c9d04edd8920c6b7806c4ae7bb891809e40ff79941b28d7a2264c0a9080fd3883205b9e85f708b5205173cac1e340b6026a664" },
                { "ne-NP", "2ce8a481db0231b2f707f9838b5b7333f5316edbc672083729ca90db8baed90ae22e86f09512984a536b0308537b49f28de63bc026b8deba94f30e50b4ac440c" },
                { "nl", "b524f5a5caba142af2d81e347cf775be134989960b14404c6152f966569fa69e86fff10b7c57102c92ccda8ae5396c63c4ad53e878fa5980461a69bbe4fd9986" },
                { "nn-NO", "05707fe522cf62fb92fd79ae5777c2870aae7c556c0856e6363a302b9a92ded7b6a153ba001c807e134f5b38e940ac96f7d426577cbbfd58c6a7a0cd14a97804" },
                { "oc", "22b6ca263b9a479ca6d917de51223777999d82bd87fa53f8ab98a9a9d38a87fb4245d647ff488d198a725377497071d4ec72ebdb91283bdddd5552586f962fbc" },
                { "pa-IN", "077edafe983c7234edbc77358a092c49c8a440074f92dcc9aa64e6ef9291de9049d28dfac738881dc9bf0148594e45f39ed2e00e3b7a9856d71c6f8b49b87939" },
                { "pl", "463fc86f655e8b53672bd78294f37fde55c7e116793480a06c1dca5d01931b6d9c339721659fc5ca03763da4f368191f4dd6e79fc34bd1b4cae52adbf5c43f97" },
                { "pt-BR", "026d16cc97e4e7907bdf267a8eb2ae655001ee600a8137234f72c0100de96a6a3be1e2c45b8366c51d514b60ff4cec0db28b48e7daf1d5718df9aebef96910d5" },
                { "pt-PT", "76ae49bf022d4bed2c08f47e06f35c8f972706af09453d48108eb8ad5f962026012a276fe63b451be9f63794f1d21015c23788d2ceb32903c7857a55abba2b0f" },
                { "rm", "f7a08d12e6744573f9b3628efde69b63f95ff0c34ea232d08cfe384f38e1f71281e7cbd2a6a09d3fb266c76cb9d245f0f2333aaeb082faf887027049ffab64ce" },
                { "ro", "2861eec66f714817b6a6ef99af9258ae7fef0fc6c8f16d2d8ff6ef9b15c42b4e3ccff5dd3db2231bb6ede4a26add6661c16dd8c160a57aeb596f001d3995742c" },
                { "ru", "e08a9b62431aa421377c01a75962c233e9999ce8400a6d78956056605f7ed8770517d6c4894b152bb0a516746e8c9370df2c5a13e280a21686288008e4cc2b48" },
                { "sat", "b37e510b7a62ff4a65c34be8082a433be85738760de25dfd6d2b6e659f36e16d86876fd4206a5d26dd0846227c973371782233f3b2c47fc67b11d866381fe2cb" },
                { "sc", "a7a09de0dbcd3575c2a852875da16830725d2d6277e72e376f3f000f49ab530296190a65d2d7cbb09de65cd2c33d12aeb0f1b03c9e14d23d1f6f036fd5ea27f2" },
                { "sco", "00bd9ef14a2752bfa020509123832d4ba1b0ede4f7211f72b50db0733279a44d924591536fb38953cecaae9748ad9763aad7bbe298268450c6d5b4d3239f2895" },
                { "si", "2f65344ae4f3af1a8b2c01683906e2ce6a20d6df833b2c67328a1c487f58d52cafe25083265b029ec189ab24dd7fc691e44d1b2e340535d70d7316ad65828a39" },
                { "sk", "aaad239bd8ce7667707c4b1c499f117ba34e4308ebc7082f7e37849dda911e0847c7feaebca16a29baaa27bf73e13bb54e4bf7431a76e76e1377568e248d1019" },
                { "skr", "d87c50f43032c68bcefbe3b7f05b1f33f72ba70afbe4cc161b6ab4c4c79eab16226fb1c54d55783d30ffd831c2e57bb11e7c2128608bc56d61aad18518fd893a" },
                { "sl", "a0594fb1dcc912427c557749679447a9efb5bee9c5d6bec9804e7b1ae691453e4f67078ca1d3cb7909648e65000eef4ba351c14d3a32df7c3c1fbceed9b53a13" },
                { "son", "c837248f29631e24a7b52f08e20400af67a85d9e821b4d8fc36f88f1177cdf7efdd2d2f804c8bb3967ea3ea321b4c17c37d394c79144b9430db2ae0bfbd0dd1b" },
                { "sq", "5b05b96d22329cd02bd570738cd6c9e5bd354124b013605af4b7578963cf4f07691a40f52864f36371c894cd1a413f1875d8010f78db42141929e07fe86e002e" },
                { "sr", "af8602d2ee6a71df285c1982527765c20f15199b70a6c80d6f613cdd6bae13a154924105b24ec0146ec5b34f25c5e4902e7a1dd2f3e8f25e93d82fe52edb7c3d" },
                { "sv-SE", "ac42c983554d95897aa1dcde42358134291954917a4222e129ba66e34bf66c0ce43635040a494c78abb99a96e3ab242fff0fd5b0d8a93830e0cffef7f7028a56" },
                { "szl", "26eca33f3d1c9a04f7bd3e90b71f9dea039970275ecf4feb43c1ce28c7646be74c99f55e1198df2364afeafcf799dac5c40b755f02ddd2a190e1b56fd5bc854d" },
                { "ta", "47e7d4d6737e5bd31b3354fe4e40cc4af8d9e2cf2284886ca94595a23269d6be2def87232180ca6903b5057a145bd7a9057a79d6dd7b90ae1d231afe4ce1e3c6" },
                { "te", "ecd8044cde6bbaa566999a6fdf0750075626e95cce3c80449e07bd9a6bf1a6899a81d4d9e400f98717aa5bc9673813ea0dca790e4793665d7d168446160d0bb4" },
                { "tg", "c02ec9a53def75244d747050c5d1008bdbb0ff438659d0fe3bc0275707582480183b4ccb65c97d5de0fdd23385803b53c1f20fea07cc02d8b4ad310f552c9f54" },
                { "th", "fc7c4682f74fdbe399bd825aa8baa686eee688f98d74a8ccd29951d708bc401167f12d468a811afe80591d81e78c7bc0f88dccc477a16d3c2d41d66ec6b7cbe1" },
                { "tl", "2bf4dc9610033242cf0648323fd4b71b2973b683493608ba33698e0477376b5e54a15c0045a6d666f3f65edf43a4c4d2873b83253706200c6768d631fbe5fc3e" },
                { "tr", "73e7b7ee68207acf14aaa8f8e0b157a59e6e0c8db51af25133dd3cc2ad6af7852f5df684b042f225865338cde62a2c0e78e51134756dc63f1506b4c570d3307f" },
                { "trs", "09149047b081c1d82f86b1b3e008e6adb5c973433e42c2eb477dfcd1f3d346f204a3b2143bf740ddd10a2555d3958896117a1dda93996b23ca26024e098a555f" },
                { "uk", "6caabe9a62f68281afa0e1709cc2578b2275fd053f9eeec1f40ae493cfb0141fc2904c97dfb4289e63ade7f238d179b4b2eaa1d1e584a974b06e127002cdad00" },
                { "ur", "f1fb0f5e6ea2fc8c00940316d02461acbf8c0e08b41ff05046e84fe8c3f67a52a58456ee6d61e8ba9d4ab735c77be26c091d6c09945c44d215ff239f22860408" },
                { "uz", "d90242923034e87b2d392d71bd325a5a54e0ee9424d63234a0b0b3029983722e55c1a2ddb2838f5754cd1ea65e13ea9f65fd01bd5fbecbc5c102f25aba294b47" },
                { "vi", "5ea36fe616c0b48b933ee8c841c472d044c0f8a75070be364bf3ed2da94745d9cea5b77b81c6355b1d119901cec14982dd23c594a36bfc8e3aa0ea852fbd5246" },
                { "xh", "bc7fa9b2e561ba12c7b9083e6da763f583ea44164bb1653df8a8c7f972ef59055a55f1370b1c1504aa45865d270044313222f555938a77eeb4bfd60e8c63b7c7" },
                { "zh-CN", "7ae42aacaaa64a8ae71bec2810b260a37cea3e17629db171daf1433aa7584669942bd61f315aa3a48884497992665556bf0cbaa411a3eab2e69c0c3f31d6ef6c" },
                { "zh-TW", "378999993a3cf414abcebc7053f72139a7853402bd43dfd73723b647711c83cb31357a9663d0a35cd477e6fae08ba97ee525cd6d3ce5a2b1b9887c72bbdec32d" }
            };
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
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Firefox Developer Edition (" + languageCode + ")",
                currentVersion,
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32-bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64-bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win64/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    signature,
                    "-ms -ma")
                );
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return ["firefox-aurora", "firefox-aurora-" + languageCode.ToLower()];
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox Developer Edition.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public static string determineNewestVersion()
        {
            string url = "https://ftp.mozilla.org/pub/devedition/releases/";

            string htmlContent;
            var client = HttpClientProvider.Provide();
            try
            {
                var task = client.GetStringAsync(url);
                task.Wait();
                htmlContent = task.Result;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox Developer Edition version: " + ex.Message);
                return null;
            }

            // HTML source contains something like "<a href="/pub/devedition/releases/54.0b11/">54.0b11/</a>"
            // for every version. We just collect them all and look for the newest version.
            var versions = new List<QuartetAurora>();
            var regEx = new Regex("<a href=\"/pub/devedition/releases/([0-9]+\\.[0-9]+[a-z][0-9]+)/\">([0-9]+\\.[0-9]+[a-z][0-9]+)/</a>");
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
                return versions[^1].full();
            }
            else
                return null;
        }


        /// <summary>
        /// Tries to get the checksums of the newer version.
        /// </summary>
        /// <returns>Returns a string array containing the checksums for 32-bit and 64-bit (in that order), if successful.
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
            string sha512SumsContent;
            if (!string.IsNullOrWhiteSpace(checksumsText) && (newerVersion == currentVersion))
            {
                // Use text from earlier request.
                sha512SumsContent = checksumsText;
            }
            else
            {
                // Get file content from Mozilla server.
                string url = "https://ftp.mozilla.org/pub/devedition/releases/" + newerVersion + "/SHA512SUMS";
                var client = HttpClientProvider.Provide();
                try
                {
                    var task = client.GetStringAsync(url);
                    task.Wait();
                    sha512SumsContent = task.Result;
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
            } // else
            if (newerVersion == currentVersion)
            {
                if (cs64 == null || cs32 == null)
                {
                    fillChecksumDictionaries();
                }
                if (cs64 != null && cs32 != null
                    && cs32.TryGetValue(languageCode, out string hash32)
                    && cs64.TryGetValue(languageCode, out string hash64))
                {
                    return [hash32, hash64];
                }
            }
            var sums = new List<string>(2);
            foreach (var bits in new string[] { "32", "64" })
            {
                // look for line with the correct data
                var reChecksum = new Regex("[0-9a-f]{128}  win" + bits + "/" + languageCode.Replace("-", "\\-")
                    + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
                Match matchChecksum = reChecksum.Match(sha512SumsContent);
                if (!matchChecksum.Success)
                    return null;
                // checksum is the first 128 characters of the match
                sums.Add(matchChecksum.Value[..128]);
            } // foreach
            // return list as array
            return [.. sums];
        }


        /// <summary>
        /// Takes the plain text from the checksum file (if already present) and extracts checksums from that file into a dictionary.
        /// </summary>
        private static void fillChecksumDictionaries()
        {
            if (!string.IsNullOrWhiteSpace(checksumsText))
            {
                if ((null == cs32) || (cs32.Count == 0))
                {
                    // look for lines with language code and version for 32-bit
                    var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs32 = [];
                    MatchCollection matches = reChecksum32Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value[..128]);
                    }
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    // look for line with the correct language code and version for 64-bit
                    var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = [];
                    MatchCollection matches = reChecksum64Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs64.Add(language, matches[i].Value[..128]);
                    }
                }
            }
        }


        /// <summary>
        /// Determines whether the method searchForNewer() is implemented.
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
            logger.Info("Searching for newer version of Firefox Developer Edition (" + languageCode + ")...");
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
        /// the application cannot be updated while it is running.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a list of process names that block the upgrade.</returns>
        public override List<string> blockerProcesses(DetectedSoftware detected)
        {
            return [];
        }


        /// <summary>
        /// language code for the Firefox Developer Edition version
        /// </summary>
        private readonly string languageCode;


        /// <summary>
        /// checksum for the 32-bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64-bit installer
        /// </summary>
        private readonly string checksum64Bit;


        /// <summary>
        /// static variable that contains the text from the checksums file
        /// </summary>
        private static string checksumsText = null;

        /// <summary>
        /// dictionary of known checksums for 32-bit versions (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs32 = null;

        /// <summary>
        /// dictionary of known checksums for 64-bit version (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs64 = null;
    } // class
} // namespace
