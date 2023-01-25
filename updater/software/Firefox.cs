﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2020, 2021, 2022, 2023  Dirk Stolle

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
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using updater.data;

namespace updater.software
{
    /// <summary>
    /// Firefox, release channel
    /// </summary>
    public class Firefox : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for Firefox class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(Firefox).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public Firefox(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var d32 = knownChecksums32Bit();
            var d64 = knownChecksums64Bit();
            if (!d32.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            if (!d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/109.0/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "9c9650829b1aa6604bebefc954a0589e3df80a9e1853db1d667ca52c44406b3490f16cf7cb19922dc576070699cc632dd8a86708a96800a8ed52b055ab5620e0" },
                { "af", "0cc3b79369063e4332854a89b1cef675ee2fafe8077be6920d39b978644f076a97f8e42b4edd9cb4de47fc95a62e716c037c96a83e1cf94a8d38b3952160ebdb" },
                { "an", "3f36a23b750d8d8481240354dfcf81d4aa5dca6f631f73552ecb6448a0592d34a9b8f3d7d7a42978ee1a879b107d3ae2c3429c25849f66f87463f139525e13cd" },
                { "ar", "540e4603aa9a2d2783affda13e55a8d834b367ead121e24787fc0b728efd81c2b8368fc265b3a07818ee4dc2e7239ee8eb7bbf9c4d3882a5aa778f5f0cb5d99d" },
                { "ast", "e64f69345fb18ff0d2418c2898f13282a1841fd2c39e807a1aac0a328e5b5768959aaa3b3274c32b6d7f53cfcae1333fe7bc44d0c9e758fd4768c8d393081962" },
                { "az", "121f45cea150217ecf22e6195163514db435c307e1c73663c25e41cd4e18bbd99a926bf002c5ab114163679700aedde2a8da5ac2308ff287018ad7395da53b60" },
                { "be", "a6c0df58829d9adbafefa3c4f240fd6451a5238b64e70ad360c157bcc2b6f5141683cee6746a2561acea4d4fc3d6724983e0c3b4d62ed2ba83bbbd0ba21cf3ca" },
                { "bg", "c40d61504034efe352437cd7cd20a61a67efc292139b872b0ec100cc06e1c908783dc6a8f1b45bb2b4d71ad27ac4782c5d11ea401d4c6a787cc93788fff53bb2" },
                { "bn", "b86d1eaabeb103898d279caab2a2a2226dc8df0866b385146eb9ec448d5ddfd600a6f04945db8131c4d35f5e7cb9fdd71297c1954f985efa4aa23c020f23579d" },
                { "br", "479b9645ad4cf29e3a69dac14163279d243ca4595f0c32cdffdc13cabc2b8c24f4469b9281bc9d29a3cadbdc95cdb15a0c9f86763a6e6e42b2f15ac96477fb2e" },
                { "bs", "c19c391f5659d24c9a556ea13e8e71fbf8c4e9eb80396d064480fb7a90f636e98b83e1c5541db4a827eb21f9db09901f74ee2f98b5ef4e7f855862c9cace279c" },
                { "ca", "529eb050e63234c7507ae114abf5bd91a9f19f8a75169a022e7ce8077e364d9a20286589dff8c1573fa108d421f53f2c75255418200e48f5422f196883366536" },
                { "cak", "a332af66cc74e321dd3e7e2a0b26a8b7082039fa9bf5784ca9e217d8e905dc6cf79c76453d89cff79583328ed3af807d360166d717d099a336118abe83859dc5" },
                { "cs", "9771a271910504fd0d1c7242c85a67705ba279e896a8d852cc16e833632f4386c5b0ca03310ec6c8a6d2e49082b418f11cc77e94e9543368a3a4c475c2613b10" },
                { "cy", "1ca1296ab2739fd91221e80e8c94da82c02da066517c3c58a66c8479d14905097746c20a3ebffefc67137c3453aef6f9a07bf8bb9bcaa539529e2ebd1573ac82" },
                { "da", "495f2bada16d31477d7b658a94a7033a93f4ba0ce13982780a52418598582a6223fb068dc4f4703434756d901d7432493720785e166c4e27960d0c3bae4c96b0" },
                { "de", "a5f090f84e8e983b61a05aef1b0203ea5130478116285b3bef39683c97e7694c6696037f011b1b51754a2532256d0ccc3fe5d892f0bb64ef32007c4d76b935b7" },
                { "dsb", "a7bafd24ef81e332c972355296e3badbecc2a204acc27e260e8d41b965d77262ddec9d02e1034051c5875d92c1d8241c641b6b90de61f3b3f354544c69b141b9" },
                { "el", "c9b92c7a631845167473103c21a908b6032569e167c8648d177c9b495d6b9162e02ef3b787fa6ab828d219238e97d3f3d8d9ea9c3e7c9e583d0f5896ef4159e3" },
                { "en-CA", "858cc9b6fb90562d0c274104bc26802345259b8eb9dc167d5bffa08a513a88680ee1b507ebc478c9aeea043de23772ea9159fbf2570c4bbee4dec7b42be17167" },
                { "en-GB", "d2d63d47292c05592cf2e83508fb2dca971f888009ffe2a533461dd74213bf7346ed84af9cf4497e3ba7c1f19aae2c9b513874de7ce0dac3b5b5a17b9efee6e4" },
                { "en-US", "868f7b53a383c5d41f1eaeba86e4a13e1922e4cf987664ef24397084d33a85c94db0abafa6b889f8eab0906f5938d871e722a9ce63ec4386f805f3ec01a482b7" },
                { "eo", "68d108d7ab372ddec1c4430d04ad8dfedde72153731f7ee80fcd22b3c372986fdc51451d70e0d7c6a149bd557392d0b1b3f65a09542445e1026c730a5b4f05e3" },
                { "es-AR", "b99b42a55ee37efbcce1e0035c239d914eb13eaf466b9aeb22acd858d1d81fe721dcf0ae5f515e7c46b2255af4419901d5e316675a17f1e7bd271f9ece674f05" },
                { "es-CL", "7a80a10eb434008bf60a88e6506c36b4324a03405bb7bf410d6fc28ffd797352a53056b11bcae82698978204b1265a1c228bbdf956c3094b3407df30d9eed03e" },
                { "es-ES", "c2971f525c2d1d36d120efe979d8a5f4169edf4470f30901dc9a4db9c82df074d54d28e5b4adc63e9db2df7f9800fa63b22a55547ddca048fa1e0e57165f3a41" },
                { "es-MX", "ba1374e4659d9e4970a67c0f271f9b95da3ce48bbfa694a099d5b07663d1626eac8fdf0dc2437976bdaf74e0e43bef1aa1e46bab7dcd85010d6c3e33c7fa91f0" },
                { "et", "7f6ac14693513d2f096f7ab867894130d04f8ed68945f10cc93678d5fba2c03d9835711e2cb6f79751445f1504c490fbe0103fafae78d3776006829204c50d8c" },
                { "eu", "f2a93f4cf9de28e7c7a07d2b4d5cf3b1033b0408a991140451347228d6fbf51c234a371273c609516e5f25cc070a44a3118e035cabb1e90edc623865c99822cb" },
                { "fa", "f3129aa24a0e3769116069702fe9a501312101fedb5a7c8073ea132e5babc4cb9110d70f79934e3482d71fef2a0b1df93da8577d33dac8ac15955285e3da9021" },
                { "ff", "53fc6b75be115a4028f039dcad78d9b64446e360a18cd05d4facbf518986e2d5baa12c1ad9ade8de5e06d33909a011a90978f9fc9a4b8cbec101e405d0ea7f32" },
                { "fi", "200390c9b169869359ccae6ef9c3904ac5ac9b31616d4f81b2a161333ea4b85c110c6efb33d0c1a118b105992e00d71dd9965811f9b46582e0849df3640785cb" },
                { "fr", "fb413a5958922b0951f1f87a6f23de77635af205d8300269eedc81fb9838b06c0b1a1e9c9ad6831d8aab7e700dca7ef7bd4e18e28154cf6a5e8312e0fa8e97d3" },
                { "fy-NL", "719a2dcd623e583e9dcdc14446a035796184101c7388751c2f89cefc53cb35262d6b742992bffe46020d9ea62fbacb7a56ad1580870840b21990cace9d7ff569" },
                { "ga-IE", "ac578d5c3d7b5c7b0ef70164b5b370dc2945c85c07e02b81fb1dea1b994aaef36313fb1a54e250d4d8067024e2c54378ede09a2676f841aedfc7540d8556b454" },
                { "gd", "3a0436ac882eb6359644c1c86d6659b7412a7ff53685ea6e906167ffee2565deebdb904ee1ad13b5ddd43386974eb98540ad776791ac08cbbfb618d9571ddc99" },
                { "gl", "4f0e75c6ff109b5f3d02711920fbcb56ca5615eab302d83886387782aaef072285a6536e490cccfb1f013606b246c9a4f98fd63281b03ae68ccf8380da3f6e7c" },
                { "gn", "c3497b038a7db3824b3579c1c83ca2b84653da549eb0de97c341dfe5516e890b78ba8125271f3315688074672cd9b366f0731309ee2bd41491f9b4ea6279455f" },
                { "gu-IN", "34a8dceccfb2754a6e0a54fed9048a6744ecbc902cfac108022dca27bddc450d5d3af26dbcc28b7e123c963a528d8db0a4255c5da814ba6c8c40d6d53a9fdc97" },
                { "he", "8f3eb025d06f88daa4069b7b738c8819ada35eb14e00f7ee72a37dd05ba87b27db5a954b12aa75bcc9218084c939f0e1087dbc5239a219090b89d0952511252c" },
                { "hi-IN", "d28e2f553f64449ce648c2a0b1400516c9ef560fa9416bde0ea5605a4afdd8add2b59b317bee82906dc7c6c299cac3ec122ca8ca05d07639d6d4e74e15f4afbd" },
                { "hr", "7365e240ca11c6898dc1f17e06e674b0ce84d5c515a330888e380de9817f50aa2d924e0ac62765267c0045fcf0f18aa1fcccb2ec93bdd956c95f05ad192996a3" },
                { "hsb", "f915c591021c045c9a3c4bb7d064b7608cc07ef2d30ac4560f8a35314782e1373ad986e6957f102af1e8cea1a814246bfb503dc88a5e46a47017b25497b2d978" },
                { "hu", "9bd398445457a5d87931af2225db7444af34d62a8dce3ea942abc285e5d2b1b9649e529754d870ea4f4528b4497e3eef2a0d7601fcf3188771e3da8fbed2dcba" },
                { "hy-AM", "374176d4d6a8a1b8186f45bc577b6c6484b857b62cb67a88b5ce655d6ab7d20e1f39c1efd7a89e71fd6267c775b0283c9200dd72c64ac7b4c6d946a08c3aed6e" },
                { "ia", "60837de1ecc578efcf07e5c83c05fab60d0e3abbc30d227b13b2f5735226af27ddb4698970cce8fd3315651e493ad4e5cc4919dca3ab2df375f8b01df5a774d2" },
                { "id", "aa9970c890976903e014ad4109068b6c467a42d9ff540a8dcf4732583ed9eddad6c1e4dccb1fa75be96aafd09d8ad5614708ec8098bfb7f3d44885d3ecd8dcbf" },
                { "is", "f28e6ad46be4468df8e4c281be8d349288195c94d711fcd68bc4096a2fda29537456ce6d8fe9e3ff0c76c9b49d2c42c99c2583b3acabcbd68875ac058a8af672" },
                { "it", "8146a920eb2a0bd6dfe94015a8e931e1914fc4bfab259e37acd76f06a47235a745718cc488beb04c75f998091a921284974fe64bb6885ed49cf223d4c9902ac7" },
                { "ja", "5f2b5d37fb2615f0cf4bf931714b23f0fd8a1b0d64bb2cbfd9bff8fb80e498c6defbcd41bb18167795ae118bcb1632b7b80f7e5dab3e55fde2a9c22f76f614a3" },
                { "ka", "d4af37b036ba8e0b741089b84b3574ac62761e9ebf2076a13dcdfda572b580f259c9173092bd371913e531c3af9c9252dd7472a61a82a48b06184683bfc979f6" },
                { "kab", "71da9a179a2a8932cddf5d6dcc4dafb41c7720f3d6b36ea30ea489b14d37a2fe0632c6617f01496c493ce163c8669ae0ac2eb3ee139cc0f0046211111e131cca" },
                { "kk", "5db101c313690ddada0f59244ea7275b25e49e449dc7da384d70f749cd21e4e8d12e4e3851e1b77bb39d30c97a930d18960e86c64421ce4002588f43ae6efa9a" },
                { "km", "3b70ae8ad8016d612b14a3a38a82b7d4da8908268e58924e8ea32260ee864c77479eb9a7ee6a2cfb05adbfb4a6d589b213038eab27b6b5ea56e7f35ea24aa149" },
                { "kn", "4634dc1747c8a908c440db39225983a1b6a06c310d6026c0eb5db749e5a00aa98c3eb99580eb0e74e9a3b5d4ff2843e6f7acff6947a83f70d6d8030bd2b842b1" },
                { "ko", "b502c21a68a3be21eb8ae990c7989726708382158ee0851a51d584e4de5cc0b53fb71e3db346255cae8280850da0b8f4fecb30ac62cdd41ed521e94a61653692" },
                { "lij", "c40ec163b70a98b24aba84936d64b00337bdfccb1f2f00d55c1e193167c7373035d6c362e78b4a91cf4a26f85d5a64f422ec22246a0d9f2500e0a83901e7db5c" },
                { "lt", "f089761aafef46fb87ad18f353a97100eef8adafb1dd8e296135d9821225787e8c38378e7e062c0f3dd0eb2a80de88bbf78715833c853bf469910d25d53b0135" },
                { "lv", "3f64e688ad1567cac3a91d91575a3f1eb9eedb60817a04f1021afd4805ca8b7d533773908fec1467044ab3df7763f5c3dfeb0530ee3590cacf8a1fa960792051" },
                { "mk", "5a1bc127a6f9965390751ba6788cad2aa020dc7bbfd6b83fa0176d9dfdaf38c03edbfa8232897d0f5f6f7678b4884234d47c06f5de3423705b8d47ded272aa0d" },
                { "mr", "4c65965338308e5544140a751aadd0a295f8c4c02a27059547d07b32bdbd1ab90f38147d58e277e5b5f2a28775ab2c8fd02189d162187bc3324f35431ea16aba" },
                { "ms", "d724eca2bc43c5f9ee0777959ebef09b1c97aef82c40e3c17240e5d0b09baddaadec5abb83547964d6e132122f9b1798c8e72cb8116cf9b772f6fc5f9e59e7c6" },
                { "my", "007155d9c9f02e290b9be7d9946669ad48374e73a0edda27cb6e1a7992b17f5aa37af3a8cbbde47de04ec1a083cd31b553a164f4adfd209cf028717f3d16d5c6" },
                { "nb-NO", "cf6cd5ec6d635d1b62d58e4c7c909a7cae9ccb0327eda936e3f563f46da7b97f8d07a87cbcdf3536ed8af2b77ba2051a9c5b161e563309f0cbb82d72b05bff00" },
                { "ne-NP", "a34ef1619a1589b4c5796bbe0a18953285320625e97dfffc1d5fd4f50ba93ec79c52f4a971ae9ee500f265501832ad6bd2dfe761050882051bb14fe51ea1fa0d" },
                { "nl", "e721d74dbd97207ef783ca11d966e3e29a0fa13e0ae71a2609a8f71924cc951c2ccf58875aa8018d3b9c9ae140b1619680d821a34e52029f15961ea42d4fd825" },
                { "nn-NO", "e35abd45a67986baa3cf91b86389bec3ef7c962f7dd8bf86f42e11e150387d65fb1b29da7c22c3349965dbcfdff4e993d83f3161fbbcabf7d61177e7f9f18527" },
                { "oc", "41896535ec18250293a358ac217a8449c3f4722c212ea9d30b0ff1498118c1c8a4ec66c9b52d5a4fc10bc25b07fe2216186eb0fa498d621f828326f26d2c83aa" },
                { "pa-IN", "834d06b1caec8ad1455864d6cd037030557822b90ac62f544ed127f0b43962f92f5f4578a3ec8fbe17c04271fe02cc2f81a26502324140fd03cff6a13c0c2b45" },
                { "pl", "2fce48d3e7044069a0093db39474d41f70ec49f1f98c909d132be59a0dfd4372d9d956754e521e0aeb6dc87d20681ea82e4ed295394ecebc7b5fc316ccdaceac" },
                { "pt-BR", "48d862e29ac28eaeab148d4805894dcf57c9e197203c1d8a8ca937f4c0f66a34ab56f058687279267d745c6000e1ba56cb7f91c970da56212e25ac7882b4a1d6" },
                { "pt-PT", "ccf4482388ab7771ca7c737cda1459112af0f4b0814f38c6e3c6bfd5cb5a090dc1fa6c52c9b427342f297f2072b934409cae06f6a29e21dedabfa779a2b53184" },
                { "rm", "84eef95a22c064c38a4eb981e970d9b1d9d4f9c3530caace63d38dee9d8920fef84cbe9bcbb4ac340deb02e98c2c3143669b16a09e17cac0f9e08b780c920160" },
                { "ro", "4639358837379a1a98086d9e994923a013a036827f01e92abe9430304c98795dff23e24098f4f0222eecf9fab12cbdb0824fe03dd33f8adaa0879a3e9d724d81" },
                { "ru", "db47d4d306f9f3350da8d4f6d99bc29df018b5f180a54279fee9aec58eedc8f03ed8b4e163ad10b82614ecc20eaa88697d4012fb878fa5e67d352235fcc64e69" },
                { "sco", "b56ed9cf6f0b326b6648780324b4fc9ae483b44cc7d9c39e272b22060389e9305b1ebfc521075b9d4695c8b5e0b403745234b1e56aaf4422ee72f921968e7102" },
                { "si", "b900bb3556a95d645df8203ac0fef47e9f4f69ad1905bd2f2f46f7cf5d967ecaaea6e0f93450fe4247a493f0bf58e57038241101480fb66bafcdc8f0ffe47d0f" },
                { "sk", "dcae5dc0c2c4da3b1e7acdb0689a1c1c1eeac569075a9e48ca78f444ed17a924a7707ccc54531b774167c08003378f1eafc55ce501ccdf6405993ed21ffdfe13" },
                { "sl", "7ff2bd299e0c0aa12c69e122ae8f1360f024cd23603c65efd3256621b6b30d718f24653b2d711ac474596fa798ce369548b23b1e684f114b4ce6c77d868ad440" },
                { "son", "8101ee41beac05fb0cb2d4d5804925a0ad0c66b91a8a33c4e4dc43d4148483193358fb3509cf4add597d53dd16af62afa603f2ececf7ba8fdb850e1a7df5b054" },
                { "sq", "3ed9a56e3b88c5a1212eab4b59e8a109cebfdf21b9355717a243a70f78c7cd2a9d0949109d6983d9d833a1cd9dfc699a6e2f1a90268b3edc88e7451d2554fcf6" },
                { "sr", "fa2b9575ad1c59b307e1841b80c952f0b419237d362869544f281b2b49d3b59ffb398e60a8f72f04eba46e5ce1baaf3d3cd42e4b4c701627b77f24dcffc073d1" },
                { "sv-SE", "a3adbe4465263e573c48cb8c3a29d5eda95f46f1235489e8309fd8a4a4920e6ce0a20f14c67eddd5538bc31290ee2b90a3ec9c989abc0f60f19c469f13469259" },
                { "szl", "630e44c66290220af7fe03ab2d5dbcc613117fbbbccd2898d802876eb302b2f0b0cd290dfee7c89ebcc704d763e792d3b24e5d72d364d275197d9bb9b7d44aed" },
                { "ta", "300f1806e173793ecb504ac1c36beb65231f0102830f463f6f441f2ec81c271086f72af6c0b7db7e1db7b484bcfb9382d72be82882ba50daaba563d580e6127e" },
                { "te", "9fb2fe57468669b141395eb44cbe5d3521523d6de8ee07f28ac0d31f1f44d5b5540af0affb7c198656636f23ce7e9277f12da7e702ba06af5b2b23bfa9df94d3" },
                { "th", "15a0e2d2687f425ee681d74b7ff99e92db7a4991f2168a6286d3ba38f3fc9433d6762899a91acb496db6701bb63752ccaa9e0e7717ffaf614ccafdfeb14e343f" },
                { "tl", "c531358bc90e8011af63525f0ebecbb5b875880eb0a1ebb07bccfaa124c0702c9fd2a2a82438490f48fb16787526969ca2ca81bd1d5567e6095d3b8695dcb307" },
                { "tr", "97a28c1a7e9ede5d7c61ad6779da89d520ceeee96bc0cb51d859779db4924d5cc253a4b3296843b665c523269da8192a9f280a019fc367f3d763284a63d6adc6" },
                { "trs", "c5eae1fa992198deb467f591189de81b0cd69c7d27249b1b38a2d74ee91b3ca127884c9797566da239cf068416ff8655464b5b5c9b8a573b1b7db8306ebd4164" },
                { "uk", "7a7dea66af9fa3c3bc925b77395eff304fb18f85844ac91001e3d831e42a2016412baf7b29a894e62e7661cad8ddad96e559d01a351953dc86a4abfe584e8c47" },
                { "ur", "984b2ede2b619c5ab05ad924c054afcdc681b2f7d242b8d18d24caa4a49e0d588583f79db9380505e111c012b2c0cc9a3c6ac78b8640da0522da51fda6605e0b" },
                { "uz", "4fbed558492a8e56875e6f7fe4538d152c2ed467895156051d47f3a172b418aea23a8d1edc4779a365de415f863b3f66176fb0f5e947ee47c01574ceaa17a01d" },
                { "vi", "31ebbbfec04969ab0e0b207e02604b69537dcdfdfdb95fb46456809ffb601d11c0ae918f2c1ad9885121ff0744508af42a947d56e773d286070d4893854e6518" },
                { "xh", "2ddcae348b29797d43b400966b2f51d10a607cf2b422f2facb41eb1d4f29b82054f3928ed05c9b73860cc6d21065490a2551baaec134b5355bff1b8605a1f948" },
                { "zh-CN", "4f8f2ff6f128c7563850729bca885e06bbc0feed3864391ca18863571b4d5deee9290044168fae0e4669f074a1a21326a1a2390e43d0faaff812d3aafdc542e7" },
                { "zh-TW", "66a1b6b5c6264de0c035a81f0c3650c31b45495478eb54d5902a7f6fb1c13ea026dc4173ec4d79d163e428e04336dc90a0def83a7b76b27c1e9208478792aed3" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/109.0/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "c4f99839ae921bd35ee61f1b08971ecc4097c7a04d3a69f23e1c5f3a8af38d640105764fadcbaf3550e2ab2cb9c09500ce553933a88786f430bf96b45eb5d6d8" },
                { "af", "8f894624d3ab7ede696a2003fd4915897bfc91f2015ea6923e63b67c2043f73bca18d47faca04e10bd938eb1c7b3c533834fad83527f3387a493ff60e38a6a39" },
                { "an", "cba34ee3c86c1c637efa3d9792d7119e8d50e01117195a244b6492d1205be635f559f77b019285a8ce973e02a162819bcb16fceea40c4ef8190fc3543c34c610" },
                { "ar", "b29bfb040396cdc6a09e00c733b40c3ea15c4339a9f7f269d4d11caf1d282373eb9bc68647a8674ffe928f636ab5067e3774113a72979f573eb07c0065b54f24" },
                { "ast", "e97d97f557ed3cdf4f887c930b67bcd93bb2c7bb82cd041475f1dc22bd7183f0fef1eec538a36e0a0ba9b9699153d446713fa34729969bb3eb92f0bf1b6c78a5" },
                { "az", "b50168f4dfa11ea6eb7d24ea2a399a466a9de32b36a28059c3cbbafcf4b9698043356ff4be414116d287a8f235365b26a0c5b7ffdb50df28d3a3fa728456e9d1" },
                { "be", "8dabc0144d06daa1394af7d370a49d90e09c9291566ff531c67cabc9f5e5bbe3d14bca73d1d332f8c241876f755bc6978ff0b1f79aae7bd15e1c6b04645a0653" },
                { "bg", "ba3cea130c9f38bcb5849beef0cced57d90dc1327ce7d95d43d457e579bbf615859a44fbc2deb68b93ae1570ec9a55a91ad05f8c03d98bb2f5e558dd479d47a0" },
                { "bn", "f5d4fe66fd62b908860437e3ade51100426a9c5761546d2521ceecd795fd28b5d8e56b624c9b5f74b188711194071be15547303ee2d8e627c25c930ea5138e8e" },
                { "br", "acb7179b737df727da9cf9b5313fae6cde5c36556d2b8451c929a1e5048a15e6528b579a1108649ff64981c75233f50619e3b1b50555ab243c6fa3b114282781" },
                { "bs", "18235842099f64ab1675820df29887b14504a3ab562d8aadb0078a3d370007a1937cc643b69c1290948b6ba8356bb38e2d603d45854c6198c2c429f0a4b4348c" },
                { "ca", "578649c5e822286a26b381d852d1a72efbaad746ab98b88bf3d56f1729bd1f2015c798655034d5a81bb65c4152504bad71a2b37ede66df3a141f504e8a653fdf" },
                { "cak", "398a22b6c8a8d069a7c4afc0704d3a90b169c965077f36c8aa9245c4f8f81c9bde116ad7952e8c0c28a8473f15da2e9626a6e4336115448279c5b66318bfc20e" },
                { "cs", "f017291f03f4892e38a49f20790d4a2c7673de732c6d8e2c063f21305f93b7f50f571d9de9a45072843bd4da332789a277dcf8fae6d20b4f4d1285eb9dce8bcf" },
                { "cy", "8c9b9b837430b7e2ea276ce1ff5e5c820b79a49ac95ea05729f82adbd2aa8e5a7bc800525c4885bda5fff61c9c9fd383bb4691d2f0c807251115646a83fc3a67" },
                { "da", "d3e9abb6a6823f8cb877e196b7286d2cd077a7e675c1850cb7ec3f428920f7a1a7e3e9a415b1ac3e07a9acee703294f5896eaaf234d0087fe34ac2161092d4e7" },
                { "de", "590f3b21218fc9da766f9ec1014e74fce963758e4c74003fe1f4f67c9983c07b6de81fef9e638ac406d0229720b4603a55a05ec0cee4aae660c6a60703768240" },
                { "dsb", "41c8176e969c295227982d2bf897927b40f501ff3c9a956eb768aa548ab278a7f0db4ea38dd75e2041473c6fe1aec55cd1cb0f4940c915cc089176b808f2440d" },
                { "el", "e88accc06316e7232e3f5f16b63c2d6be880c2cbaa89cd468847576db77be6109bd420b64082ba4500b643d6aff00aae225a5ce3ba6088ae37b64974be7a1b60" },
                { "en-CA", "2aae329f2bf5a5dd48c6705ccbafc904c8e275d2c135cd8b6dcea12a8db2ccf0d922d013cc4e7178b690ee945195895a818872351adc33beca02608ab361053d" },
                { "en-GB", "2f27b5c27eca7f4e2d6384807257cb52aba8431ed8caa3217e9a0d85ed21d50c792c3eab4fae91650a3230628677224263ca0520205f43e7003861062b8d1ae9" },
                { "en-US", "49a3d2e187e894a7aa23656b783c3be5e82196bd8394928e5fa5cb10ef60cf2d8c1b934208179a3035daf6d44a5f50405e683a2edaded97bbedf75e859c9f0af" },
                { "eo", "28c7d64ce13a203eb1d3208d95f43fe4c1adf98fc1f619222e8bd92ce4820360341d763434d8d034c67714c238464281065112edad09d09a1b330b9be0b1329c" },
                { "es-AR", "11beb591acc1fbbe011cf8fdbf3163c3aee6d37b0d1d0992ed9fe46c7476b4f65b6d03ff21cfff1490d645a8e19d6429290eb7446d2078e8523c081f12760a50" },
                { "es-CL", "d7ea6fb2c99cdd65e4d1adaa0d1158bdf03cc1c4fb68cefe386deade66d76506269c065ad89fba612b44cc7f737dad8e5a050c38834e8d940407dda7e5169590" },
                { "es-ES", "cbf01b31f7bf9dc41c5263884cc61fcb8cfb1a06477e02f6188edc0b08ef7ff1daf8ad1bb98fc2979df07b493db0a93ff1cd753439cb368d2008fefa8a372f34" },
                { "es-MX", "76b1d806d6a87491e5f29648ce3df76753b88f917b660476253b0213844555cc4db137481c44449483d2dc3ab9a950dfeea0f02cca9f3c07da70696a0c56e8dc" },
                { "et", "83f8d83a231b7c2bcc0852b84c52c9262af6d62f552f90a8f1d72856b74296b6fa86b4249a844764dbd227dab3c693452bef9188ab77d92bd8e65f2964a1545d" },
                { "eu", "48666c04311dbbefdf9bb1fbb8ab413fe0724200f0c80956eb327a46e5559941d3e568a7b281ca05eaf9aeeb080338952948fc058c7c9f3f652c98b67eabd4ce" },
                { "fa", "684fc8d6b3ebcac83666d21887afc53d9e2d97c1936a3f127498291242cd0731e18c6aba9adc1f45ed4118b22b240e00273215c5e9490792b4510314c938f789" },
                { "ff", "26837cc19d5418cc3e74a432d09bbb5d9d70af2e5c897692b3976280e0e54d72c4f88737e6b98039509023fa3b17cbd23aab40c2c60381c97d4da0b5bb3608b1" },
                { "fi", "a3f19972ac4aa1633c9b7553080594ebb24b45f025333b9e0aa7aaff5bc69162e2b556618179459da4d1dd9923085eb8774ce425b1407a8fc30489071f99830e" },
                { "fr", "0a6f7a1eb8bc67e2a725082acbdd6f81be0a6c5c13b7d2fe38922c5951d0fd5b9bd381f91e1a9339b51c23464883c2090e4eacb9adef3e2e1a0683e2f9392602" },
                { "fy-NL", "82a1992484b49b7044df36b6e61269584a59f09fc30f2d738c0e27c589b99ac6dc4437ac54cec2df33f6ea920e3ccf6b3f9472ccb0234ea10d6fdd04efbcf6be" },
                { "ga-IE", "1fb0c8775172d14e881363905d542ba25faeb6bcf0683b60c839add49373ebebd7c3410b674bd42ef1f5305fd3263875a19baf0f7cb83c79339864a2340d4ba2" },
                { "gd", "c09cff0046336678cbc71203595fbd372c4fbdf011e88a2616389e437410c5bfc3a8299b281258ab5ab8215433a320e0ff6c3459a9d33b8c6274249e870b47c8" },
                { "gl", "523721827d9bed4cdc5fa8d6d5d366a9500c25a6f79a490628d67aa005ebd32d3b1287fbd30a9bf1f89508123bffb039c0affbb261dfc43daabe17bdd236e2f3" },
                { "gn", "a8e746ba47be6e7e66e310f18c9388cbdc3101d92d5efc849fa4190e9640d15a60c0f98c434c29de38af4b8096ef8b3037a05f67dc22706484b0705366d2133c" },
                { "gu-IN", "d09703d9db6fe1c424aef10bc86f163f02d4ddeb1e6ad4d8543b64b35d69a5f646acd3a83c5d386b6f55a8df4ad2a7ad00059008946248e2b8daa91560491a2b" },
                { "he", "36f5432f47056048504704fbe0702deed669734bb829749d7453961bf528ff0fc12e0b4092857cf16cfd2249d10b67f31e92407876534e3ed4383e4399222c10" },
                { "hi-IN", "8cf3ee83fd798ca5ff6e5bfc5b754f25a2b8145be2676ac70e97a8c8c5e16ee97866f7e3eff8918974df56a7e414442d20f1bb2bdd16bd24faf70a67093b54d0" },
                { "hr", "a12af1c796b01eea6eda198659f56a1951dd513ffdf566f11804821f2758c50e72ce381902cad487669a623214107b4fc082c8e7ff86b5ced2fa6cca8c44fd77" },
                { "hsb", "d008f76e4514b865bf902c3fc67cb1a1d03988c7a882a380557384d3ba4aff15ea46d95ab9e441840ae6b03ed950405bdb493db4b8c568753cce239831acd010" },
                { "hu", "3033c7c59251937808bdea23b61709d2b626662ab18be7603f17f15d3507bf082025e93fb7eb6a3e6aeadd90385496770fc7c8ce27947feca78e0e0e2305fabd" },
                { "hy-AM", "227bd67cba552eb029036c51a3d619663c4a1b1419f60360d48bd782b94b6e47aea93c1bd87532b794481e22c22cc09e7a713a1da1dc02de98d7f66709db414e" },
                { "ia", "807f5845414b21d47c252d5a4b24c868c4f1ecc5ce955b71b2dd846d5a8131f91f67af3f23312e29e93c651e67267ce5d43541cba0d73647b1b95f5944c17210" },
                { "id", "9e657a1f5242fd53943c9e48be8f3393d94d3a19ee46af858dd26a3700b16717f084c1ba118ed91d0f6cbee7fadc2ec55fc90841752721d04ce4a209faa9f44f" },
                { "is", "eac57d2e51a4f2c21477c3260ed7b91c7a611fd2657ccbdf597d504eac0cbb9c36ca36ea12945711a2e948acb03121db1fab805a98c854a302f3859450de3943" },
                { "it", "a9d4f7d5535e9d147937357e060cdc21f7ba835a5eeb3ac87916df0747b8ba2a7bd6aca000e324552aee11a31421637d77b4184e014e18d2d62164e1756d9357" },
                { "ja", "f9d4e007bbd71db3c6339a243d8c7a9f560a5a384c75f430aa5a4c66fedf0c39e0e28ab61c5b6f3d10c642f366aef4059bdbd71675385d7779095bb0b0633027" },
                { "ka", "b47b59e8432244618336311e33a7326443c44482a5979dbdb6b3996e43cb181aba8bcb3f1772d4c5eb23a39cb6e936587cb94a82fde306a574d392098b6ef900" },
                { "kab", "9a0aeb5507c2903b8cadcaa0a1e0b7d30be1b50b997592d7d285b531b2608e7f6c30cd6d7c4d6a588dcdfbfcda222345a023b72da660c36dbe5bf85c99a1fbd8" },
                { "kk", "8354e23a99bbb86e39ff9e681ebd36a4ed4ac0afca0b27d1dfe34e5a4134e188aa983d7fd2e542027fef208efd7e15693e744506b941062e7f8db5395b30169e" },
                { "km", "61fca50be43e47f76dec5be401ac3d4bcecff966d600eaf4c40fb3f872c4e9120211363cdbd0288b5e69bebaab94dba8448d9f79df8b41cabfb669bedd199201" },
                { "kn", "f40cdda9b16525afb90bf605b6ed6baba1ea9b5caafc38c74f4d10f30d973cfa149aab74fae342f18ade3c01683d15aa81cb921578d72fb8642551d2feafd4c9" },
                { "ko", "5fed41945a255632c1ff227a0de36652ad04f8035a6711a8a5b947b22e3e2bb7591b9f40f1050cf661c23e3b878ba746786de0b82acf31f91bc5d32ff02acc58" },
                { "lij", "08d4fd822067a9c274bb647ad566bf02933707a6de39e593f97eeeb73204f61e08c8ffb93ff54ba65c8bc51ca61145f0d692984a2d9735afa8d813cc519469c2" },
                { "lt", "89e29adf5968db0965e8f8ea5a1fa3b22da68475800c24b4323d54d4916626bb546b6d200bd16a69faaf7ffdf0f48a945644083c1a5bcdeb55b0130a8466d0f4" },
                { "lv", "73cd93bed1a69dd610be37425e66f189891a94141aa79f32bf2cbe3c93f1a2597fdb1ab2c7d020988d549710c529e4a2aed56d37cfca4df204d42b6462a71dcc" },
                { "mk", "03196709b88b4075e9800e27f392945a3fdff1c8aaaf4ab629c43dcff7c38ec6a70349e9262b27a774a077ea1858b965fe9c4d01544cd09f54c90be63cf10d83" },
                { "mr", "255bda3026e317573f6203833db8264750f7ed0c33af3ac914f24eee298efb5a373b3592135257e71de79153f4c87dbaba5ceba161b979ad295f32e2e1a06f0b" },
                { "ms", "155e20171a86675684b49554395f19d44c7ffaa74df794ed1cc102aaa96d0fd0d114d1321a613d2d7508d4e512573c0e21eecb255da9b8dd42e152469fef7fd2" },
                { "my", "285941e555d9144abd892b19610b37d1f0cbb4079978acd9fb1e6739466e50fbf9083d83669ef23f279c7c7e8da5b74bff4beed7a83b712f592bc93119d99660" },
                { "nb-NO", "379895154f08a57b776ac925cddab6b56631bbf26f98f3639d6991c24f8859dc88b04d7c90ba062d0364f58e2554a87e904f50d82ec72c13515fa29b8a80d9eb" },
                { "ne-NP", "d7076e0ef1db220948e3fd7eb207f10705e3e26d083be4618b574fd1b91382a975e832ebbe0a821fa6d134858eb2ccdb5530f4b87bcf942c4ef94de0592bde9d" },
                { "nl", "f80f6162adb0b03ff16f458763814fcfef912644ec2f1560f96d76dc3f9eb04839a3e16346d1bc5b6edae68e32f034547462ee3a48cbe36e3215e23b24773825" },
                { "nn-NO", "899ad56f06030c01c96c6bd70c33c3045a96d9ff4c93a7ef3a8f57fe8dc01b7d28a0d538f0b5e3ef24f63e9c415ae702ea5416520985da5c9140dfcc8c1f7c8c" },
                { "oc", "1aec21be6d9e51ca88005bbc69070548ac59ed081e727d03a651306023e281ff97ba12027f7a456c8181b79e5da3f3e704e7d1b3fa290634c9b75316b0828ed7" },
                { "pa-IN", "2eb7832a45ea6fa3cc694c6431a5c63e2e251cc974744172d7e7e1004d72d14106687768146c64c8b80f1e97aa51d410aca7984d386dbd2dbf242a0f89cbde2d" },
                { "pl", "9f6df321fede85bfce987e305f8d950b3ee770f4d42bcb2a4dca875713ee85d2b95852b7f1120110ac5f4491248f70bc00acb098f9de8a3fffa043b009d6712e" },
                { "pt-BR", "447fa7e0b3796358a6b129117bee8fdeeee19ff90faad5543f7309abd4bd59e6c713b75cf4de6a450352099380c6861a59d0e99cd5f013191dc5ab454fba8508" },
                { "pt-PT", "71e63f09c4a0bb5023a70a692d489de1fd6bb7e799c0103eae8c9d85f61660a200bc58e17e7c23cdfba38e6ce8048011ce690433c9d90f6300334f3532f26c17" },
                { "rm", "5cfd5ae86337bfbda030c8b386f3eabf641c3b4f657b39edc0aa24cd86880b0f9377f30ca55850b74f7aa430246f7917355b05da4a9c1113ab1abfb8e4009132" },
                { "ro", "e70764dc061c4fd1a96d1f1bfbcb37615055ac3aed1ef4265046d345b8dc89e42f3ad7c3444a78793039954dc411015decb994b356242d8df767ced88fe1dec4" },
                { "ru", "ff07c59d8482007b387c89077e6a2a9799aaab962e05d5ddfff15d38f022295f0a3d6ca5b75a54322356e8b5a782a6f70e0a91c1699b1c9409caea69090357f6" },
                { "sco", "7f953646423cbd28ba44dd480d83b51417ef643cd56a04256b25b8df2c5ed39f8ef64a3969077b0d79c538d3e51effea3206d7100385cbf653b612cf0d883d4d" },
                { "si", "0ec9eb5c54c518a849bb6aad4cf935bc03ebe05b63da0d1b519d7c9fe15f78cad1202d439c1e3b7fb10148e6068f9c1acf519d5d78fa4d03aceadbdc1bf33ad1" },
                { "sk", "a8d1046983a4904806180de08c62e1bace929328d777ce4cfd2e5a1004a83ebd3956979ac81808583f218d80d7281df36f84202a5e62707039e01ea9ed11535f" },
                { "sl", "dc6f82bf1d5723dad5f2f9978490cdfcb4be3e925187abafae7f9cd3ffacc1b5a4034aa1a8edaa0749e0ae189c99c0686382f1ca1d4a001b2ef1af954a05d46d" },
                { "son", "da6b7b9eae1c717c6aa22c041efc4a1085f73f92b58237604712d8f2a47aa8202f01732d3e7f521e3dd3ba1b1d5d92366540483801f245cabaeefd50d7f62787" },
                { "sq", "0bf40201bff92dc700acf3e2fb3facb5d4bca8faf7016c824c1100132ed362569790a002ace6c9a8ecd9e75715381eea83c63df9abccd7e6f6ff4dbb85ca6fe8" },
                { "sr", "41ce2a3dcabfeee146641df58c1ef870d8015321bee287c50ca4057b4dcf7a1a36e1b5888b0998c66801ca1440778160a0855ec815116e719de95ab97e52cea9" },
                { "sv-SE", "fe5ede3e4a48c1b285326ddd3f6d18251b2d1050c7e47fd6dc623448e44decbc526f8bbffe842cb190840fd561c36145880c6ad2ab0df3fc63398c4afc141dbd" },
                { "szl", "82cf26570abd1f84e5376cb2fe20de8e080f0f5cb009fcd8fb0847ff65fdd7b639e037de1c1e0ac7a3c8f9a8aaf88df3e371e1651048ff9c9e02f8a7c869c9e4" },
                { "ta", "b8b47ea86b57dedc5edd91942bb194a106be60963ffe17fb481819dd916b65e6801706946a4a360710be26e80a966f1e6243b05fdb780071c39e68d454ea6acb" },
                { "te", "6d6ecc4eb68046b9264246aad0c98f5b5b74f392e1320318435cc9e5e6a583e5718e2aa8b828c56b754705836e1caf637a635afeab233254f9e848b8403ddd3d" },
                { "th", "6a1de23356a621d79ff6e49f0e143a2d2c78beb73bf2e1118d2d043155105ad8553242bc1447710285c10a6740bc4268b85adf0baa0a374da26a4eab769a07c7" },
                { "tl", "1aae4b5a67ded9585cb3e3d755cf1aa8e438166697df32cccb63854bf3447460b21c81408815fa2689e502db4397e6e316557280ccedee5d8aa2683b313bb6ba" },
                { "tr", "84c9afd347fbe9ef02f302810a3485bbb71116ec25bbc0248c8cd31c9e34e463d125a8bd781aaab0cf51a725242563a095de2bfaf38e4cf262fa861030a68b15" },
                { "trs", "3d57b72b825a9b69e79849d74088ee176165a35d3b72c26b8cdf3b98db855c4a342b7aa7630b94ce5cdae327d3594655f241e5ee0ed3ffa4c013f740fb73d521" },
                { "uk", "54ea92d11c62162d562647831f30d4c7501e6063fc6a64062206086b2c76711571f44865c18c27b724899f5266c5564029a5664f980664ba3d9261a21587b6e6" },
                { "ur", "e8579870d94e6d865da566f1da94ca9b36ae2f7689dfebcbd9dcbb3be7e5dcba8fe9441e659012e9ca3d5955914d642d4e656a398912d4362a9bfc925f5c8335" },
                { "uz", "92b51eddd485fcc78a78802228d13458f7764cbfe69867efec9c8f758d8a6971931fefc29208a57ef923f0b546d059dd3820e1b4ac4eed7a3fd66cae5831058a" },
                { "vi", "ca73222c2de70a7a5a23f8cf23e8331f30baf975671242ae0ab149a8694f56e6b2c7ac7b8a9014580d34f187b85a51d348e0833fd69813c52d23a8387551cbbd" },
                { "xh", "5723aeb5e06422db78b8fbd98a3ab4e5bc503806f91fe33a29c59dceff1b0212add127e220b0e206dbfdc9ca721e1e06d17986e0a3278d10c92ac1dc2962a022" },
                { "zh-CN", "2c578621c41278e5600c7d9de1d88d8bb05a933112e73140569d2054a08e5321b112f76301842c1449c2ad15cdc4d23d42a4d82d3d1f7727b062e440c8912313" },
                { "zh-TW", "12c83d07f2a4f7ca4e0f78bb3af06d352494f61b8658ca464262d5430f0f3e2156a4407b29b3744a25760d092bbfca3b6d34fbd20cccdc0f834d44f14ccd766f" }
            };
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            var d = knownChecksums32Bit();
            return d.Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            const string knownVersion = "109.0";
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Mozilla Firefox (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
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
            return new string[] { "firefox", "firefox-" + languageCode.ToLower() };
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=firefox-latest&os=win&lang=" + languageCode;
            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };
            var client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
            try
            {
                var task = client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
                task.Wait();
                var response = task.Result;
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers.Location?.ToString();
                response = null;
                client = null;
                var reVersion = new Regex("[0-9]{2,3}\\.[0-9](\\.[0-9])?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;

                return currentVersion;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox version: " + ex.Message);
                return null;
            }
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
             * https://ftp.mozilla.org/pub/firefox/releases/51.0.1/SHA512SUMS
             * Common lines look like
             * "02324d3a...9e53  win64/en-GB/Firefox Setup 51.0.1.exe"
             */

            string url = "https://ftp.mozilla.org/pub/firefox/releases/" + newerVersion + "/SHA512SUMS";
            string sha512SumsContent;
            var client = HttpClientProvider.Provide();
            try
            {
                var task = client.GetStringAsync(url);
                task.Wait();
                sha512SumsContent = task.Result;
            }
            catch (Exception ex)
            {
                logger.Warn("Exception occurred while checking for newer version of Firefox: " + ex.Message);
                return null;
            }

            // look for line with the correct language code and version for 32 bit
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // checksum is the first 128 characters of the match
            return new string[] { matchChecksum32Bit.Value[..128], matchChecksum64Bit.Value[..128] };
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
            logger.Info("Searcing for newer version of Firefox...");
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
                // failure occurred
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
        /// language code for the Firefox ESR version
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
    } // class
} // namespace
