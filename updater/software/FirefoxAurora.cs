﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022  Dirk Stolle

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
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "107.0b7";

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
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var validCodes = validLanguageCodes();
            if (!validCodes.Contains<string>(languageCode))
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
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/107.0b7/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "3b3c4ce71e7cbea43ce84d1c5197bb6086ff840b9edaa770cf29e60f0244e9c79c5204e57aea9f0427f448d8ef72378ad356a4026776f1341da7d1d5af2c5269" },
                { "af", "f266044399b39c0024785a4cabb4ebed89a457ed2fc9ad23d8f0426adf3c49f6b85c5bc041a023b774b1573f7f513948d32002532615e6dff9220a21d00e8187" },
                { "an", "1740106f5f80fb7c30ce11b6fb0ffe730c31033254923c535146f7a32acfd82220f00cd57fb65ab2a00d904a378471a26fcded1c8b5a0896b3da8165ae49d33a" },
                { "ar", "87f541bc5c99358f887b063ae323026f3ab2fedac2e12dad00e1879ea9f3d56f76e944b97c08922411aa1184ba6a34c76edd38d2bfd97c663e22ebaed36eb574" },
                { "ast", "bb46653de00dd68e37c370c6e8281f317c344f9402d61da5c8b753b9b586eb7e7f5b2423919cc5ea8064ecd32a756ee547b0ece26d1475cbbb89601ebce781f8" },
                { "az", "b543daee838fdbbebbf8184bca65829ea0342dfb9ce591a13f7c60a28a0f05cbeb0feb0dc57dfa4a10d6b353ea3234e66e9730d3d04f09644808e3ff1994ccf4" },
                { "be", "40ac50507910cdbf08e764a47fc8089aa1ea252b4ce363c182ac77eded1578f71b19acc99b1ce20a549d67de1815b1f7c0f4e5c5fce0cf3cdea586a940f010c6" },
                { "bg", "20f489ede24e35a5a594f1cdcc2da60611e46b2c6f0cda68db40a57ab94a9f1a9288c2f1320e97e33150b6f52f4a70d851bf9b770e7d2c0b05721cad8f839972" },
                { "bn", "e749ee4f07918fba4039d385fc1941f01b4825ab961cd4cd2fef6089690d1977eec24ce04d89a55c5edc894fb67d01b605d5c66a7a17743cb270f8ab42bd2ab6" },
                { "br", "2dd899d116dcc8a3a98871d93ebb49db85c2ab520bfde1a0b70e6aafcc2a746e624ff2ff88d83e443db82ba87a8ca4fbb8b647058a0349c46238fc21d0e40831" },
                { "bs", "6301d2c55cf3af80ea66fb3422a9afdc5b389712c34b7c48318ef3dd9d7dde49b016cd2d11b59e9499ca36ff9d2714c4bb69bbfebea9089fdd45e2ca82ce022f" },
                { "ca", "1be44007ae8ec6125c023fd10b9cba243015540ca156b84dce47cdc883fe77bde9a9e3072bf77592458404764a84aaa73c7bd1eacaecc23a621a48e8591bfa2a" },
                { "cak", "da46d631dd41de9b5e53549424263e765e34ed75cc49094ed78b81a7533132dd698ec43acfd14d1b4560dd9ffa14ff6d8a25f4e83bb64fa4bb5c8fb70c132c0e" },
                { "cs", "e8c0310eeb5c95e3982b351adf5ec298cb3bf728ddf3f2f50ee73181aed6ab9a4c33ded72ce69b25039bd4cc8156be3d0199f6e90110f6f5637532fe2f738eb1" },
                { "cy", "f1087f3488417f61d1a3d29725529fd668c4b3399d87976dffc8b12917b5992ea71f2f783c2c56a7f9ec26885e403d8cf201e4d8caadaeac0e6f77a4383a38fa" },
                { "da", "e73217df9da620ee5b1526158e4ac2a371707ac263f3b940ffffa8d2d79f3a984b48c3b8b0effe8d4a87b82ea46a6cb21739eb809e4499bcebf108497f2f94a2" },
                { "de", "af9c2aa0bf805033756ec327beb8b8a2949679ea6a9e11fe45bf565ee373e6a82d77befbd38aae5272cea9fe3149817367a5ca6f17932d0e2ebe9a8366a14c0a" },
                { "dsb", "961fddb256fd07fd1aa0380c1ec57846c9d2742a5c6f2943daf22a009efdb914bcb8beb4fce0a3b96dcd7942d469970155e00881b0b5e6c17139cc90da5a692c" },
                { "el", "20659b51545b1c576a6a333f3a22358be6bf6cab6211effabd85e23f0c587c80af827ce621f2bbb3e7542fc5799ab289e119ae86821d5d920dd828224cd63db7" },
                { "en-CA", "f6fe85f9b4a2f0d22f4b7bc46453950c2d3c395862fab1555332b04d2394c2fa710ff688190d6c963a4a59d72b37f379bee69dfc26f260e542d13e2e9055326f" },
                { "en-GB", "2913102619ef25863e779b9c75294da45ec87c873781ec74eb264327bf35cde2bec528bdd752e583d66b16be0769d5ad24e70e31c797d71aa0cf25052cb8b8fb" },
                { "en-US", "c1ec1db0f4eec309c308b4e31b28ad1f56ebe87df095c7d94cacf3ed37b94e8467f3dab4bef3e2d5b190f0af4b90bf04a1e02e19824f979d2909bd31ce377ac2" },
                { "eo", "8e6017137b62970210a63bc0955febe5907aa7f50da758773294afd1ff9f372599c348a56d7912ec2e3702dd3c7ec8205aad85d593f8a3a431c89d19fbdafc4d" },
                { "es-AR", "055c0443181b46e2b50c3c18928ad66fab3c2cf78ed8769458edec652ba4f48d2538ba5b38ad6e19e1048181afba21a462c00e284b829b57fcf5469e1845ed09" },
                { "es-CL", "0445de330ce3d825f3a100b46e36bc31f612dfc3ebea0bc47b0c1a75973ea888e6137a80da5f378c82d4e902b1feb819ff2711cae08df9a44ab410e29dea6017" },
                { "es-ES", "0a3c4cbcde24942967f00badc99f6e39c3d33b7ca763abf79bb4539b68f7559c69461818d6196e486d7c0ce2a1733732bd2d539f68ad1580a372c1c8c69da352" },
                { "es-MX", "d8a91bd2ed8cec6e768882f27dd6d3ca99bb3301410f3403f8822532cf48be46fd9965ab3f7bbd08328a65e0464733ea15fa4f00e69ea66e8a56996b0657b9d6" },
                { "et", "8e82577acc354c00679e56744680de029269650776652db65dc048cbb144ca740a13bddfdff0cb7f8269c78c6d312323efef7cae4264f31294b8b5a0febf8705" },
                { "eu", "58c0fc6ccfcd71f038c674e2ac8d0db73469cbb003ebfe421b9f18c3babe36241870c478aab280ec961cd5285f5542bd24dec1498ac6fddcfdaa97f1030958ae" },
                { "fa", "bf15aabcf9f8b041b9d93699f5be1962d01879ca7895314b5f88d54e90e2d1be1247e36ae0e1455524285bc4653bf88a0698f105901462e70fb2bd03352152c4" },
                { "ff", "9ed692751f50fe95e205af4758d412cb7479363ce20595c21b31d952ff71beb2a627dec22d1945f637e763e939613641919b1821c8afedf9ed34b3e41263334d" },
                { "fi", "bb4833ab076f7d9a3d16c6c33a36a089f3a44af729a6f9d281a81cad352731f8024db07ffe7d2913673e9685b1862ed45e94a34e8a08d965dd328e7787a948ae" },
                { "fr", "688b203ed74646912eeabf1cd25d637aa8537cc41ccb18136378bf9fa0a3047464fed3d1d755cddb2631b96623bfe7633e973885901d1b9cf6a850d25ae79027" },
                { "fy-NL", "58ef64017c68ec6cf24ea7b8d9197e52fa2bb432564f6206c45f16808c948e14808404ec2af22c84668ee8322115403885c9c9324d2ca9579689e0a021e23059" },
                { "ga-IE", "590e9d89e578273a0d3b669b5bed654f788c53d404ba702779b571c740fae08ce6ba2b5b43602ad0e0899432dfdcc5bb7977070a65f8709ef8cb47ebe8179210" },
                { "gd", "23c6f4a737164ef4e6e8982fb43b232efb101e4021aab5bd1577a3cbbc8cef9bc43dd68b4269dbec76ba6622784391949bfba07c1f7d07e95f25ba2b63f2f3a3" },
                { "gl", "91019fef3217f2328dab574a9c2bdfe517612ff446b117cba4683ec5c13a83e1dc767d9f57b509150559b5ddc4667b88483c294679e2132db16049a4559ce48b" },
                { "gn", "765c41529ec6f2640009f9a53f6eaef92660ebee4103c7058f55c7e263ef31e77bb62517923d3443b91219207d23bad8db310fe340efddc422a98c1e526a7e9d" },
                { "gu-IN", "847460fe8c0b1c0ff0addd348176c566b9257a4f9500be91af50eb691ec73779c39fd59c1406ab583cd143e489f21e0ddc0345c7b0429909a42a0b4b82152a3f" },
                { "he", "b7e5dd34410c604ffc36af59b786317efde93ba72ab7538d8f62f021d50d994659f3ca3add473bfbfa640dd081c6f1cd2ac5223c57e0b76092aa12bf403b8011" },
                { "hi-IN", "ffbcfe7d54405e808db5f057c867c800f69fe774bcce36ea123f35d1b732950b53ca029c10a35c20bb5b41a09c0b8ad50f858161131ab7a690ce938d6b4d19fa" },
                { "hr", "a9d52ff84e54300bce23b32aac51fcf38e3abc016c31e3021bc878f300c9e02dbdb82f1009a10fb210c77ea4362a6eb681038889939167d49bcab91c09831fff" },
                { "hsb", "a737330e6974759f3a503758a764813a9f49784dc120be5dba57ac079a0c87032bb86faaeb4b8b0e2e9236584e24dfba5a65239ad6d8a1a1fcd2f325ff985510" },
                { "hu", "b0244a241b246d34bdb927a2e93276f6c4200cc17337c0c9f519407925ac3ac3233bfcca70a1d67307c3e2675524a6fb3a7e7131075b8129d0d40f43c47f8bbf" },
                { "hy-AM", "d804fc9adcd11d9097751be04326176f0238095af3f25422383fe87492f8d0042e9e6072b9ced979ef0d376c2ee6babfdbc403b05b37fc0f2443c7b77ba2588f" },
                { "ia", "f406ecda6d2fba754ad8835c4ce8134617372b7b302dbe02728bba1454f2585b51b09c057e006f56f6dc8a9b4df4e827471878c1583c0bef34ed594eeb58157d" },
                { "id", "4cb21b243bfa07856f486483d2a1b5be558e1fbde28a1efaf0c6077f47efe38dc089c3a355d77e1abfc9691e1cdac6c70421cc94438084fe08493b7069bc2d20" },
                { "is", "659afbd41c5e58a472c9ff4d70e92fd8a1aba44442c8d67bbc52005bb031584182dd4aa1de315b8dab59936d72b9f963999f670903a20963be8632efe0ef6b0d" },
                { "it", "c3c08009c343c0e08df274b65d16a4408e5e120cc837973e39222367b28d6ab07bc1ccddac6ba3ad72c4faef7b23577b8685d6cfa808a8c3d1ae00459273c86a" },
                { "ja", "4914a1ebb48add8405d9c7d713d1724e2681696d34f1a42de720501a8097440456fff0d1ed9ec32c3cca28b7d99bb084cb25006f1de37bc751034f7bc6f9ce3f" },
                { "ka", "0a85d958c4c97f7146a338ef46780bbd31315b2f63806992c37c68128e3c037ad8afa58a510a512505482312c171ca6a5951efba7eb261a5eca7be7927d87387" },
                { "kab", "69fffb3c962bf95aac706bcecf89781f8385c94e0e583e8166dade65cef7091760985d8cc42308df0b7d8f4e4da77048aaa73a96a0d33cfbceab18b5a739b54b" },
                { "kk", "79cb7a49fe6f178129cf4a9ba19dc584e335587db19f8cc555ee3f783b01b53c2ed28bb05c1a8cde4ae243b47e204205f7e1d9025b05e7e9ea99efbfeb91e4d5" },
                { "km", "dbbd54264ee747808038d2fe9eb747b9f03ea819dc5a6c2192d18bdfaf767419d82119eaa33fe38e570d014bbac006fbba4684abd5009c0cdb77a0bc74149876" },
                { "kn", "00368d56ca44c3c2eda5356d6dcd0e2f4dd00615555fe93d6343608e9fc4ae4a25ab6f61662f58b1ce0d9d714cd6320595b5d99fb568c186cedc7b7c70c231ca" },
                { "ko", "3f24a099cb34ff1089010693268cac9edae06230fce68c811c0d7167938277fbb88b37bb86e71f8f15999f32a9c1d6684daf36ae2dd5dc50618a81deba8a093b" },
                { "lij", "155658506a0c9f2adb406822e8f289717bb608ccd5b88fbfe47b7baa7ebb54eff630abdd156bf403cf6d166bd8942e2d5b47413f0952ba0b819eadf398cc19b8" },
                { "lt", "7291d029adadc3004ef0ca588a2d520774fc2d51ac5a6c525a9d88115f523c4ec792d879193299ab320876acd59c82b9c6f2e1dbe207348661375ffaf7422eef" },
                { "lv", "d1f9e07d59987a0c8de253261337d80a5f17b4e2364e727956ecdc71bb5c1493fdefaff1c98421aaa9a41632dc1f1730cfb759df4f06064ea9aab32cfc2965c9" },
                { "mk", "99977cf0a29e8dabd6f98ba0d9b810dac5afcbe20d35c7e73b6257d3f4fb5bfea5bfd016bcf3256a6e5fdb119c2615f1b7d4635ea71764d34e793ba00d1f546b" },
                { "mr", "50d6bfd326416abdd2e7e269c7f24f374a9974cd94af4a811c2114f7d77993c6a9d1b2b617685c66c7e27089489c1c626843f806822948ffe37f6cc71180f1d2" },
                { "ms", "0997eeb375f0adf28b3350cc3225b6f5232f0236b4e135d64157f3f3363881ce4e896dc622f335680bbd5cf9d00e7e0e65b87b8995557e013da2fd4de90b32e6" },
                { "my", "c10c4665fdcaf0206c6072129482885859498bf026f458147a64528898f47d552694c115c56db079d1644e9a24ee8a355d983cc8003a3485dfc2eff14e79dd5c" },
                { "nb-NO", "2158235961c0f570048bd1ff0591383778a643abec19ea621f056d26b6497a4d496289b00de46b9bec4371c0159d4cc37ad33c85b49892b60f8a738bb3098ef2" },
                { "ne-NP", "87e864da2ce9bab1ddf5051f9e309b15937f944909bdbeeb6216eb6fd286fc06630bafa6f2ec5f2b86e52f8fad742c9dc8937c9da91edff8a70a5d499a71ee89" },
                { "nl", "046925efb538faeac650f7df0b255eec654fc0e8f91ad90ccb7ffb1b2613df8ac5d9d7e3b1cb1db67a0423056be8e88046fab0085a064613e1d2de2a7dfdce29" },
                { "nn-NO", "83ad4c42331b17cf144072e7153d32439e26f030940d432cff4d1d120f8a1d919d8157b9ccf4a227c507b2b5dc42dfa2c141b61d78c735085c4daa3fdd33a3f3" },
                { "oc", "a40a2d0e43f3db5532b4d136ec02ff008f2f67ee921924882090b0ad9f60a8f3077134316e260a3423bdb971b31abbe979eb4c51aaedd9082388ff137798a766" },
                { "pa-IN", "0948c7b4bc57d26ab40b23a1570388a8e6893a367f26785f15d8a739be62fafcbf9d19c541a04c8b1c59ad6ac19961ca5ec33bdfee297e5ee23a12cf1e46dd4f" },
                { "pl", "2c2d7e20dbb816c017b28ed68fac6eb7dec028108a4c17b640784acf6882df0c8a4b92752897eb707c1002c3e59e822f5c9a6ab7be8fb7bbdee274071c0febc0" },
                { "pt-BR", "f890c57261938d704b755aa063f20c67a29abc0762f2cd65175eafea8610c2285b4fce46d9f3386649962cbdccbedf522b98272c71584079e88e432f6cee977f" },
                { "pt-PT", "d138e6214fd863b06d78d0337cf1669ad671476dcde3976902323c6708085f9c2b1cf0dd9315cbd8936c570d645f3e2f79a124ab23a1af639dc6b082923436b2" },
                { "rm", "c9728a31419f84435da1e8f625f6f96f6c9818bbdaeeeabd7a14a9b574a6a5f3cf62d29c66e42841216cffb880d973469932c03e4af4c31ec4a143d23ddd2c83" },
                { "ro", "2f9e9af95c1fe03ac24997065623d1c24828e2152ec1542870d64015de1ab5967ab40f87262addd6d6a9c7a8c24e355dc62c89d2b2e62ac5530365b05ee39db0" },
                { "ru", "7f9ef9216be11ea3bf6c06841f96b3a3ab30dabdcab5eb81d048e671601bb7d62bae3464503e295cb42233a9624ec44bc9f5b6b191f2c8351160e46fdb03ca62" },
                { "sco", "76acf12e9011be6c9a3329600b9b682c368aaaaa725243d18119327b9d315cd673b7090c8f930d4cdd8ed836bed12a051bb5a2423075c796dcd01a37cc910f55" },
                { "si", "6bc882ce6ca5e175c306c9dd4f7212fa3d771a1be63f71a3d7e4f80e11a486416dd8f56e503ae75a7db3ebf38b22af9c536ff65c348389c4571e9452e0acdbde" },
                { "sk", "7ece27fb45e88fea586c0e1b1113a784c2c52c25ffcbd888bdc12a0bec5ec188b8c90c7157ae3ea0bd3776c9687f70d26de6f930dff6e11dda65505e5bd16974" },
                { "sl", "25e50bc0f78e6c63ef90977f14a6d6035c8c7004d9ca14f76a7b8b41f3741e0fbdd0247cbd3da4c9a199e47a80a43e95ce953fc0bd0d9650b9c6d85196d574e3" },
                { "son", "4505d839e526c814ccc117dd3760f31abe62aa00e0476d63664dc7d641c671533fa6dc8599f6287a70d1abb58635490bc93c36549c9ba20910ede47fa14afa92" },
                { "sq", "64e1d48b1732b5a0ee0d1df16c423708a66cf642c9acef6cf68a6e45947a5e0413f72cdb14ea77ab5d056b03b72687b2007038336f979a58f0a64fbe41dfa300" },
                { "sr", "89a4279ae1a5b4feab9fd96d1df35c31ad766ed060365b73637baa655a6aac39f0bb8c18a9d9439573a801a93d7a548a4ce7ce19dfb783d2bcc4ac91f03e5414" },
                { "sv-SE", "0edc2566825f612f2efdef60ee5e93f3c7368413f385d3fbec4f11ac474ddd48f908513e3d6557d3f3aa4d82042fe2555e9fa570920415d2138545e6cd6f8373" },
                { "szl", "0289a8d185705f9a57c8ee1e1956bc92cbe9509b08cf4e41053bea3b6538c90b89abfb2d6e431c3d8a369554caf04fd9dc1f0157f9a87efbe937c2a19c034b0a" },
                { "ta", "207e441363ad977db539589ba326a7b021a689ac759643ef31526913fe3c477d1dbccc5eaca648617edef6647ac73d90892610b13fb9cb104646c3d49e3c1a0b" },
                { "te", "b57499e7149221643928d2e1d60714526a944e8b1e1dadfa415ff203900c2b1834a77cdc253387726ad3f78ec692db93bafa2963bcde12362aa8edd8bd89ea9b" },
                { "th", "15805c2f37fe492cb582d4c903a6b4d5e57ed18d03b20682df3843a2090fc89a256494e91777d600c449425ff34ac6845ce01658eb8b5a5260abf3f6a3b05f37" },
                { "tl", "26ddcfb32b36c88dc0f993f8da2a65dc0d7bab3f99e6ff38b8f64ee02e784b3098a1846a45cc2f24ec4fcfc8ecd9f1db954bb87a3b29fc4a5910a675dbcbce98" },
                { "tr", "2a961cd7b9ee809819ed34e855c170ebb69722ccd2c70071878b64d4312485411e228174b8c2dd59906a0fcd82397a2f4306b0f2b49d8b871c2ef10d32b1797a" },
                { "trs", "7d13f961858926aa4d0411945aa7d69404a8a2de77e48155128187a38ecdaf64e852d039f25e7a85497b54c8c5fb78e5a220051747f43b6869b9905ab1099d4c" },
                { "uk", "0788d6e82178e224b97a98c88b12beb91f70ce0359521c7bffeb67fe94c1e83ec0c52f36ec83a486acb036fa7b0e7267213ef8d500f482c09acbd90c6948e47a" },
                { "ur", "e17ef2086d8037621072af07946711aaae746d08d69b7fad3a6275416dc2144c652eaf3b4a566bad327e75aa507bd2b0e03896b6b7bd54545089c71f5d8ec0e5" },
                { "uz", "299642a9d311034e9b217cee438e4f326f361ac6f01c31d48cc18e0b074c44d83a0caad64a518f45b9d0535bbf5da1fc9a1c7c679262b099cc6bb8b27bdc53e9" },
                { "vi", "c6c868354857f4341ad426901a1841687fc4328bd5c49ad26e0d3584351db233198594ed772649d3b22e18dd06b61cee8f6b4b2def720b96a46c60352d2c665d" },
                { "xh", "3dfbece3deeeca93c6b66148e5ba795d941e0b7ee6738a34ebec44255373268c2eb024728c46bac05abe13523df4e606d44887d7ae70622429f3461937dc442d" },
                { "zh-CN", "45c128d7b5ba1ba6b3a28c99ee2cddeffa65a94df91173d03bd628e8d08ee438e278d6295a8dba6807159b60f9e907491498bca499a5d44ca29cdf96ed1d080d" },
                { "zh-TW", "3dd2ad2ba5be7c5a04c7677967743750051f59bd9f66df063152063cd67dff5529f718be6789b9d6f45cd485e205117f1091968a3fe73e4908da191b98c133dd" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/107.0b7/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "dc29e32a09e0324f271b9d5d62e9469b2d7970ac7210337dc6748aa167345ab979a560bc93053e3f2448e5341022fce6c3d932679c81454b6ca5bf35cf191be1" },
                { "af", "54145238e9088d36ece7b81987223aaa327c22a34b96e6112206b297be518ffd82d006d1571fd18f82a7c6046a4ce2c0eb22c43378dbe4b31eba1072bbe897ff" },
                { "an", "dadb21a1d28c65a48dbe89649cfff0e08a87edb84bd929be989b63c3f3f31b093b7ff41c383356ef3c334b4084e3a163ea48c1c6e8d2735e0e015a7b9a2893df" },
                { "ar", "34a744434a8a453d1a9e1a1efb152a86aa3e9b5509e32714b64ba23afb4bfe8b2deb468cee984969ac2fae32732178cf8d165c746435542bf05081b98ea304db" },
                { "ast", "dbcbb0a7b0ceb9ab5d17c74b8a81d1403939aced7b6c953b51fe297039f901a33014ddca477669823121137ef428ffce4c750bc332609e781e571023bde3efcc" },
                { "az", "e204aedd28aaa10335a72c3b85915b4cc89f9e7b1c895eb22971626977a4c74cafc4903c216c1faf0cd73b3c1f464cb1905e85b7c2afd8c116759e1ca8d09c8f" },
                { "be", "53b94af4381a8fa5adbc818356d7d2354e842e9ae3952ffedc1e3d04284de2aa6a286fb049e54dcd37766974cb868773d7734cd7a0fe8adc9f7ca797e26ecd6e" },
                { "bg", "1afee8921f6ed79ced41d1e51706d46e2cc923a3aca3b2a1b350a3d77409bf1aba008f2f853c118754faea5686c4b8648e1185c5cf06ce3561d04bccd948e8c9" },
                { "bn", "1011ea837befb57a6b813b18ba81ac30ceafdc648498bf80ef785d507b1980f15ca1558f3fea228efd6981a5791d5ee10ccc6755b19a25a23ea4c17af1d23918" },
                { "br", "11b5c7be2b1c379e4cdf5e67d4eb73715d81796a979c6f9e625286c4c75f231ca57a1375de059fa35fa9f03a6e637d0443f656efea5245bc2069e6c90eaaa8b8" },
                { "bs", "925883ba289e7df9b1bcefe240f3676d7e1e7f94639c7faa5923abe9c89324418c79250c5ced6a4131077c979319ccabac711fc8dd7d940e38d13f271e1837ca" },
                { "ca", "4b8d341ec48f74b0a1e28f8ca4ecd04d2d5c32d3bb44eb5252a8cd46742bacb8c877b36526e603ebc7b4a0d0967d394e79038498ecb18be472b4a187ca592e05" },
                { "cak", "5decb938cec1767578faee31bd8af9f6e28ca2a1e9fa9a895006b0caf69ae3dc439d039ee4e1e59992e9deb22150a4b0ca89cca876da548099e917de0af1184b" },
                { "cs", "216d0080799c8f50404146a6e76a599878edfbae91d77b8e671b2a2a8e5d60444db2ea050180a9bf83edea12549183f00343707579d407cd5c7c4e2017d05452" },
                { "cy", "29f2562ab09db77609235bc509eb903eb68bc55bbff633487979f8701bd494065923650a3e77f334bd2c2c55d7e50e914e57c7a30741fef77b64d25099f607a6" },
                { "da", "b7c010530eeb48da3934bd12fb311915d9f02d4244b11bc60613f9cbc5acb80399cf4f26289beda2964c1511f2723fbfd3b763afb41309ed8ab152a35fdf6f21" },
                { "de", "dc142645a111ee27fb4f057cc2a8eea42be5b5de182e01a59bc1c0eb42bd753028006cdf05dc4dcc9f248b3a37baa52884475cbe8a0198e8d0e34cbfd95e545b" },
                { "dsb", "76e3a3874c7a5a88f720a1746d318a45616fd885f29f2e55987e09250b17e1e8f77c1cf913267e63d006ec9db9813eb35a9d8ba93ceb69232369126e09a3f8ef" },
                { "el", "6e07e21d95427ecdb966c1188ccabaaf48f81ed68c97246966ad8379fa49f8e0ac52a750c48cf49435f867b999289a689146186b55985d8abdea6d2a341b7b78" },
                { "en-CA", "c43b53d7d911cd3c0475802581378b65a2930d763f1dc0ba155cfa691bf1d5e78c0f82cfaac280c2a06b16158dee6e875d265dbeee211e2b0e389c3a569f294b" },
                { "en-GB", "05083a7254b83d8c7cefb237df53cb51d754cef3f39c08c28e280db3c7fb360269e53c4ab62138a735730008f7ad65e0056d4497ee65abd888874f991cd87733" },
                { "en-US", "430b12cccf710620c29060fe0c4000bc107c8b450b6a0e07e6521f92feeb1fc6db892718ef0930b3d68f3b81a7333f82ce91f87d0ab1d81b3a11905accb884df" },
                { "eo", "3dd11edfcb9cd94ad3af58caf1abaa3c40c29fa356f0c5b184ce54c03f5a5778c52faac55be3c9167bf01a2e056a27f374248425b5119abbb141b395e1420e85" },
                { "es-AR", "2caec9c3d7a0a1587c9b6e63955278be86078fe02576582e2f36882f2896510f65e0ea575c29faa724d4fc1d467d179d8e9547e4219c3cab384bd2d66dc5619e" },
                { "es-CL", "00ec871ef62f033ca8940d2fcbebaca2ca1dbaf776d44a46113c5f5320e816ee1d64e7b870fe54666abb6afd57d05f9dc984db1f86fdf824558a6e6b19185bb0" },
                { "es-ES", "eea71888b69248f38a19d67172d669fb729c8c6aa6840ce081e04b6ab7420ed5a91c8b70ad6f313844cb5fbfc7da1b340c75de049155cc233bdac297974e7f16" },
                { "es-MX", "aa20950de80ec481dba4dd20e4f47b080724a255cbf1b74d194dad9a511b5b7d36787d8d284c57c1bb20054311cc3a692081e75c1449378b33676eb9df865b71" },
                { "et", "4fcdd33060b665e2281601ec4021f1585448eeb96a91021963292b86f96bd76979f7c3ee39b2a2c665ce92bd09694e9c868c2d2e485af92d57f91c7ff27b7823" },
                { "eu", "494ba61efbafca1351c332d131b5eef743bb673e9914ab5f38ea2fe05529fb5e1dab7a32141ca053d0f4edd40f104a75befe9a6fdb4edc86188583c0692081fa" },
                { "fa", "6d3d190e46f890e4773779ccfd657785bd3707d0ed27c741ceabdf1d7777c92757acee23afe1da2aecc27e06921e81b53c211f334ea785e73a30772c9b08bcb8" },
                { "ff", "612314ff8f7038ad7ecfbd8352841998188957c3f1175fd837f86f309149712d5ae098701a751712d4ceae868ea660c08b0f93f33b8bed9daff9133737ee85e6" },
                { "fi", "ca05370f7020e4eda804060753fb78e0676d2f80f6d212f00f2235610261ee3a685a74e7480d85939fad5b01fb74ff3c366fe24f0161c5f16bb276d60e675bea" },
                { "fr", "e8ef04d1446bfc8e103d2bb65f4f4c4dd36fa39130284c13c7c19b6bb9515a767decfc58448c6d50cd809b5b058d954323ab8b9a65dc62c3c9408ac05f1ce41d" },
                { "fy-NL", "ff941db6315ccf3611bd7984c0e1c3558bf9c894bd3c9489de21f9e09ec63d527851fc1ca082622176c98ce2c91f1746dc060415cf8afb5f90f27d709d7216a4" },
                { "ga-IE", "8b33fe127827e43cc0045bfb61163c8984a99becc68e61a7f7b69b78698e2eb18884a60dd08ee63102517f42a93ada41e6ae932258faa7db0dfd29225eaf37ec" },
                { "gd", "779299020916919b1a0f64aa011e3e67a93fac9da565ca4151d9bc02a48ec3aaf1e2e7a63eee06dd5a77e0b776152a94edd1a73661601e0795feb0b5cbc003f7" },
                { "gl", "c0116010d5b1bdfc649162b5f8b87b2f80ccb8c729b83309cacdc1ef28c82195a246f60f2c8a9365370c9e457f0aac8f2c3a27b2a9db6a503a6b25d41828ab42" },
                { "gn", "067b2626731405b2799d6c4e6f7e4934bce2b231733c2b45378d2cd762a1acb4ee271994a3423ba8bfd5e47a0680becdb468663bd0bed8db5b9a1e37d2611c82" },
                { "gu-IN", "2756b431c80e29b4170ecdb1b88c6a47d4be92a94a061076689524a53d9aba1c2d3587983e5dba87cfe1749e7095bb6fe0e61e6f0f13a9196cd7ef789cabb639" },
                { "he", "4b954a5e746c5752e9851f538ac2aae2043431e17a048bc6a294a32223e6a8a6d753047a22aad531e3807e6e514fe586dd8d61ca541f4da1ad8c90441c529e67" },
                { "hi-IN", "8f93927632cf3713ed74083af8b19e6735b5aa83de6be4c2cb15b323d2c380d67e7bf0c914df103ab83041ef5f304a3b710713a74e4dc22e75e5886b00cd166c" },
                { "hr", "26beeec886a9bbd2eb69aecdb4bd968b4dfb718e910640ab5eb144665f769464da896d15e5377526b1eadf177102e35eeeb0564ca7e14652d2fc9f040b9fd100" },
                { "hsb", "c17bbd7428eb5c90ebe59c82f157d72830acb0c7e5d03bb7dd77f66027c950345c6c0917c1c847fbc4a690a74c62340135e58df60759c91a54331b46b1c1988b" },
                { "hu", "b1057da11de63f39ba88f0c00813f67f78fec19cdd952824356f98bf44060ccebc4107265b8db9659a698c188a8c03e2284311bc50fd91714e97fb675ed623b5" },
                { "hy-AM", "efc825508c9a5c44c8a4382ddb76c5fe2e91aa58c30f9107f71886e052385f4c5c0596e0b8872ce74778f9afd68ca38511c50f82b56ae2a77bd084393caf028f" },
                { "ia", "a07b719c1bc1791ffd48fe7ab0ffe439f5d06639a2e91dc34cbdf64921269aa8db85a16ce55799aced34040d8083331d466de5d31737adb5dd2be67380ef0694" },
                { "id", "2ecd241dd0fc50587a713999d68ace5d93043ac41297175d31be1a8c650549b112761ba2a5a085b6b1358c1dd52515dd4c9d31ba2f8d1181b26beb8e56d24b12" },
                { "is", "33a00cc54d684a96454e588179d58d021ff8440e791856199910bb5939c6c08cb5cf555dd4354569863606a4974d0d4a2fd2a31003c706f0306aa1203b0b5e1a" },
                { "it", "30e4384c1283aafa0c20357110f9fb902540d98e96e5b7c93c2e6396b2545f6a1a173f18d9529dbbfdf38418576efa2043ee5db18acbf42eab365827104c5260" },
                { "ja", "2007ef966a5bf39f664c61d34fe74dc4f27c607b2d82aa0c5e296120758ec4b921943868f21cf7ba14afdaf269aba4898133d521858f913700edb0ce02df124b" },
                { "ka", "350f7513e57fb46b2326700caa8f28a9a71636f705e7b2c04f914a5c83bed7ef3b6ddf0d781d39341d53bcd895eff1f0a3a42231665bd328094f976978c3eb7c" },
                { "kab", "dce6f9aef4c2c2882caa371095e033fe899514671d3c456a747e9865c4043dec0ace2ab2527e577a31fd47ed233c72d2e4b6f2f23a30a2ec2ecf97795960cd33" },
                { "kk", "17f09db0986aaa0cb3989746dc70abce22e770d42ebc84012d453b00c20e03c151f637ccbac4fcf3ae80da3f79f6d4a5a8c8bcd6f214bf14e3098e28f5df8e9f" },
                { "km", "a7f6e1571ee9957292feec468432e6ef1fc2e6c095b94cf9235f8e7f98909e1ad7362361e7f44b952fa1df204446fca517006ecd5eda8b208c75adb48dda5df4" },
                { "kn", "36d0346ba798ef54ef57c98ffd0bb98e6e02d140c30cd172c9cbee81ca898cbc2046b78db0df7734f99e0a2b07d7a240175421b021904226656002a2b4c38813" },
                { "ko", "308213696b74547fad5cd323c4025d0304a8b42ca6676a0b0865ee541ccb31e525a6924a994edb8d82b529f4e967c763c0929ed48389a8e6bb84bc4a857e763d" },
                { "lij", "c558fb24480472beddf86dd681b79f2cc6d7eb1a7b86a883c3a3e2b94088a307a121526378cd9cffb53f75511af288c650bed16e5843f79b4729c70f18b19e95" },
                { "lt", "f6c6e917820e0dfc7ffd8f48c2c4a3901ac5bb1a839f1011f35055620712ecc5f49ea35de6455fbe85a9601cdfd626ecfdddce2dfba4c44841ed06fd82ad0914" },
                { "lv", "817ee127e419807e9054bde863715b5fff35fa79f8407aaf1f937b3d53cbe71cd21548b74b4f529ee8d48ba752973bf31aa7d310df7a7888ffdfe3fb327ebeb8" },
                { "mk", "75516481c11af472b628fde6b37d7c4b67351faee0f8f608bf73bac8e457c74779d549e59222355cf9b6ec4095a84ad48d9e218f221f1145e470e16874f894a3" },
                { "mr", "aaf2208f4842939bd0e55271403f62335e1c38acf39fdea5daf9f06c28c0086a842973b66a38ac307717e46fb08f5cbb30cb0167ac48188edd7bbdaf6da0043c" },
                { "ms", "99808178f2dac782fb19547e737113a025c2176cc1c05458eafd298e49f0b787fe38271526fced59f5120f9302bbed71c5e729d61e9252e24072a26dafda001d" },
                { "my", "a26c1b8615a04516cdce3247081d7ddf631bb577456ca388fb7eafc3daa9d1b3996bf7edded7f047f6310e831518337aa0044382d93545e07c4aeb1f42601916" },
                { "nb-NO", "2bd0fd9c323ef85445ce15421c3dda60b36bc19724848a021bc6b0afa5500a80776f52c5233ae9386440ae0801172da062ba7777ea83a8d3e087385aa9734ec3" },
                { "ne-NP", "c8d7b9e7fd34101e99c60d720640f88dd978a9439a04a1ecea2276d6a13d9cbc74d0f87519c4bcbda65c43bf2e3ad8c14a9653e46a9df51ace466b85c1854cac" },
                { "nl", "122b7c5493ef19d777351d1d14405976dd69aef5aaeb4d312df80d236127354ee3c19c01e4d62674fb89738cba2704b0b95604f5b6ff541329e957135f7c44a8" },
                { "nn-NO", "8f6266361ab2c1ad02fa12240864d9ca979da96e2c006706e3592e878b177ebb9c538ef89a4bbcef7cba942a1ccc459964103cf783a1ac02dcc91fbdfe4094a6" },
                { "oc", "ec68d511f6ad05822de729ac5307f6a83de93c21ee53d59d87a0327fb568d8d6bd69d28b22ac5c9edfece3585db6f1273415b9f96e5b9cd2aa859a3f9e32573a" },
                { "pa-IN", "079ef4216db2ec971c2f36f46446c7e4bb15238508be4f2104609ae347166da44935d205f396ef0fdeb3e3cfd9c700aac94920944eb8f4abb12f4a9fda7cbb4d" },
                { "pl", "33714151786de7576b378f71e4aae661bf3706d452fd87dce640cbfc4e7dad08b3e7d85f3b9b936511103c24b6cd041531e19c119ee82981c1588eb73c8bb70a" },
                { "pt-BR", "55ffa0de59a3b22fa11c10518db7e195d42de2d79c7856441e7893ed86a994f73f251a0360e5f7e1169a40564c12667300f3ad7489ccc1314aad58bb48831486" },
                { "pt-PT", "f503e4c42905fde9a714f9864970edf028cb9bd328ed21f2db34dba06e8d1f112609f8f6717007d111738c826d6f5360c18ebf3cee7b28ebef9b8b0b3b87d139" },
                { "rm", "c006cf51b40d43422b88c23d6726241beb239033df2d9d0823df3bdc4fa2f281a22563bbb90fb0556d1ce33411b199396de49896fb3dab9d206f8e1289833c5a" },
                { "ro", "88e4b0af3ef317169f7f439051e3d19e21f65129c2dccf0fc0262f62d3e4dda6293d0012d0324b1f9d1989d6ae28adfc38dfdff475303c0e65bd3cb1d02c8b38" },
                { "ru", "ce777efd854998a6ec287b9a861a98dc16666f915e230680822e6dc5b457d9d9a3ebc2af176b11663d58d8c9a789e2597d5f6871021cd10a4df4bc97df2db241" },
                { "sco", "b26adc3f1bd64a3eba867f9ff0061088933dbf26410bd05bf8776c929e2fb67d727fccda6c4f1e220c1dc2d45ae3a32be9a17488153ae23ce529539537535d66" },
                { "si", "35ac1484172327584c2aa2505e781fece4d7f9c07d1ab44d799c3ce0bf7de1d66b54213ceeaee180f6050d714dc5befc810d325bcff87e2cffbed92c820fbda0" },
                { "sk", "e6627516e7910a7e7aa2bef35200f2fe6d0d8015176c70fc43e160cb9148511b12bb7b655b543b39a31c8c2b033d71e79b288e6ca2dc7bfb4622ecef9e669653" },
                { "sl", "d20500d4300474e45b815673c5816b08c284bbae677a40ea8f49932b85b693b212671cc8f5ac131bda0c1c131da73f414990c12f899e2ed88133566c7bbc48cd" },
                { "son", "9f01e7f62ec7f8f2586fa02733a9309ca460da607b0296dde87762a230569b8477246b85b9a44bf0d7c0fbf83c4e6a181615a3e3d8df782750f8d447c80a7e60" },
                { "sq", "d10f5efc2285ac2af26dc20f3a91e8feb58d4383171584b68152718305a718132a33c611a82649fb5626e70364ad855f97235cc54e11278e134e23509453a847" },
                { "sr", "d815a46455f66935f568b864ee129c044cfbda69d97931f2cf491eceb86066afa583fefaf0285e47b998b8040c73bfbf39d4bc23ae1cf943cc4ea522f5268d35" },
                { "sv-SE", "019e71e9eca8059ac1274684acc576049672134f643dfdb8990890e3f523523fd7f5d9ca3b20019d0f4f9082aa9ef83213b8aef80d45a38d3418b19655e4a08b" },
                { "szl", "e2d1913aa6ff97cbe3886b0fadf1f03d605db435c488f08dd70a9983d480d9a4f70e64e214629e30518b05e47f91608b2e082837084d5f1cb45e1bd3231471e8" },
                { "ta", "a5699f2c5721d6a64ca4042fa28409a303c5f01f73f8545649dfecd7292429fec1606bfadcb2f00c2af2e74ea63bfae6ec946f75c25e328ff5e8427227d6c783" },
                { "te", "be1ed81938b0f756a55d715320abf00641869f3ce52cd3436f4cfda04b932aee72a7ce9b6a2d2fe34e718d8464992c6cb89097e7914bc1d50412a50ac3b65796" },
                { "th", "0e98ef831d7e9c3fa60efdfb4052299cf5bd1810eaf3664efde114a0ad5af602e30e8e6ce1f67835163946267222e653a7a554998f2a4967a7f1692d2d49b710" },
                { "tl", "4229ee8aedc78b0b82d07bf6d7f002c2ab6fa17a57505f222b3765e2d711e2278e02e30e9a08a022371994a9deca46642b5cf526ad4f4f11999e01774ba901e1" },
                { "tr", "6d0551cb595315912adc3f6f31c82752331a2a09c736b61d3cd47831cf2dced5c0669745c783e758733e4d871a7c7f4e438254de4056b95be69995be1a664cf1" },
                { "trs", "8d82c5a523c7ca944f1fd618f34f29f14f7eabfd24f04d4f932fa681ea047c14de28abaebb7e13df50e23db7778f5170ea7af45394d77ba5cb80c5e74e953b13" },
                { "uk", "273c2273c676d4c94a0e9e5e9ff38185445bbdc81e3b6ff0f1c275295092ccc0ee37f7c0202b7890e93fb10423183e018151aaac3fada2894d19bcb23360c485" },
                { "ur", "55415bb3f43f31a68ca7e603e215af50709689a5c9b812b4c78fa53c70589b2fee479c12a39290404a9ff4bc8c447435e484445a3a421c07bc5b3ef40841c0eb" },
                { "uz", "e68d202d65ccb8b5e336a754faf724e585c8f82be3e117a009d1d7a89abf967ae47a2a3bcf07517fc977ac59334a20ec18477273f1e09cbfc9f58a54dfad8d36" },
                { "vi", "564fa80e5c097ae1436fc2838bd7d2af77c39cad0479c4d22e761899e96077c9d27894616f3e476133c42c8132073ca9cda232dc988acf4307360a80aaf40b24" },
                { "xh", "0d2498adeac378b19f9ce6cb987b817fc9942a02605ce67f81758d6bd5ced7780da7d1edd28738aea0c920c792d245b8106ce9b39a42cf4bb5fdb47d9d1474b7" },
                { "zh-CN", "3b56a5bf0604311c235579d620ebbe2c8b770b63685220c70d1a32de836573fc1ecef97c9580fb3701923d4487c36aa5daddcfdb93780d54192f76a2cfb99b61" },
                { "zh-TW", "3efee5e32b201967e42851ada0d0345c07a7bbd9a8677f9234f69a71b6cc08c7ad888e6538625ff692d491119db43581c809d690bfb732d602b53b659cee56fd" }
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
                // 32 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
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
                return versions[versions.Count - 1].full();
            }
            else
                return null;
        }


        /// <summary>
        /// Tries to get the checksums of the newer version.
        /// </summary>
        /// <returns>Returns a string array containing the checksums for 32 bit and 64 bit (in that order), if successful.
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
                if (cs64 != null && cs32 != null && cs32.ContainsKey(languageCode) && cs64.ContainsKey(languageCode))
                {
                    return new string[2] { cs32[languageCode], cs64[languageCode] };
                }
            }
            var sums = new List<string>();
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
            return sums.ToArray();
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
                    // look for lines with language code and version for 32 bit
                    var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs32 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum32Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value[..128]);
                    }
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    // look for line with the correct language code and version for 64 bit
                    var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = new SortedDictionary<string, string>();
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
            return new List<string>();
        }


        /// <summary>
        /// language code for the Firefox Developer Edition version
        /// </summary>
        private readonly string languageCode;


        /// <summary>
        /// checksum for the 32 bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64 bit installer
        /// </summary>
        private readonly string checksum64Bit;


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
