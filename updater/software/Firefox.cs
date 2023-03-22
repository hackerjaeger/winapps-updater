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
            // https://ftp.mozilla.org/pub/firefox/releases/111.0.1/SHA512SUMS
            return new Dictionary<string, string>(99)
            {
                { "ach", "02e3483b11e0b8bda52357d12ee0bbf02fa3c0dde2c50ab8891e76a67b738db27cd116a9e1a3b54a1a8a33bac215982c11fdd7ab07e8265c866165e062d7fa2f" },
                { "af", "f299e02d8020a02e88348461083077111590108d7dc7c7f2dc9f2aaacdb48c47ef190817aae7c3f4dcce19ac157fb4e7fb03d2850606ce0dbc597b66f134b6e9" },
                { "an", "00553ee53ed23be51028218ab18dd957f71ed1911cf801ce0f11bb398b3a6e9ac58017a61d0a15ddb7447da577665e83b52c8677c81ad05f60d2a12bb575fee8" },
                { "ar", "0d0eea3d419898d729b2993808e3e8e5f4b634459996bc589fadc782c12cec8ca667eeb3b2a74882fb89a69854d82cecda2c27ea35750be613f4574f8d6cac61" },
                { "ast", "250713f6a046c1eaf8fc0331c12a9c8ab97159a00a18c242cfebf68b97875d7fb879a8f5d0f3d875f0f9e029b46f3361315da3bcabdaf2fac9ab55f4842be275" },
                { "az", "01316302ac3a2ea2b6b518fa44c804fb1f48f870f122619eaa544417a9f9882b17113b58fb3a48e2793b825ee65fca60aa59a733e5cd190626b78a09e19aa70d" },
                { "be", "46e88defc6206fd500fec922f127f0ba98344a4ea43bb0a0de4eb3fbcb5b334d4233cc93cef441e310b97fca54b4c1e51f4d4398656be9ba7e3ef8e74054cf72" },
                { "bg", "a67191d84bdf4db1b27097660be478604bd07fd56376a77964fa866371693ffaf784503e5a41cd5a2f80cd7db67619e8d4f9f66c44d3601cf8c04a92b3ebc0ec" },
                { "bn", "7ad922b3f0d47ab607def2e7917a8a85cecc97cdc81579131e69b9bad07d441a2fe7cf45dafdc8a9166a6253c2649d2edd0a8fea6e87cc02d26c904a5c48933a" },
                { "br", "7b28170fe7173293d058ce9bd7d56da7982ed023f92b831437eb498101d01e232dd12015a30ce03e6d3feba48053c35b66b508ea6fb47269c24c0f8b7ea484e4" },
                { "bs", "af0a8aa48100863d9b7c48cebdeffb27f07b804c69347a5aa7111657e82239c97052f4816a883f1a001d9847603ab8b6cd0d21285e47a29feff1170e105f9a36" },
                { "ca", "43f36fadaaf3d19647bc62307ec5e0f06b4f74ae531027b7e2a9791d6f85a975d85aa4565792231153dfb8418fb6e6d68b948261494776bffc2084cabc60cab4" },
                { "cak", "c0eabd8ea5fb9d12a4d117c0c11867b5087a1664813ab677e6f1fc279717fb82ecc9d4a36a7bca2b55875a1a9d13ba644c36dcdaf9c2fd65fc1fe8816c9b0d58" },
                { "cs", "8525e9de6eb4cf2fffb91298f17394bf97c40b30b0dda3252efb2658f3fe974cbd4b287d883ab5d5f2eda94fe16deb5794f3bb6d68db438cc5d0a5b8c12c62d7" },
                { "cy", "0412e54aff3f437e443d4124bd5a2390f17f2438cec6f5b100c2feb43772cd046d882ed6714b609006784cedcd41428f65444ebea74dbe812144cf8666d1c801" },
                { "da", "7ead33cd9ff28f4fd566fd97c22e3305921d1782fb978bc3cfea30b5ac9928e046fd191247e2bcb0d4808a47f397b324bfc12c9ce35631cf78584383f359d690" },
                { "de", "419ae2698bf1c2fbcb860c26ba2c4f0ab32896b5c3c9faddcc2ac645ddd34878a4b0ba755daca448aa011f2c963a2c86dc8c91f35e112c9b93e47f907f0dcc01" },
                { "dsb", "4718edcc306e0b7188c411cde654c58697e5a93df50bd4d75b80e4f16b5101649038c4378faffb887bd5b197061bd04cea0c97ba92a073da844c9aa07b2685f6" },
                { "el", "2b2e549e10097027c019db83894b9fc931123e7301218de89614724593c9862f0795ee0c8c6f0dcb848a5b6429fcf66a15720d122c468825b58fbe06bc14a3ec" },
                { "en-CA", "6e1acf7f5c4ad74b2bfabc10b465bc1f249f42f93ce84affa6b05abdbe49caf5d3a6a4542e7330bf17dc02f8f503603d97753836b62b078a9b351213f91d6833" },
                { "en-GB", "92a07840a6fd5353f8b1cf7f81a977c8122e9bdb1267ef9601d2477453b8c3552b30b95d477d6250c740f5a0b99ac40dc07b3476db7c13476272b6b84c4e1a83" },
                { "en-US", "fe163265fb8d210ee865cd7ebc11e2948db5b609227e43bf47043833b13902678bf72a80d607e0aa3f4fbe2954beb7814306560be4f66927ebb3566758d2fda5" },
                { "eo", "818b7896a43478303afc69491a1d480b9fa251ec31fb10dfc1167be73822a193d5ba59023a3a4cf682996dc48165eeba34bb2aea25babe911bd38b17e3d15275" },
                { "es-AR", "a1eefba8b8ef284012e4b3c92f32e4ce18cc97d8ace6653196729a6d1d8a25cca9a4acbfb6f7c4d8eb1d354947e8fb97e937ff3a73e49c816c189c8be112f3fe" },
                { "es-CL", "28465c33ab5aea41b662ee1d9480a441a960f2c4afd2af04e18c4195c49c0185594af5b843153a141b779b6bf4c46c67b661403661735170e5b0cb1898c2691a" },
                { "es-ES", "499db7183fc0e17f86c2717bae08f10c85b9d087a8c37941c0727b8fc8cea369843db5861954a703f7f49fc198b8610b6ef891b5617390c27c99fe1cd79ee202" },
                { "es-MX", "8f16ed78535742603da5f4736cfa3b0bd90b8d068fb49d93698eac5422fefc54d03cce527d3cfdae25ddbb550a2bf2f215690e5b52b909623e978496f3bcbd98" },
                { "et", "563951e4c3968267ed3d391daf7529250d9f159b214c6690c68a2cd0424f075086f40f0574bcff52e328a9d1edc1968a7565ef8680b5199150200a29d40215f5" },
                { "eu", "ebaacc0040839289cdad8c77f2df07a40ac0eb5e1d40e6a4df3ba9e94fb111fff34fe08471b6a40b83ccc5833ae07af978e4d94d325154e40be4dbc218e2e276" },
                { "fa", "5479ae427e41780232c695025ef8f32cbe94a4350f61b48737ecfe569bffc0666922ca4ffededb3d8159f7f34de6648035df53b687fbbfa9af1360ee040d39f4" },
                { "ff", "eb1bc5c6a4666dd5f7751e87f94eb8039627b5a08eb5fedc3e7bae8683505016152088afbd52f247590be45a3d5182c1a0ddf7250f102e1db0fca736701a9ed3" },
                { "fi", "734166bf3cb329f79fa1b24189cf1489f62b2e25cbc5333917ee933f5a485faf36e89a8f0f8afbd751892024b5781fe1e9067f08b062826437df0d6e60af4251" },
                { "fr", "5080d6493b254573b3f40fb6716f39924b2044f463d5cdc6c060b41fec238dd50388760b6ec174c283dfe3624df2b6a66171c5ff524f04b13c9a2f97bb6b6601" },
                { "fur", "0543ab785d3b17887a65e1a6ddf1592be6df0161f37d5a9ca9b7504853b04870c6ea547046744e0cbd9bfe10c1444ae4fe7ed44917628365ff9ff1f381121478" },
                { "fy-NL", "c8cc1857ac882a2f6e615686cf0e61b4fac1174d2968a58352902c6266672a36e34f63da588a013c8f5a5ad93546661aeb29bc7f4033a68ca2884714680e0e0f" },
                { "ga-IE", "29043c6e6c40a400c1e3f6f8a133d3efd79c3446d92c7e0517e9b796a7a52251977b4dbc15c63ba769a9b11bfa3cbdda45e24c7fe06cd589a2cd86fda2fc5778" },
                { "gd", "4495df187ce67612696e8ceb839cce941e5345a6066ff255566f50caccf9ed1dad93b864692c52a56dc84208b189886f1a63da93413e37f2c0b859db3b50da32" },
                { "gl", "da612029a74057a07dc7e1bcd14793d395840175a773cf1583d33e65d91da7fa4b3642127ffa54506f291472714f795724702f8d4686bd44f5627c544c632d85" },
                { "gn", "045076a54440e6f5130e954660b0388793e1ad84e7e6398612c1b7927d6aabdcfba6e3a842dee2fcadeaac018b8d6e23a9eca1693438c1baad2b542be6fbca2c" },
                { "gu-IN", "e3b9ecb2d13f47ac767f283a34cc4002a3936e0ef7fad17103d0a614d5c9c13e5f7dfc1e871520e6f5d277eb057ac070eb349fb4e504f0c349506ebedfee57af" },
                { "he", "8170bcf21392f8c361999147a60d7b20613d28a70bf4bb0767a69e9e311791b1612f498eb9e7ad6cbad8f28de689310f83b7e94561e4c49a57f587018dfae0bb" },
                { "hi-IN", "a6eb4e9a5e2c2f9ef79f60685cde51545fca8608a30518680e9c3f5bc9440857aeb67b7d371a189dc0f5e2d1e63b89dce4de0566568b7af332bbf44330e1a166" },
                { "hr", "172e2e4a2b01996564618b11a7c5e4a27c27f895d01e0e086a69f7053b4a19c99960ae968d54ec30b1196a9a8223db13bd9090559ee72490a757ee695e6d9366" },
                { "hsb", "c0a49c0d6bc4c3c1036a9ce054a07c10aa66b32ab08457045631326ec86420fe1a2a2a6a8f05c5cb7fceb974026bd3812dcb0288a881216dbba4f760f9dc945b" },
                { "hu", "1fc0323ad017a5d59409e34bc440a2db6bbfced30058dd851e4ff14ada7eecee9a08438a1629791b1978496fbda90459b77cee1306c71c2ba1c6b2826d64f6ec" },
                { "hy-AM", "d33596aa858b68ef9cf68e4ab1bc09d6236304f0f1a6c78d85968925638ac4fe59e33e6a5d114a83509b9c2a6c271d3ff56a5f24e129308bb0b1113757e2b30c" },
                { "ia", "675280419341ea94077c9b989321a63956963ae2f77e265815b1500b0a292d6507ef50bb05e5118e2e8a03705e4ed49302207c4a6d9979f1d44956e02d3a6f79" },
                { "id", "229befba4b1e2740f4795531bde03f791f3792b6a724c7cc949e8423190938bc52036e780695063f22c01dc19bad14cecb50eef03fb5bfabffa8d581abecc93e" },
                { "is", "97b8e93f61f8f165433f44629681961dfd268f415805fa380c260d83387950e8cd2aa29947bc468a50db1d7c60644d09c292a0651df08bc4896cb5f3e4b8e7a4" },
                { "it", "459e2343341197b531c130ec77cb87ef3fee8c13f66fed79e1760ab22fed8b30cfd6dc94582447a316b0afa171929473a3a22b59e81e7f4cb7b307375c9a2594" },
                { "ja", "0458e78c97506752fcba1e82b1255d98bb056990b8b88024f4c352aa98c3e95eec53ce7885a5cefe2ea8c8425ea8ad2aadc77f8c3bc06e673725fef546c61b9c" },
                { "ka", "be7c37a1afa451c8336bb725c7341623c89402bf8d09d579b6965a74a4431363c48487f47f1a49d1b4b7a5fe5563e1002e634f71f2fb8dd4b77afa3415ba28d2" },
                { "kab", "f4c92c22a69ddaeb942fb2299a78c4e662ae898e7f8adbbe37d8acfea56688de0ef3840df237f90fee7a4e280c612987f7d1afc9376a04f343d0512f35d04ffd" },
                { "kk", "3ea6b3b791a67e585b5290d7df4e78216fdc5398060d91249a9620efaba40648c47ccf08ef7a7c555ea3e80d7e20a436d9d43f6739b93dc94622bb1e6921c22a" },
                { "km", "9be71cf3d85e1a8db43f459ce1814c980b6700856a5cb7d16c5499fc22448865ba56b9383b278c18f52218055187561fbb0de955b75939184d70585b8879ec72" },
                { "kn", "35a3e72cc3a69b54a1c3f407bb7f237ec9b5c01787aa13fa4a87aa58585548c0c557dca953e5b2662c8282d505f74d4513b593fed23395164ef0a890e0f75b5c" },
                { "ko", "adc6a19698080c842b84ef296cebeed7b0ccd34248bc50106d4a69666b2516105ca6fe978c6d0f455d5c9bc445aea80b33e5116455f3427ad4810b5c341a894d" },
                { "lij", "0f901e0962b30e2fd8a198255b7f36dae12f2adb87de7de71b545a6d7a6b5f529d28cdb7c1d63df1c95d90ade5ca3cbad9d08381d8c13f9481724e41994d12c8" },
                { "lt", "33e49101157ff635cf1d68bb60b6a1c4c9a19b61bfce592434451c2198db3cdc4e74502077e49d901692f9610856833ea3f5296794672da7f3b8180aa86c2b2d" },
                { "lv", "caeaa3130df42b4043cb16fc9c401792a08b39be10a5a3fe450bbbc7b463ac37ef8f1d1b92ce15076f54da59fda89604cb36cce8566e983b8a5f87a8ba26b32e" },
                { "mk", "c6e304f73882c9cb51b3756480ad58a962d8a19eabaad94a8dbeded7d1b82a49b7c3c8bba553e2d325ec9f23fe19756056031f8b563079698c682ef9da0a7cab" },
                { "mr", "1e269fb8cc2053d19a5b61f7a46466f2bdd4e8a547b77ecb8d4fdfe7143a28502831da24f44e0ef335222b5cd6d952b2ac9c186b93028afeec968a7d7d09e44a" },
                { "ms", "67fa38f6d57d60ed10d8cce7ec7bea26157295970801d7de931441ebb45d8bb67b9d6cc46e7366d56990ee28d23f600f7fb9f75fc94c9167c7ba22c8be0715e6" },
                { "my", "6a9c8b32e502b503b354387d02d3bb1fcd2be761dde15904dbd546547f8416b6c1e25257bd53b729a2bcbfc837e3abf99d073411205f5745f9fd351752df0de3" },
                { "nb-NO", "3be4d134537f0b654b675c9906f4e6fc730c16ba8cdaee868342db3e77dd940211a7be6ae444cd2476138b22560bbdb87ca15d0eb876c77b4004bf9896e8e19a" },
                { "ne-NP", "0cbcb5d7ef7e2880e2050c33d3b60dbfb2ba99bb14c632a623836753f10bfcfd2a8e5f1a3dd47a59f555a6861a970e70837f987a7c0d88676bc58ce595875c68" },
                { "nl", "fd2779950496372a11d76d58f6a6efcea2f83a87d9c3afe6633180462fb45d3396a88085e2d9769948ba84f398178ca12d93f43892bfb0f16b37fc98191c260c" },
                { "nn-NO", "d1c77769694c33410547d3d84ff8def93193769266e6b420a319aa27b3c1e48b594faedf060a417922be611b2fe2fa18618d8579cc30039f2df40ea4cca6faf8" },
                { "oc", "380def09c1e9e1375c3a1c95b32de1761c4104db6af60152be31350d170b7799a4f587d2cfdb4f8ba864b9a5a5fda7a70d84562c8142a8e4909549c6b984ca73" },
                { "pa-IN", "309d2a0284a24eb1755996b7df6e078d1503b2a9d5a01e226896b801e73a1394302abd6450145233e56dad6b8ea13b7fcffcb341a992ae913231127e04ce319f" },
                { "pl", "6460e8f09acba9fb5c198c1faeba2745239514ffe0b5f448f29aa6e18130682c0950c8d081be1b86888e7e14720b8497fba519cd4c0333d277c0af5f4237feef" },
                { "pt-BR", "672a2154f73fe5fa754bcca10728a594c36c8cab11082b6551966ffd3e91663b04c1f2b1767546a4ff5749a16d286b7bfbead5746fbdf319aa93d42bc0d12999" },
                { "pt-PT", "62ffa728a6f7bc7b465f9c9ab0818af54f15233f90497cd45459a610078348f4e29c1fa5aade71ac0f1fcb282aa769f472f840858eeab5c8a7a4fcb003885780" },
                { "rm", "b618a905c1aeab4a9541bcf9417e7ea2f8cec9faab0c22a8517a38ed82abb669a970a920a2bca697e0785e6c53e24e784002f2183ac7c9284550407f92cbec05" },
                { "ro", "ec393dd193067b307d7c57960e5c4a8438e2f082612027b58b8d94da069d6f04e834906b6344af4d9e112ec3bd8565860766cdfa0629532ae566bca079696787" },
                { "ru", "34fe918730577918386ba2a37f73572d2434f4ab5e9b52ec7c3e3edaa82078d635fb302a08f11495a6e99a7219aec54c563204bbb9fd98dff5c7f5e593528f73" },
                { "sc", "0d4ccf0676c11eb734a4e6512324ba38fc6e54b8fdf554c1bc29fc1d63a567559a35062b7c06bc4561e61c90c831094c19ed4307ec3e479e6db38dc083aab00c" },
                { "sco", "4767d04c034edded2b714e3e403b5a21a3154ccc05c2347b0ca27609ce501a37942a376b7ce027c56e42dfe58d4d471b315cc55ea48036f067d33b131c2ba87f" },
                { "si", "7dc494c24acaa550bad2902c74ccebdebb064731beab884dfa841a6c81b5ffbbdbd9ecff00452187a29cd43a025daf6684af5bcb7470d0341ba7f72cc2744c68" },
                { "sk", "69f1876c2a75eff7dd4a92656d86e52fa25dbb97451f605e20f437660f32398bd40ded0878ab0e6dbea98db088f5747becfb73fba1eca0f91e6559c7ce31832e" },
                { "sl", "7717b7af27c89c8d06c74463573ed94da1fa2dd123da0607a86394a088ff7cded9cc274ac7c513cdde39958b3dd9b1aabbc84e0c248be4f3d13c2e74d7e52856" },
                { "son", "3985cd88b4885e90bd2255bbda6f352490114ffe64ad589b60edcabb4aaff39952b6219a34685e53dd110822578439eaf320e38464b9b9d9eea44a5972580dfa" },
                { "sq", "71a85414a5e2b54b20ce82cb7098ed54ed09083589a498067fe2c51a02bd6c88072acdd20aff423a8c0c528af6d894eeeb400d0f375e84fa478f6c2c586afb23" },
                { "sr", "c6588d177fb55170a311e666913bd8ab99f5d5821ffc24da17c3ca7120672a8a402a4acf7d4348cbaf4bf5518d30b8b6a84e3222d3476bba5444702469b9bbb4" },
                { "sv-SE", "8e86431aac6f9e86ea1ae25c30ca134934711a4f29c88b8bd8fac7dc57a6db04c444f1609ec75dfaaacff1d2a5f347751e04d565758ffc24983e531287254ee2" },
                { "szl", "eba37cf425eda7d5ee8951b4dba4b9010e811a32ccfd977167a1d44eec4911a386db81f2d9c937e84cc579e2ba7ffaed999d09d3c8cc1a8cda62645b06d8b26d" },
                { "ta", "2a099f72bae2e6d11e19dbae428f32d08103ba83c0adec7046b629e2593d06e2a139e76730bfe931571485a3d1edef03b4a19c3d795c4a0ab9dc384e2d77ac8a" },
                { "te", "09555987af9a0d73fb00c941f2b636df8cd273828138f5beb7089046a8616ea808789dc729e16e1a2493ceeb78fa5fd054efaca80ed072d2d6942af06ac39fe5" },
                { "th", "56fd8dfa2bbd420bd8945a67a6faa5d48af994cf67c849c8deffb4920639e2d83b18c1eb4daa7e6a55011a60c3abdf5565c535eef7051c57caecd37108663b64" },
                { "tl", "aebd7bce2fdd3ed333dcbd5f033f2944a165ead6af068918223b833b2a7e9623c51e9d11384f967a99e40183fe307c9a73c8e86962c6ab2665c0edd0db04f10f" },
                { "tr", "85589eda5fecd3ae74f8fe4de6d18a60ba2b6ad4a77f001f085ab6d9907a8df8f62738133cd02fbcf1d9594ffb8ba22a274da82420225dc6f604f79bfb36a37d" },
                { "trs", "78ffe8bb479e3b6321b1f99d6a7dc88fbb6e82c15614ab4dcf8c04acaa6d83487e1e2f759659bdba5db9bc234889498d151219b884555d1a909d22f329a90c0d" },
                { "uk", "fcafc01b7cadc5a6bd5d2522e37d6db1966fa26c4ec2a7e3d4a450f0004ad4169982f8bc64bb5e7f0926f49674ca0f7db792db2c199ed671076ff7113f2f892c" },
                { "ur", "0f8f1a48908f5806c704038c4cbc48c1feb5150684ca22c917e33bba9779f13d51103948a330fa2367de3424539c06e608179aa1aa445bc039a76e533e8096e5" },
                { "uz", "92d9aa91cd6248f1a30b3a7b083835b94ccc5cb3eb0a90e14e55a81d3cc5256c86609187de5d36637f2c553253f0501f181abc01f49f945d91b00849aacb7aec" },
                { "vi", "254f984a8315ac0441fc874e36aae905874c2d32435f2db2efe944db131deb35a6b8668ac1e92d4f54fc3beedeabde6c8d74bd00e74f38dbf0ce5210b054e3b2" },
                { "xh", "4656840a0952abcaae4b5adcf09167db7e751266bafa22b6c6b2d67e07e136456eeb24e4e28050361d9a28a5eaf827ab67f60764e69890e565fef1e92dd4c9ed" },
                { "zh-CN", "b1be7f37201d0aef1db0a2bd887dc5e93b1491edf7f56d9581e8734d88bc6787b3fbda46c685d4552ee146027017ac322399a25df036bd9a51f8858586da7571" },
                { "zh-TW", "5f18a0676f7dca2cbc312e36bd7422522459a112c420f28cdbb68b526a54560c4af99378042f881897cb4090a855ef5f1dfe23dd3163b0fdfbb44cc086c50e85" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/111.0.1/SHA512SUMS
            return new Dictionary<string, string>(99)
            {
                { "ach", "cfaeb80f10d7513e804adaa85ff50e74ed3b3821fe27563763c83ea777a369ed82cc32cff0b9fe93936f6236ed3824e12e1892e48c397b70695acb8f3e4d1ede" },
                { "af", "599e3ed8298e0b795f78b51c8d1dc92d17075aaae1a34467d492c6aa88cd44a8777d3ae196fff7ee40cfdb7b9f3d1309a7ea6dac8b238e6c497650e25ec81c05" },
                { "an", "d91de31546398a99ffa629bee403852d7c03f543b8d00cac549a201409bb51c12d88402917bc1496f0ea006740f7e3d0106a5141cc659a95e7073d93545dbdef" },
                { "ar", "18cfb555374489bc3170260a8d0bd0f642f94a267ae2460040aff22ca039d48cf09dcf4dfa62573fa85f980f81601dffd742b22edba414ef52ae7253c1454b6f" },
                { "ast", "a99e2da554b463154176386ddf3b876cb4925637782595b685c7e29b9f48c55ff694ee3980aebb2487da48415aa0bd1aa898ba343746ce5f455aa523f12691d1" },
                { "az", "34e54db9a1c699d7d765f667969aee9eb60a4835cfc940c9275815feb7d2e600e1fd634f4e4a43df0afd85d1956e60ede206dc2b946359f0cfce12977ad1a1f3" },
                { "be", "494dcf6e5420b573b32fe054ccf9fddba2636814f6671980195f301efa58197c1fa4433fbb8b151fe643b35754103bd4a90fe48bed1f69e5a619eab43085782d" },
                { "bg", "b6ef6b85ce537cab04c88c9295b5f9f26b75a56ac147e8340df775f3142fc178516a723958683e9a0ab2148f9a268a369fb623c9464463ad1e6c9c33431deea4" },
                { "bn", "8a56a396471bce0ec8968d7adcf3b4b374642963bf2b9492e9e04da3d2672e8ae7da1ff04a22a31edaa55560f742b90928fe7518fbce6683e13c33f359f865b7" },
                { "br", "501b9d1f30be14c4b439d286cdc34fbdd85d264606c3346a061ec623afd9db50858d87fd2d8e242b1f52c62694e4a1c1e9a1bf53a98e678152efb58a27df6cda" },
                { "bs", "ea5f384521bbab682d1a064fd58a38e15816e5883c88584c4c10c77a862804435971bf2552ff08d0e5d01bab003b5dfd751d5a1b3f8b91cc97f97cbc9de2abf7" },
                { "ca", "954b745bc12502f2a9be4ff4db59e98f0a1d02f96f62ecf3b786e960bc47e967b9e48e573d9c8ae5593aadc4ee00fccf0ce6a819b929919e84a3075db7a81d1f" },
                { "cak", "44cbef4af9f4330feff8c6272458fdf6639a6f9653fbe3510d3939873b474b9d1321a30a245bba3a3b5ca274a9f4e39c9c62d2ef718c552c2d8da87f7e5b47cb" },
                { "cs", "8c5fc637bbbfaf3cf2e373ce1a7c4a1a7d451b2927f999c5e8823fb380e6744b6aa0c2834122627d4a8665c27d02575bb1d6993d6aeb9d09569c35cf8822da6b" },
                { "cy", "4c78365abf43336174e73c7819bbce3005456888dd17ba319b538b9cfeff56e8e80e4528bdf1a7f69ca0424f6be5d6886450780c9dfba8cc002cf3c89858effd" },
                { "da", "bb754317eff294189f996231050b997980efcaa99710089c093244ba3f99ebc7cbe86325681e2470a69f70ca79bb0d0dcc38b4360cdcc6104e084771fba43987" },
                { "de", "22e0bec20318f35b6017e8cde17c983272d069fc12c4ff8f1ca868586dde55625dc968b949b176b4e7aaa4c9f0986081cc60f6452b146732fdc7e537a03209db" },
                { "dsb", "b21dd3627471b709ab9b7b82ea000f82f56ac86dded265c57eb26a9353cbdf2f4ef6219d2b4bf8cd9615f23cfe3b75fb3da6dea1cde8bb7fe1e20b5e947887a0" },
                { "el", "5c76b03ee1719b8cb54df144d3044fe4bac22e7e186eb23e1bdf57f5187c0cf902c6c8254b3fb450b364e70bb1181f3db5a9d12e7f8a374c56a1f404c848c5c7" },
                { "en-CA", "e9b38b12de931b9ea073bf8b1261f34942dc74f0d4485f534cced6dce20c213ab79a341a9cfdce1fdb1b8328e4f9ed3c66a2c87956e0a68b7f0a4d20306cad00" },
                { "en-GB", "1a56ddddf8e26113622c30d70f294cfd3c344ef0b2b54fb79354147b98def4f84d10ff37f5c85d26cd166f9ca2a5a4026dea8070f7af036fff597e9c89d8a676" },
                { "en-US", "cefb1effafead9834ab85a923a511333a7f518529a1d3e7dc71510e09635684afb1b1816e1fb905e105bba3eae5fc02969827938ba2497cc7575a056c98f0422" },
                { "eo", "f55937b7f1a6536b0e534f0ada5ab332422ee50ffbb9ae01708d16e2cada7ed7f886ef1619dde2685b1c54d7e2a61e4797eebe09099c5fd6d3b62560eb8bec45" },
                { "es-AR", "fec0dca03813ad8711aa8860296c1106ffe99c80af5d11d83c98836ec7b777ca3ed6cad4fcfeb2cedf21dbee1f833cab61537a66acedb44790fdac638ffcb0b1" },
                { "es-CL", "70be5714c7c83bddc8c85317b6b3142f21695b6e83da39392a63e91b30f4d2dcea324bebaef1341b13642de3da31050c1e458975a13874e48bdc5ffda5f1d47d" },
                { "es-ES", "c049b9beaeca6e81438112d68f8a2ed8f29a2683c0c04b146aea1fc4276226b195c9c328d9e308d32aecbc258c34cad489cb8d2c455bd7305c3a7c51318fd50f" },
                { "es-MX", "7eb6e5b455925bccde37818472a4b439f3332f7158a2c32caa73eecc7c944a02e26fdbb6ea86a3aa6684bb87cc37698e778248fda827301cc3dca65255793d2f" },
                { "et", "f488a6fb99288d70048bc755e8d104a77f1d62d6a29103cc9e6a873771e7801723c93d5519946194a31cf025d3829cd39290fb35f146b6781fe3f9759a8f35a5" },
                { "eu", "2f49668d33fcc92e1f173d6c6fdd3803e743b717205664c88c9ceb2004aea8b9b0c7baffeaaa5ca6ff7d328e67bd34da211aa6d1bdd31983f30e6367a848755b" },
                { "fa", "30a1d152a97199d388f8bdf339e712eeb2b279a082aa4855bd4836225fa3424572999d3f48aadd23c9ebfaf2665583130487ae77dcd5a502bb7a35b700176f54" },
                { "ff", "94f6a098552475e1da14097ef65f1651ddce54636f9922a52fd2ee054ab7a77571b53fb460611fa99cec29689da6604aa739cb193c26a05e7ccc631743b551ba" },
                { "fi", "f9dee7214866bea446cd88ce01d753d0065ae839e45e2c0dbe59a1f4e776fdc6b921b91d1a30e4c0d32f9752ea0181d41c045589ecad84b671d3d65518013090" },
                { "fr", "ab8db54bb89a8b83f9bdcc8fe4ba0b14f2adbb60c7d8f78d1c18f5be1282d48e5e4f783c27b95bc4a59f118989045ebedf8f926620b5bdb8d9223958650a523d" },
                { "fur", "d32fa26d82c7787287490ad6de15dc321a5a944a6e3663d33be4fd7b8b0538a2eea8d39d8e7bc754cdd9fa19ebd1ba1dab192f1148dbda3d134c84b9c1cbca5d" },
                { "fy-NL", "1bd940861b298456936ef615f3dad70f16d7f4ba4ab207c305e22520f98c5215aef3887e9487f85d0f4f1a56c67610413a90e607247e05dc5bd5c501549c36c4" },
                { "ga-IE", "af283bb4528ffa87d318eb2cfe07e340380e27aed6b382d02319aaca676672e22b43229bd4a1a4c89a43f7d9d74eddaf13c32700aca50a2f69b2d9028d0eb9a9" },
                { "gd", "3980eea2ef05c1c177f8ad2fe3fe91f5e0a4dc7a60f5537d9727cd4d50d49af58dddaf2096d306e05e706ce700608b44ce6e92e1062ca32d44d25da5d941794c" },
                { "gl", "b5d3eb6d688ffd5ad9e1a355984b075d4870215dbcfa4ba78cb480dda591e2da891f05ae78b298694b93305186e23dca2475d2322cfd0394db5a6138561e10c1" },
                { "gn", "ec4ab6638cf8b4fae7a60ee1e3b3b45f9f40940b76e86dffcbeb1cec81e8f83450804962a940b2147eb6df4c36e6ebf1fda7a2c514237a40612748a794896dcd" },
                { "gu-IN", "a389141fb3a116932a8655a9c0a24a059fc8482d331d5820e88dbd2cbb3d1721eb75995e744bcafeaf018c23b1e4539830166a28fd81122f2b59cadb67613d44" },
                { "he", "9479ad5b70d225cf8c98b5643364c97dcc5b8121be52a215850736ecbdf5bf815d2571281966312d58d659bfbe42478826c72a6786bde079c22475691527d057" },
                { "hi-IN", "446b1d03b06941de492ecc57b11b0ad9340faf8e5932a6081a11940387d954df0831411fa03d84b6f2b918ca81a06488929582c42e06003cad182e8dcbd20859" },
                { "hr", "78f77804c08047be287adf75744182fc382854b2b5a983f9e0ec66b1e4aa4accc8e6e4081f0a282ced3134bb3dba4078dea968d5795dc3fc4637d087a9335506" },
                { "hsb", "6685ea5995cfa963bce12859ad6aa18287f479dd5ef837ff44632f6bceb48ee5f399d9743a7fee7c3f988c2ee317d7678c92a10dbc4b5179fb6a367678aa37e1" },
                { "hu", "ded4c7e3800968cd02884f957f10e215ad44a7d98b9363742ca529146dbcb40ceba67f6882ae249924168392ac944b5f8ca7e047d2847e105a4d14bfe6595827" },
                { "hy-AM", "82e2b93a93b5b79b2256c386398016e3cfd02e9a2e400daa1d3251b8d7b7d4c6e7bdaefe32a24387ea8d94d3dc994d696c0b1889c48737a4da2112fc70101bde" },
                { "ia", "17199fd613045dfdd446793add7536f45a7f8bfea33afa942eac420b7b527e9441866420fe773ec0d37cdf906409b0f9f9c571dfdbc51401cfbc9617027f884c" },
                { "id", "9a559394ca764bd435e95a2eeeb901c562ab851c5c3f1c973398ba784667b965682f5b7a9db554eb2e08d6108b0005e0ee49c776fa1cea419400b24812b4d4ee" },
                { "is", "70e55031d58c7fca89f6ff8128bdbd903fa4e1094ec0c96ef43da05447062e073f6a9913af050c0e83e85de59c623d20dab5706cbabdc0482d71d2bf1d0145b3" },
                { "it", "940dc805b211711fb7667e1454a6d2dea3489854f328426216cd49dc06ebffaa3d17cada00ed2b29974bcc0091c7736962c380c7118427f5e099d31b411c9982" },
                { "ja", "611e77fa47f29900e91f29c331182aca4b6ea9b28fa705ce9639ed4188e01a7c979a6954413dd168ab9b84c1e1416f20582bd402bbc8041f6b66569e08b11d69" },
                { "ka", "7dcc970d8405bc4a7a8b9e09d5901af124ff6e4eff9a558034e90d53582de9862f64a51f9c2875099f9a2f6fc48a07f698571c1fb9079d7cbe25135704e1740b" },
                { "kab", "8fc54c4846a9f1a54cb731b4938b31c9ff2e09fe3533995fd4512af8149c7df4d042ac2f2f0193fabce27ed116062450a1dcbc0d5cb127473e5e46243719e933" },
                { "kk", "76fe94ed848a902cfa585b8d3005cecec1caed3e75520710e7a155f72b68c63239603d31caf3289ed7cf3b0c05043d482ce9516d0bf4206f1d11292aacecac18" },
                { "km", "07436e5e64e1dd419cd0fdb749a9bff35791d5ae076bc86c0a9cab5cbece56db81719570f498e648e4415e1c89c7cd7c4f3e8de3a60f02531b9c7b18f962b999" },
                { "kn", "b5bf19fc20096e12123378a3c02968b365a74504793b833d1198f6dec4a75a086977100e866f1dd0710b222ee7c41630df41314d31b1452fe31d394474248226" },
                { "ko", "e432d062e101f38b9fbe001840dae82abb8f42cecc4de282493c2828a75042facd7819d3a1a505e8bb3dd0b55713f2315b35b4c96dbe4960becf83290f6d5bd2" },
                { "lij", "134c47a826c6260245e6c2839c566e606312a2e70bd464ded9dd0342098b03199ffcef13423f5a0a7ad245a23d4a5ceb48372b7cb352190b949e1865472b7f3b" },
                { "lt", "fa3370f8367f39b1138307d739408ef0c1268a9e937bf04488dae22d02c6857f9bdf8fd97871a72a5702f38220da95c2895c5bf2976e126ed2c80a799c9036b0" },
                { "lv", "95da0e55ad18a63ffbdfc633e138488785a908da50966fa754bd2708b97d87d7563324883da3c1fb0b09392517c0e0adde1709bb018851efadca627a99bc73e3" },
                { "mk", "712e51547ad63fac496e68d455918a448796ed590ddfd21148d7e92112521a8161cc83c95d2787da8f0421f01aa923d0b8eef09c54cc2c9a74bd92328ede85ba" },
                { "mr", "40fa12c0b754c52943b3f818c0f981a66aeb91236f9762a689dca17a8ec4999e5e3f2a0d1ded043473e9088fedc73c8d9af6c7f35ba3834927bc116cee8bd4b3" },
                { "ms", "e852a3af52ebc42f6400afc470fb0abac373d185ca6ff5c01d5c5fdfa84272e8165ee3bb7f22c0fb424968ff7ad78bd04de2d2cbd9a331a1480e39f29ae82c88" },
                { "my", "eca1d3030bb257d6a4b482b50ab20a7df7eb2f5e65c9695314dd54aaf6a31c1c9c2ec8d01fc914345b62b686b5cb6449ceec3ee96d87fdbb2f79bd09ff217a67" },
                { "nb-NO", "52afc9741afe2f0f7baf39757604e38c6a61b1b685fec3a5a6aa385590391ca83dd9e2d3fe8286a065efa89360eb8db868cd92935a4debf75f5039fa27fa5b5e" },
                { "ne-NP", "e2d6823c07cc143dc9dff81d821141f6a49c0747a8f6155ef0c61ad5d8719e2bad0c422f28fced9729b01cc6bd65d75e6b66986a3b35021649b3b6e20dfb1cda" },
                { "nl", "e59223649f7733fdd8a169ef9e3b9cab4c4cc3bead4901666bab98a5cd8644ce0458a7e1da4eaabe296d16f13b35b9f631c5a5ccb1637e8bdd6c5b0afa667807" },
                { "nn-NO", "ddf20b8201d3ef8fd3cff7544a30891d8d5d0d0707613b99b03cc4570647b0a37116e6eb99b41366b45052b72b4667a2c0a6eb3471452051ebc990347fc2fcf5" },
                { "oc", "765b81e04924a1e4d0ae2b8a84dbba172b76475c7cc67821430463f2b98d63dd9aad0018f5f14c04f8a17dc6e8a742626fa9434e02a42811c932285430943f98" },
                { "pa-IN", "da627ce4bf2977f226cfe4a834bb8fa350540b1aa7512cfc33fa24f160684bbc941c929e26633dd8781300a4c553abb6138d27ce1ef51f520599dc49694a01c2" },
                { "pl", "85ee83c17dcbe3cab82a97971cffd88ec9205b98d5a5c69da50c70969342524f83362155573c473caa33d3f8bf1f816461df10b5a34cf14560b39525f80c4eb8" },
                { "pt-BR", "1f2e1628ff6173b7ffc4d047477621480c62060ca8409c7c1e132c707db0e81ebfe42c28b198617a0c0d89a7653cd64ec7bde023fb094fb7ba52eb257ee9bcde" },
                { "pt-PT", "2cf1c8c713bff45667972471aeb02b2c804811efa38d6e7a6352456ad1d9cd475f9dbb82244c3254e55860d610b53f7042756e94a3989557adf876cfbc88f3ca" },
                { "rm", "02edc7f2a5ef2730c7bdfc542d77cad227b389ae2644ce071fdeee893fbaab30340a7282abc30591e8e606bfd44afbec9c3252e970df6ff88ceaf58ec30ad3a5" },
                { "ro", "5578c541d209a4adb7e4963e0377b94c334a14f8945d512958aa8965ae31feb2e9eef8c8f026b8d9ba634366fb4a264901b2efeb684bc5d66ec7cf814fd745d3" },
                { "ru", "8b571d697730c19851f51c19140af543c7b25f08899a339877330c9822a97ef21e7e809f74a6325368ad431d99812290898d6bd26dc066632c1c87debc3edeed" },
                { "sc", "c538fad2573934448692efdadbbb0bec75bb48981d59b7e751f9fc738ba229af98feda1bd05ec196ac783017efe225f899803a9a214c5a3c6df7a8f088fd1cf4" },
                { "sco", "e4ea15047803cb93a69ffc041f8a1094d8f69f03dab7229e221d0ed723df081d7e9609fee7aa07ab48844f6334448e7f17b2e7530d5886c9c75fdecc7e993cbb" },
                { "si", "e8c720eb0b79ff852792b9e8383f45150447defc314151cc2dc3f55b15e2fdad6bb188cc0fde4a786ce1571604757fdbfd2a63fc3c38c60f55528feb3d90b864" },
                { "sk", "a369b661f44722af1fba7302d2882b9005530dab4b414174f65eba725977f3293c0ee66218bd0760651461b6e954a3106becf60199aac9eb1c4b961536251a55" },
                { "sl", "523208f18f0400152d9b9d2aff418fbba0911c50f28e50d3ddb1e2e85ad3109140fd404b6c746e4480dee97d77aa0d9b9607bdc7e8276befc7998d4790c0147d" },
                { "son", "97a71629b6cd42d7d2b43e8f33719311311db1920ee5d1f47e653370f8085e81a95a3eca944f3d12735efb7b312fb4918d392b0965f24d875561544c78d54b92" },
                { "sq", "a01b19982296f5e121b96d857336e54e92ea4da677b161f9adb559de67851e24de2fd4beefacd942aee5165265b576fa7ad415350bff57034acc94980a8f06d9" },
                { "sr", "4171be6fe6cfca5bff04bdd0c220f780d6cef79028919d5fdd0b4f99186354c717f0510fc786cc778675609181c6ae2e727bf1f4e45fa97f9418e2eaf7a2d2ae" },
                { "sv-SE", "403acc17e9b6bab3c2e36365478b4d84601087339ebee424848d3d3bed6259844151c14e19c2412f86ee5865ec0e4425f809f455029b116055b1f7ca00ea4e29" },
                { "szl", "362c3d413ff10b115624948d0a9ccce50517364e0f6bfb14eb9f3d68fd03b465b03997f106dd8f3470210636484a65466271e1b7312480cf8d2f31156223523d" },
                { "ta", "b9d28fa6fe726ab71ea12bc539634297cc0af8613b17ed5e6f97a41100e18b75b839ab5a1b68b8d38540a025c7d98507f21a36e1e24144d06013995bd5460183" },
                { "te", "bb3074159500698b4ff9c9b4eff4ce23974bea232f89ec1762363d42ae8228fd3269fbc11f2d5e7dbfce73ce55d38b136e27875c9b28b4e5861ad35be2a37544" },
                { "th", "7aaff45d4698b222d553f2af1d62cea5028920b348cd16a2d54a442a75442fb42fc9dcce49f8eac5608fff83a926a8cac847c4784566b53a8b21081b91b0fc1b" },
                { "tl", "c695b9ce5437da8e249c5263d351f7a11b21e1a6cbcfe17e57e4996853f6fd76537bce7aeee3fb043625a2aacd9ceb397026723d5b7d2f4f754b078dff8c0734" },
                { "tr", "93ee252b85b6265e7d435b6b07cc4c2e6796eb3676bf30ace9b972ca15eb5726e5504235498093d7eef3fb5e8d3b1e6c14db20f4db71bfa7f653c82a02ffcdc8" },
                { "trs", "0741553cc282b7a8f87e4d5bc097b246a525b0373fdbf95a9d6c9016ede89ef3c09261fabcd185e45318e2667917177971a2e64ab51f82ff006304639fbf337c" },
                { "uk", "2652a705489e9a90412195b97fd254d4154cb5f8a4cc351e9f610cda189f28a18d9f7a4e6440a1677a93e664a1015fc4790fff49ada4ded2a0e89198d37f2081" },
                { "ur", "fd87a4730e132da1d34d341c295e996f5aebe7dfd2a202499ba30eddc783e79a0d0807b6e184bc25441cdcb93107b3ab2a030fa93a27a58e78e78e75cef57d32" },
                { "uz", "e5bfa69c487e741663ff774988906b573a8cdad61fee9cacbe7fe101f7c0ec9b9566343661b84b2bd2487a9d49788e9a4105cb9fd40b949c4761a79fd6e48579" },
                { "vi", "0c45287af3930c17bfec8e6db3032998b2323e080693114741ba749d91aea5e83617e63c275c47dab163dd3d225c7e33c528cca745bbdc5a16858d5e572a071e" },
                { "xh", "441313391b14b9f3b0580144037df93830cb1574c0750d901ec4d03c02166fb21ed9e0a336eae54b4feeff82f95bfb09a238cd2adb7bb267a26c5fddc031f860" },
                { "zh-CN", "703082c244e68ffa91bb725d673532253f34c2de2b4f73a7c5fb14459c1d8c7f0f7dcd566cb2e39e10da97bf73407b9619fc56b9ea736d570307af428c5df257" },
                { "zh-TW", "83f9cd8e7481dd5f1303a93fae1cf46a876ba3b44eb8a27ebd16721e623f4d5beaa22dedb801415de79f0ae72217546510f4dffe7d1e22f1d18c98227c84a2f2" }
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
            const string knownVersion = "111.0.1";
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
