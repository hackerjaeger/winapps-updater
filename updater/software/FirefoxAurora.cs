﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022, 2023, 2024  Dirk Stolle

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
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "125.0b9";

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
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/125.0b9/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "651a830f96f00466806f1a601f7f7effe85d1590d460bc50939a4ed0ccfbb0df7267d9a9cb52cfbb35c5a7a6b1bfe93f6d27c5b12e0daf38e92b82e507ff274d" },
                { "af", "ca66888236442d8c6fe1f0993ac6981fc37af6545e8a1709aad26201a5aefeeb99fc36c4d4d227a353f47ec3a72f57c0a1bd43b8423ea5d70f796d7c49d0f18d" },
                { "an", "3467976d37c61e51d2f74d31875d2c00325c8742df612055a127519a6e287d3aa6cbacf419f9b0baf3df4d9d6aa7f9a8a04444442af996b6606b91fb34a4c263" },
                { "ar", "747b108666146e003a7039f92941574559fdb853952c4eae681e30e30f5a61337fef8d66ee73b7c2824824fb18c880596f3dcf7e5b4a6d8280600c07830e7df0" },
                { "ast", "14258762b1fb4a5a4a6beaf8f470857044b01bbe2a38c384beaab887bd490f7d3a347453a13c3f9262ad4bda1f88d273d0b1cc5da649189a28a127a2643b502d" },
                { "az", "182d7b838961bb91f0da8d87c5a5e9a059c0b825a8b3a754acc053f7765d4b3623bedf104c6677e400632a6921f887e928e037f33d0abfbcd827b2706499c534" },
                { "be", "832296fb2af9c06ac6ba337bb432cb3d616bbe4b7c696c124922c622636d67d52eed207aa435ca60ce29d4356031f70de8993183fc943c8da13d95d645b1773e" },
                { "bg", "4df88ff7617584dc6b6e8b4a5c06e0fcea489a7a7a3f69f6dc62fe0949e7327b6e192fdacbdbaf137ba8d2e8a8fd2047e70e1d252c348d9772bf86a6a5e46a9d" },
                { "bn", "f94bdd0dcb293308e169070eff6673399f187fe2be36b37350fd81a896d2d8e7f103082f207b3f7afd2cfcafbec48256d63f8c5eaba13bcc346b50440182ec83" },
                { "br", "54f49a1728cb58a37899c987a30989ec5386f8147621c11a82406c6860e445642187451dcbe2f0595e43808338ea0e523dc615d5120d4ce3c18c3abbc91c802c" },
                { "bs", "5bd159ee5dd790b2e11f9507736cc72dae1e876d4e69a9a46e339e52fe6172fe609c5716913c389790f3312bf9b7aa092317c73f379b7456c74c9b55259109a2" },
                { "ca", "a257fdd23079442aca9f8ea03115273265946779f2679592cf08e5850da4342e6e4d7a901e688ad2622100008c6592946d841081ef4d66138b60d5556b1b4aa0" },
                { "cak", "6dcb5b41f4d2e76dfb7aa83de6f4b839bb4f1c78060a22020ec4326a862e94c5d58a032b8c443e64c4fd914c910d3d9fedafe100481a5474d9d6cf786fd0fa1b" },
                { "cs", "753f855efac3e906bf011e5d7ba9b90fc9f6d697d902d3cb1f84ed8d187d6ef1d636e967f319fde253d40893284a904c757e8d06c92b7d7070382907714c6c98" },
                { "cy", "d23dfd5993bec102a0be7d1576cb8a5faecc8ac90fa5feb869576a24113e3ac574e9d72aaafd9cf6a4b4762b0e16bef8e6dd6f347c32131015adcf810bd2b30a" },
                { "da", "5f48fcc540727b9e144ed3c0dcaed1ec4b6bbf366487ad74249436a8f1250f821cce91be118b4bb6f7dfdf4b878aa5df513df9e7603537a5306f065b2a3593ab" },
                { "de", "dc05678ae385474fec5c1174d1511b9ce0b0d021be0a8f4d98374f23e7f062953040c91e925e6a5fc898174b93aef86556e11bba4482648164cc10e19ba906ea" },
                { "dsb", "40532c3ab1b47844761f5eb4fe1965dd196e60b4a0c62846a08dcd06bbcfca6db312038b221db6969b03f090258856bbcc2b0cc128dc3f8d2a841aef8792e75f" },
                { "el", "c8100fe9f37155387a0305789ccb5b817f490306f1aa2ae702f3762975669e3992bbfde95905078009a4217809cfdae62c13e08e06d22a29f9053c96509a7b18" },
                { "en-CA", "4fe49e76ef84aa5e7aacee153a63f7cd63263df8a653db745082e649f76aab4d829ae13488ff85e81be8944bda81e2a7d587746ce22e0555b1d63dc9de062d9f" },
                { "en-GB", "eab85905758eddffc69487f27396760051d1037de43801676a60f462a20e8e0874b603d6b948668ecdc5930868085b2479addf0e002466d3908717b2419fd7fa" },
                { "en-US", "57ceb27105f9875bae12b0bc2cfbaec8b4c0cf3c5701c0cd3fad00c8f28e621365347581c91bb4e9c9c2c91f51c9339889e39cbdcccd3692bf984ead8fa0e36c" },
                { "eo", "337aa1881324d533f520568ea61b6cee03b6a12535cb1f906b2e41da53916147bd076963dfeb9d3ca9b3e0175c522f24461e611677c65978e2d3ebdfbd438cc3" },
                { "es-AR", "387c9efb1d3de7b7a5ff90c59562abf4e3d244c17b9f3d7b1544bd400872145bbdb0d2a912e5cdbcba528c6ac6f4a8e07234df6c8c3b52ede8c762148667278a" },
                { "es-CL", "a41874c9c4f7460441bd3a6f1bdbaaf51efa7021b0f7a712e2e76252bbc16edfb2242f2dff2d540e8187daa339954a8587787cd4657db6bb95a83bfb07e0d1a0" },
                { "es-ES", "d15d82317c6ab072506ae3a449f6c9718350e91e622e0bcb091d080d982222bb85614189e41938b7c7e29bfd835d95beb5d6f4e31144a7de5c702606051dd879" },
                { "es-MX", "f91766031d63907d6daf07a0337a51a5fab002fae261bcad1aac13002bc97783ae43306ba040a81b33a306a8b7a3b49a7a825869f6ad705dac3b02ab040e7ed5" },
                { "et", "860e3611c0041120b552f0d761bc8fdc6a223dda637a287e56d99b08e67a8c7b6d4684efce2d4c76821bda3662437132d6b0f504c115444eab1190a500256a60" },
                { "eu", "84bf562a60e61f6e3627a4a44c4e347b9a2277db98542d0739d5f40848affdf8edb6c5bcd7a0307aa23816342639d0b8f252302fc79bbb5ca99d2f8d400acafc" },
                { "fa", "62c389091cd934b27db2fa5307ac0310b7f60a4501286f35cc6279dfbcbd31d42f4c651cf1ddcaa7a5b47961e33bf171a4d3a9cbf1c8a578404d7131d81a5d48" },
                { "ff", "05e86755fa0ec4b24702f49012e1c260281fcac0ebde7f3ab141f6e2f84b519fbaf8c6d096a1db615f6399df4edca1e26e807a02caa36bcd9cef25984ecce61b" },
                { "fi", "5f772ce8632dc02fee7b71447b942dd5ea7e82de293ae40824b9d3a33f3779277665a4f623b95eb8a51c90aeae3b0350382764819b89088d3b3acfa9b8b6b9bc" },
                { "fr", "c961d21cfac34048b1a28da1a9ffea5a07c0f708cd77a4ed44d42a7b0d1decf79de73bc6f064cecbdaa13314c03d8594f129b4684c87b8831e2527fbe9616430" },
                { "fur", "10e47997be563a92c048fd736c4c1d3db3fb21e5b26a18ee44d047118427ac3021ffdf825d8e8e2dcc857895bb432d70a0d259b4b4e43e5e1df3abb6f7063747" },
                { "fy-NL", "eb5ea7755259963c993cc9dfbb35114d0a30924e1ff3542ec8c6cc21d34f510cffa20120f289f979754c55485ae14f4e7a461c608815edba06a3fd64e476ec69" },
                { "ga-IE", "720b62daf2cd8aa2fcc10eb266a9c130411fa45f4e9ddcb9322b2a5bd495f6f64f0ef36137de8a60028021111f1a59fb1aa8dc196c542aff300f4659acca4a80" },
                { "gd", "1229679dcd38be94b6888e6bdff4abb366bae97c9ed8e98338ffef48280c95d08ec44f28beba76bea39effc35acbc9f6c0f65d87c0926ae61f97924a0651c57f" },
                { "gl", "ac467cb43aedb02e9ce923089af84caac8ccb113d2c720ef0234242c7466cd39da15a9b8ea7f1744fa0d39d3e67b90495dcbfeefccf820d269258ee92d754d05" },
                { "gn", "a0da160bd537c5f159b6d38f0e57f2ddf946656eaac317b3c00a41d469980d0cda7012650edf37e8c1d6f4fb5bd932f6b55448365b2f0617409067f80e9c47ce" },
                { "gu-IN", "57508d29c3a7c68325f831ca929cb8b9ece3644095ef4a5da684c9b29c7a96a6656fb3998bdd858abecf0fe8914322b206b64190c58a057f7da33d431431ab5d" },
                { "he", "b56d604f4337e588bffbf0fa9ee940718a9bf784095153006b438e2ec82122aef3c4c7654930ecda8a1593a591c80aa80a8fc580f6efa160d64db0ca2ebbd08c" },
                { "hi-IN", "1203c2a6cbf6a59b12f26cc8fe094ce9f56bd49258694a9bec12feef7f7fe55fb49dc3d1ed5398676abfee9eb1b93a86251daf08ef96e2464e2d3966766bb887" },
                { "hr", "26391432ca11ed0a698a24dd3f7b0705a50c068cffa7df42ff046f1c3a9727074c745104843d63388c9d88c229dc895f4023ea68eb665d2fb9fd07f6cdb05ebe" },
                { "hsb", "f26b56554028a3a4d481b84ad4f58f26dc9a8ee9bbb9468124932a098adccfe1fad9455205e2c6e94b57563898aac22709acac244bbb0974cc3456ea5c768a37" },
                { "hu", "844c0a10595b154213754dd572d7768d8fcbebddafe03a5300042645854c33f3ff2e924515cd0f22a87b23867f097d1e772d69f02fbf0c519890ff4b6fcc8ca6" },
                { "hy-AM", "7e90b5d53c4bc82a4cb6cd0fddde5a6b89a7eb103fcd499a67a688deacf553fe12e9b6836fbdddd498354d5d2b11a55e17514b828add22698e8d771a3e6a5bc8" },
                { "ia", "ba2290d5833f311cb483e7362070bd176d66480c59af1b52e776084d51084bdd35eee3a766a7e9569dceff65de4ffcac2727e743f23cf0264fb30163dc80c7b6" },
                { "id", "5479bc1018adbe837d0437d59f0d73546d951805fbd98043643cc433a638d6f534a0ccdd0c3048d08b94a672d6c4536df1344456818d20cc5288481464cac5bc" },
                { "is", "a494ecef7d05f7e4266198dcf5e1feceb95a1adb70ad6a6ac35854b35a46b05361997dcd68dadbe5c057d2a085e0443e10e5f389c393a49f2b194f648bf2adc5" },
                { "it", "da5a985641da30d4f5e49c4d88f0980316b15882efec31536886c6d806aa13555bde1093d2012cfb3a7a5e0adee02b48cea72023db04b1bd665b1c8c1e693359" },
                { "ja", "3a407b2f6b6449198afe6ed4cfa132bcdd3c9f5e7cfb234f109f22e1eb5e42caff83d903d25edad3485ba68df735d7a6beb8b79c3b6ed7106120e477ffcda01d" },
                { "ka", "2568257f684a159d2df4b0575daafc7e4bb62722e7dc8ddd348f809ac34351cb4fe9fc2b94bfb94c1dc680e3f86d7a0ff0b80a7180540a8ef485a7d6360b8a81" },
                { "kab", "dd15fe01a1bddf291fc9203ffe170a2f0807e1c5df5d6605f4d3c69c84f161953a822ddca6ce509d69bd7088aabc2126b60072033fa098e7b91d6b615b53095f" },
                { "kk", "0f4d78c52364c84b97982d97d2a9baf9e26c1fb814960a825cdea200f5b0d552399b57a7e6b3343ea813f16f38fbcefd317bb93c96eede37ab9cfce9f26dc182" },
                { "km", "60ce3b5fd8de0081bc04b340e508041db6fc16deaceaf1f2a5695d39fa7638fe6b0ac853c0ab59f50b6d30ac67bb6ad6c3eb88d913956ee1241831573fa110ec" },
                { "kn", "94b9eaa4ddeb82fa26571d1e3da0761b2a6bfd3e45fdab2d99751a318996b9f6da1c8e9f56f63ccba8cdea23d17ea902f275dfbcd13b223bb8b7d83f93d14da7" },
                { "ko", "ba32b6a24953df8cb5b596c0500acafe1b81c2c9207b07b1dc8b7c0d25772c993020cf7f2db84bc2efd9338a57d7f298c377268e3a7c38c143fa97f8ac4b7376" },
                { "lij", "87cbdfa668af323595b2be73bf4de1cd0fb73169c190b13becd5b69a7a97dd7a0de4b54ae61d6387d436d7390917fad9629e399d8a174f79e7476af285d05526" },
                { "lt", "282f9d08d07bd3b8033fd9070a70ad3ad87cb6f8b93a9a8751be0b8a3758a9e6095c80a91f78f83382f6994f52e22f48a800dcfe5f81e00df92897771c2c7d4f" },
                { "lv", "456d2cfe8779d324c5d8b72d9fa6f97660d24dc54059452c8d2ae7b5c12b75af09421ff01ac13088c03b9de923ac8d0877b71ed0673c03965d834b7f0d9febd4" },
                { "mk", "d0d27547aa7c8a548aebb9f7cd4c23df14381df026036e79127abf3df18c54d231f7813279136df0561fecb1d5682bfa26d3e3dbcac985743d1f50857f181209" },
                { "mr", "42e5fd1dbe145c6a4f4e800326d70225c0c2721eb70967f0e7fbe9ab5060c7a9e6083c02906541b03fb4eef9915a6a178307d1f1b236b9d39529f25a1246743e" },
                { "ms", "f27c819560ed9909b5f24259947e727d8923d4ec86bbc3e916a0b957bd6e8911291a4e503a40bb3ea355d9da707c4b347a4f3c6864d3cf6ad027281948cc9675" },
                { "my", "62622a10d29f56885821b05d071383029571696858046386a3361b8bcb4124105b7a698f274cb23c98dfc473710d90a5ba4213f4e3878ab3b17b88fc3b20752b" },
                { "nb-NO", "09346b73b7afa29348b1e9c6a9920756f7a55331f920ba5da0c6432f777986d5f89d4c346334b2aa06256e4b0803b3515a2e54aade4f844c9ac56b4defd58ca8" },
                { "ne-NP", "9d68424dcedd5792bfe46f2d53a7bbed4765a7df7e76380afe90e755008cdaf40d110d7a37d4cd43b001f575c044fb5a702cb8ef77abb81dd3939a7972e809ca" },
                { "nl", "f8199c989866522705e8b6e364cd19c54eee57953c52606ac6844c7ef8dbbdcdf78db72b415f7ba539400def62190ead268831b997b04291414f339ddbbc6868" },
                { "nn-NO", "c5d088d7b6b3ea566e249d6ef53255f58de88372a311f1118d124eb5c50e14d0db6fe66c600eb46a06f9d2621dcec6a020119b997c5b80998643057dbbd74477" },
                { "oc", "a526cc688f20252b7edc745f1cbaf9c512021b02a1dd85f46fad96fb0b791460d95db02e0fb4828a881b980584b918d1931c1c46bfc128bfca23301fe692c8b9" },
                { "pa-IN", "6f48d486fefc1cd33043e44c1a7957cd9019a4bb1b5e869421cabdc486423b6fcd210b1e1e3256bd3e87dddd940791e16c72842aa844642dc57a4666867d6378" },
                { "pl", "10315220cb22b8d8c6b0ea5028e2455ff025b59bec7fd83833b382858b709fdcbabfe0de22053ac7790b2d90a11fadf41752ab764985fdd8fecdca913e6d1579" },
                { "pt-BR", "489dffc34e75572d2dfa52b30e4f99a84d1a59b4d1954ec157fc45474922c8c7078db4dad07f01f81e8ce7f3c8ae37da1bc1e4d7313f04fec75fa59a34ea5cd6" },
                { "pt-PT", "862b828a75cb54ee930eea7f2c9775760a3251f1e2664eae57726d2f9cd007d3cd99707ae2a7f57d2e722c52bc557a8783f44b1d0ce399982cdd53400abe38e8" },
                { "rm", "0eea7a2938f7042f33cfbd70b2668912e566221b720b5943bb9f6043d5da4bf289ea77bdca860d4e91dee08877a8400d17778a1a40d7f142b6772d8b70ea6023" },
                { "ro", "8e57637045641bb74041e7aee048c1628248d103d5c931b66741e4231704e801ff8c9a0e80bd3f793be1527e7d333d9a083b8ac85af01bf5beee70ff900cfd96" },
                { "ru", "6c389d7ab9e94e5959daa9e039e210d01c787b96a05782ffb4adffe16bae3da8ac0d859cebd8fdfaad3027efdccf2d44b5baa52bc8e97e3062d975be8093433b" },
                { "sat", "3b35bfa103e1345f7d7d7c82be0d05695e837c865d8e3b2eb9fbc205c75596a32571f35f393f58aa742f5ee4e4d4de79c465ad6764caaf86c1cb7f35b575e7f3" },
                { "sc", "f3437ce9f82f5ed3418deda5cc60e1295aeb3c14de94b846220cff5d8d8b50eb1da4a0d5b4f82d7f16e1b242d7806bbbcbc295e1027e2ab10599bf4ddcd96ecd" },
                { "sco", "e086bc6cbc73839105ef0f88ebade397c51735699d21b75054c0ce29bcf17f69cc4f59c2fdb985e631083ef585d7843035197fedd51c5c6b2080a9565b991814" },
                { "si", "4b6ad938d68e2cf28d276a908998c28c5aa959e560eb833bcfe49cb53e60fec5e07ce01e101b26f03c1a4b387af73d6f5340e60431029ca28481d59cc23b209c" },
                { "sk", "9ebb448dd2bff375f7a72fd2de92931fc1cf0036cfa24e5a84afdc41804addb1c68d68823c55b29300b2d8fdb71e48b07785c77dffa62d8e3c16e420381aca23" },
                { "sl", "d8aa780e45e478254b711f99e07f5b3194718113ed2c6b7138f62e4f242451ab6a5f4047fbcee4de0c064eefed0bd1fc949b5af2eb952cdb72f2de47f954256e" },
                { "son", "3e6bb5f633f96ecebfa0f567fc71e41a091efe6b394a59241e1679f0be70d176c3fb50a548485a88d0955cdceed320cec935c37ba3b4f72613203acb78e4c611" },
                { "sq", "122dd78469b718be5ddc97aa11e882154acaf8ebc82127610102bbfe3fbbf3db4a4ad49f693826117266f2e8a7729a21d1985a311663e156c211ef28835f99f4" },
                { "sr", "c21e0ab3a145320ae6e263e407ca09042c543bcb5a4ca9e14d77ece713534e197aebec5f37a54de7ca40fbae59fe1f1d4cdfa3ee090e7b78cc87b499e3644ee6" },
                { "sv-SE", "4ff6241d9ac07a946382ef4bac417956f82e964969e08f931209ad8550ab9d8ca4d6cc6514d7a26f70a53cb0060b7eaae1bfc926848ecfeea446884d862ccd9b" },
                { "szl", "51d920d3b94414bb708cc6ad3bd189aa4ec3b64e0b4da26c189538b429d04a30730ac4c25eb37e84dc7b167301c728d37cde8528fdbf72b12e561ad45b0fdee8" },
                { "ta", "e6a220667117623eb91d3364a5886f3fe7891f10fbf8bbf084fdcb27dbee0267dd437ae8ee8ef92a01aac1abc617c2c2566fdf04aa0ce149369d597e0242832b" },
                { "te", "f73fe535d36e00891745d73cf52e8fe741023fef2c9b35c0a6ba71e6c7a58abe3cc27cdf84c5bf1bb2fc1789f00e2b966045b84aa98126fc1d7c18e9cb2d84c0" },
                { "tg", "b30886d4278ffea04025693657a11a4c8d84e5ee63c6d076b2cf5d98bd9b9e338e7ef4bef35071d0d97b8915efd3325adb45d8c8526dbfcae8dabbf00b041571" },
                { "th", "ff60802a78ebcb784aab3af46dfe669aca565d0a7bf042c24a69a82797ec5e2a67df1608d95e3f90b378238e4501dc9f9334a5cc69127a4ad258d6d291931971" },
                { "tl", "4faccd1cc72e5de1c1730f33215769bbd1b84f79b96386e9e3b7b5e8a3b0fd3652bc03e2c806d4055638f9fa184f7178c061cbf8e4d55d3c72db7558f87f96b2" },
                { "tr", "3223231a380bf22b19c4a061c7bb19af7f07d00eeb51dfd7eb8b2d9a3638bb152f195b36c6214793664e06a306080ef05932eb166cd60e27506104a9c34376c3" },
                { "trs", "7ba3feb6377f2f67728464503ede8b09168344758641eb660e7bb22b7cef580cc78a13bdd8776edf2178271845cfdfd170b18b9a1f4c7201591c26c0b12c6302" },
                { "uk", "0bd27f89020e13d537405fa3922469d99faf8ec853add77aa071e6ee71eb7b465fd66944d64fffe30166951e3bae655ed3284b5b0ca22e3f9ab64319e9443b76" },
                { "ur", "6ce95838e0b4d8372d5ab1c32992473c620fb3a65a15737602ab322b15d7c351c031d03464e3d7d499f1c4c917f5900c19cc0f494f838cc75d870199cc2f5c46" },
                { "uz", "66957e6ff99b04b5c74ef6090db236c5e21091bd06f9d5b1f2d19ad62acc90ba7ccd4a5c0959afb66c375251d7e967391ac133183e0a081cc5e7c4c8fc6bfd78" },
                { "vi", "be2cb03ecf46bbf988e4b7b40da377f95de5d6cf710adde0a74b12d18b89921eca0e2f1f49a87a47702d85083dedbd18d8736d81f3c058b476875078104fc875" },
                { "xh", "dc7ae9719b559eb38bf058439a3764f52970fd341aa97696432365a562c1ceefe21ed492079ef36ff2524792aafdc86396e74208e68c5f6c20af6f25bedbfe10" },
                { "zh-CN", "8ca9a6f8ebe4e5531af7624b6b94d1fa2632acc37db73f845dcc5a6d6a9254e8b6a991f6d5c66bc2bd8e9aa989b41e5b107aa3db2f6cabf5d36c50e8c41be7e5" },
                { "zh-TW", "9281ec795ccca0aa8d7436881fcf8e718b7c9a82d7e8de3cba5b0fd5f3c92314ccbf90d55a5ae38bf28b871e9a00e1d612239ade459ff14ebcd5268a5ccfe18b" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/125.0b9/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "fade3f3a732395bfda2114320b7f7cf4dbd9884ba6e9d5438bd09c43c61e1021fa14df222db051446ccbaadfcde91582eef91ba85de621e37948a4bc69d48ff6" },
                { "af", "05426f434ff6e437754737a33fbed8206d344ab61096c816e8b41b8c5ff4bfcd78010df7e291ac0b4b7400b50391a79e4d69cee045aa47aabe5838f988c90db3" },
                { "an", "4a7519ad2eba9f2cc753c575bc66de5a5894a92b975321e08fc0a1310c07df9583c429c072c264cb79f54411f6069717d5e099e7146b0a81b10b3c76e02ca6dc" },
                { "ar", "8cc20f4cfdc49a9b2d5e83b967b011b129fae63067280657d2b2f445f22a40a887467db10023617fd68fb4b76257be0bff638e779c5232f6660600f1983090fd" },
                { "ast", "151cf71bfc9643ac25cfbd707f638a2e740cd46d86b1f00c3865ff1ca3b745e23787bbe769a0e60b6d0016dbdb132a7c6aa02f6205f37c77a51e7c418dcc6000" },
                { "az", "64745a6e7e120584d3b55139c06695c99708caf633f3dc0df1ab65acdc91d68b6ae9f5dc5044c3533cbd0ed0ceed5724d0d199a5e84a948085f00798f85aa5f8" },
                { "be", "7080c109787bf4072ebf49cf4258d66a6816fd689eda1080ca993a5e4c73434976be8179c05780cefd8ba8a8db9c49fdfa1939e9587154f4f50c480305e1e9b5" },
                { "bg", "c1c7852bf7cb0d39cdf606e255162fba4805f7835e089844055ceeaab969c2754f90102678c8ea012c7475edf6fb82ecd138eb9b8675fd98bb4df8562e0b2aa8" },
                { "bn", "ee6389102048184c59f56f965ed0add187f917a8c4f63e85f82335fc5b6ca189d1cac7da487e613aaeca6df3b78e043a947e37e724add223ea18dec9fc300a79" },
                { "br", "2cab0d9d7047e787ee7839773cab9a6085abbcd4df6f9828edad0c9156fe5ae0fc602ee7d7fcb723f162deac53fccd1742f6eea6b41964dbaf656b19610f0c25" },
                { "bs", "e71f019dd3a2caefc6924a9368987b04d4d5834c6d1014afd0f1324ffbbff3b47c6c566381ae1d28bdd02a14b3362b80fb1aa1191f133e94e8acd7d6a1cff471" },
                { "ca", "016b202b214fd4f1664061de88fa39ebe50e788d79c8e953e6fb816edc31c48f23a3fe9db6b462148cc4541887d985cc5051bead2085b9587a35220843883acc" },
                { "cak", "dcab63c9d8e17ba3f83e0dd1075cf6d97f83b26e6f525824bc12fdd9cf6091955dbdbf64887d188d1595fe7b1474c330b69946723affe7502df676631bdefd35" },
                { "cs", "8bedf4b3386987935374cdd277a4d9712c5cbed094b7d0aa4ce6cc99f09ed1de10889e85e956bd0076b42e0072cf59e96c9ed8a614bf96104c1c8820902b1a11" },
                { "cy", "ff9713a23981355521d5ad74ec02cff7509dc6c57ca9a4a65c5f1410b50ea52b5f79d39f6676a7e8d249a7610ce76902142646c07254a689cb274760f8b61efe" },
                { "da", "9d792766ea98b9d10315d88511af20263f06bf366b09a0319a0ceea347081f005d7d04d437f64e16327cf52fad69684584eddcc3a617878f64d97450284a4015" },
                { "de", "06b135d6c4c86c4d35c4ca76aa5ca0fa7b664c1f737e1a121c4ec71a83a0bcb858dcb96899a382741a6736b165209d2e5f2ead09188cd86e9186fb2191fbcc0f" },
                { "dsb", "52c6d7e20a36e2ee87dc6961d3d1a679bd7c6d0683e1a4dd122c14cadd2a3478d3347392cfe7b0c2915885a6b7963a1940e0c1c48e406d239745caca49614572" },
                { "el", "24b66069460508dff7d16275017aee8630d8d297fcf0b820f00bfaba0c97c6ec1b7e7a39c56979877e5df783f28ec26686ca493e612b65d792474c22d0461d7d" },
                { "en-CA", "75626b28cdbdb5e8402c0fc4bdb1add850447d9f2279bce3d8e6781928d1216f069b5a9a417d0523ef8e6bab6e615e7dcd14dd2a5e39a050c85de95bd98865dc" },
                { "en-GB", "640f03c0a24fe7336302a3d5fb434038b913a775f873be6f2a6021ee49014fbc49e230bf8e620b25747a196a83eec6cee7b470155deefaa3db5492c42278ec3a" },
                { "en-US", "2dc1a9217b37ac66fcf8ce7673245d9660f74d8188ac7e14add5e2760d4b75be9d1dbcfbb0f8d825328c32f760f01f4e95c2f4a09c5bc0726bfe2340b55c987c" },
                { "eo", "29fc8c4d8de7aaf92eb200ef4c37c2f49716ffec1c27f620e3554427751b30aa3720e085980b66d6f09d138f5ad40b9f3a124d7af187809cad592cfb9b0a61c3" },
                { "es-AR", "a0bca496f2e64fb4a95c9e7abafcc4b471b445ce771397b762d84c6b95596e279a79f6365fbcabf192d17897deafd21f3aa63d983e7bc384d3e4b9c103caa985" },
                { "es-CL", "04c44c3d727dc6b0ac65c34e684f0d680f0dd96d2f6aed8f2578c5bb1aa73dfd9d5ce192c0fee991cf2b2aba0afa997893dfbe4a5d02dd4dfa88035548be554d" },
                { "es-ES", "c5fb6b2ed2c1a864474cfe9d41e7fdf90e1a220f4936908e86308c868a1e1427384a10903e4a60aaafc49ed68cbcbdad0d1f6cfbed280b9c9e047ff7fab23d82" },
                { "es-MX", "3f301902aecb6157424ea64b6404fc62cbd355b29abcf84320208fbb08c239381f9c3cd8bb4cf7357fd345714f8351142f7793f65fa5dbe61ba91a9c99c9b46d" },
                { "et", "3d95c7416c060589ffd3673c5325154d312778b03afb8324ed88e03decd94efb4d86e6e622514c1bab435c7b8db5706f2090c6e484fb3ede6da636e28d1e21ae" },
                { "eu", "6f580fb772e23755338de0a0b48a3f418b855eaf8d55733b5ec3531515c4431d9a0886cfcff90e14c09a673c9701d5ada9b8fcfafdeb2c32b1f8bb6471ffe66e" },
                { "fa", "e1620867139b0a77042206bf0368a84e86520fecd174a3e68fe368c9598e5d3c667ed9a9b0b728c4e41420e83053e763692565b1ac134eac55f6ecee0cfd6b7d" },
                { "ff", "debac0dc6c4633938d25be953dcb66107c2d7e63ed611d354fc2b1c93f79fea43bd172880d6eed445167532c18ea8a8704ac03c5747cb0d4dc02d2f3d97dbfca" },
                { "fi", "0fe5367e84143447a73ce28a09d9de47eb840b3f2472c85050cb5dc60c907816b4034b2b0e2fbf732fa735298439e0b938cbfe656dafe9cf082abf4bbd82893b" },
                { "fr", "b1c989c90c634ffed1761990d223da5df18181718f6be84cf9dfda73d0f259c70d089f80c194eeb2026ec38eac46e9107d4cf2cc33c15392c5df513f69058461" },
                { "fur", "c7317cef312e8719062a8ab6db4f035d34ca26e51a531a9fca1ffcf9931f9aeb2094e70e1e118f689ba7be9b0e6ff17232ebe124da8976ea03eb88bc830bc381" },
                { "fy-NL", "da93833690276493e1fb2aa71a01ec60fd129908aec5fe539ca490129a844ab8cda8fcb008fb1505f1aa5cb572edf7f522bdab058cd9fcd7ad091dd16eea5488" },
                { "ga-IE", "1e6689328848f563b58b671e89900f624d5fcb655fa09be242aa318c17ecfdef30792d6638653c9f2dfee6e6f155e50cb2120457e8a130e28ca03a0c32c919c5" },
                { "gd", "46d6dd68a3516abfb5ba3294332ef1de5e636fdeee9856de3ed5a644db28f1c8f2fbf3c6ba396dda17f0cd4461ae7194144c4bd834395b05a5022a92f26734b3" },
                { "gl", "1a28b834bc1789b03bffd7bf3ee26e774e4a749737efb27894c1cecfb4e3340abc3149ec08a61f22b7c69240e48de4931dda067e07bd44c5f5196426e2730ac5" },
                { "gn", "1134499f40d9f0a5eb0d6bdc40269f5de024c348e53b502e684980808307e6615721010a312683999c9b547789713bbc445c1e14a721db863d34ba13670bd90c" },
                { "gu-IN", "c27ded7d47dc389be9d240784f1da4ca3b0e681098666df4cbf0019074324135e8a4a9141deb6f6fbc334c0f70620a335e9e2fb3f4481776807f76f1cd18f1e9" },
                { "he", "194b92b28fd7044b620e414f40cb54e72ba6ad64e2fdfa3d0baf11dea898cfd256bb6be45d4d6b0dd9c4fa69881530d1107286b7773ae601c0f7a278814868f3" },
                { "hi-IN", "1baf05f4d226095970aaa672a0b294593f0c8eb35c2cd19b9af95f81a444b7af0bffb86c1650959caa79c879dd738402de48197fe0212937bc5376fb81a6bbcf" },
                { "hr", "3f653e7feadf6bfec4754acfff0dc241de80db517492cb890ec715e4f2979707e67f32d92a1ab903f065c3b8b46691bba1641e81d2081b006708397b1017696b" },
                { "hsb", "ce7e86636370b47a09d438520d0feae62db42a9e970bafa7baae0b6d2abbc4c9328865e9d195e04553659cfda875c9072cf93f1f286eb1639344d32ae801ad63" },
                { "hu", "9530715ff253b8ee88ed379653b7b7554436bdc742addc9375ae835fb1706eac2c6e78dedacc3275e0709ca7a2468c8283a81f128f3d100bca9608da0ce716bf" },
                { "hy-AM", "5ab877144ce5ad43fc99e52ab8d26f34af76cbaebfe5830b117d50699abd727cb4ce3631fe33739a321ed936311178be119b398027961e8cf83dd170f966eca5" },
                { "ia", "2fbf051c5d6033217ac9365ea44f835e1781d328f823ddfd74f11d22ff0e13f1cd588b9ac9533eda9f2384a49d8965181c64a08367326e02e88c4d6def5db472" },
                { "id", "911ac121bf7fb0b6452f2c00189b319f63ba84401c4ec2e81990842bf4bf9b70ea73d946093e82d770e238893628f16c3b0133dbe6aeabe9e7092380ba5eeda7" },
                { "is", "513121c450659fcbac7db620ebd534d73ffc7200321ff56594f3c293526b10a41ba948a35d4fb09ee154fa267ae2c44b783f83846141e3014eedb58ff7bdf548" },
                { "it", "f6989bc605fc1ea3699cd8d4b9aec9b01b313ba7f98c799e7462459bd5c6b411139480cd75ba7c528f459c5a46522362c64daf79e312ec9c57667146ba9999c8" },
                { "ja", "379dc280f1e34d0794c4f981a4e390e8a7971d343321a0564c6ec173407056afbcb0afe2b7b8e036f7b3ab4bb851c9dd0fb2132d5811ef6fddb760d44cbf5053" },
                { "ka", "b8801aafd446f36d6a020c8d6692446d9715d384229263b6c4514a0c1a3e37e56eda71947a03c1a53050436acacc4d8d8c55a8c8d5c1967a0811503496bda132" },
                { "kab", "13ae57396dd33df08588fd4981868d4affb800070f29948b16cbbc635ea1fb8fb1380efcb1ce2751b52e8a254f3d35f626e5be20b601b7d0bdd1998b28ffc978" },
                { "kk", "891fd8ee0be57cf08be40a2541e1b203c2895f8e79cc2318d35a102c31f27f23aed4a447ec057db8eb111b49b9b8e464d223cade157358fead12266c8b9ee415" },
                { "km", "885c0460134770d7a882cbc95b00b4d99d22fa978e08c6b9249eeff1b86d1cf2840593be60f0ef055406df971c285bdf25da0313aac6cefd8cfd09b994484dff" },
                { "kn", "e7d20065b01d47cc98943d53b1798628c4a315afdf32a4c247d6b4ad77e4fa3845638d991848ac3bc1bc2e73284842b2e1ce4be4755b6746e9ba2745a37a8225" },
                { "ko", "3e45f59ff6d5939db8f15ae4b7e098a461a21632c9e79568d06beff494e7bd5536b00a5f1aa573c3b68236c0e096b0fec1ebd7148926086240cb2064e396e674" },
                { "lij", "9db70429681a1c1bba4b57c9bb8799cbf75e4612e1dfb73bc3daceaaf2480df4e087f962286bb918157f9fb75c26b31505484a9e062b1cb962f1738407ff3271" },
                { "lt", "cd72320027461c1054d72409186cdda9eb072eb592fa81c92cccdc4b1bd40109158c2ae04224d2d15174112827d5c77d73fd09f26b81f842d46ad14e83f20745" },
                { "lv", "33849df6ae21555dfd9c5fcf43fcf5258df943ac690d8da6f018b7b771baf35d921d75c2011d0298dad1ce9474b7062e398849ce2469bd26cdebda38c5451091" },
                { "mk", "9c2c76dbfae279a6e22a7229b810afb59b218b9e9fd6cc1a8545f3be192f78a54a9d8463fc7bc6d5cdb93979e144b1224aca2c3dbe6772b68a8410ed8dfe5732" },
                { "mr", "30809d92f0e24e12b212898dfd0208afe6814c4d3167dda2c64171d1f54b739c45e32a0c7d15a6b8a7819f18754da53b16869bcbbb2abb20d5197f0cb5812a1c" },
                { "ms", "a161e8a45457b5c05e372dd2f392f129e88a5eaf138261acc6f191cd6a21937efde43d29b64a0b228592cf620d920ae41bf7b8e1e346b8373e22ee4b2a52d85e" },
                { "my", "3eb8845c54ab28d3a88bd27b2514f5aed22b1c8e437ee95205dd84c99453aeca49b0a90a1741fd659e6f3aaa4121dbf0050cddccb36e31351829355df4f0dff0" },
                { "nb-NO", "98c4bfc71a87efd7e50ff5f6de4b762724f0ffbc16b85d0dc085c9588bd9edb2f8abe5c147d31b6470996e7920445347c29f585a41925668b9d58ace0542487f" },
                { "ne-NP", "cc726d6039ea7bf4661ff04936c95546a8427193f9229fcbc46fc99e7d619ac8a8553c5fed32270a34720ec810bcd325a9c860980295e3823fbd7684dd6b5e47" },
                { "nl", "e5c31936c84ea2457c919e32acbe2971e38675127aca54334218ad39e1351418091c336b4a2542231d9e4c313446a2ccc6e9f04f47319883b5a6831f717e8619" },
                { "nn-NO", "34bea481fee6d502adee3403a1f199d34b7b15594cc6547868a8b47575fece824d7af9603e14420627d94670ec374245e9b1dbfc2c194d7f7b56722d8ac7eec2" },
                { "oc", "afb0c27cfd0ec9e859b952ead9a9975c7ce78bd8621fe70832366cc723dd3e07f5f3d66174c5a68c3277e1b95c4a1dd924a1c2638885f3dd18f404540266e470" },
                { "pa-IN", "5c2793381a3cb10813bc86b6f455d8f09c66c5f790d121a4a4db56d3a4b9ab6edabd21434b528e138b9c4f048892dc1fd78468e89adca94e03a10c1fe2304906" },
                { "pl", "42e24fa11ea8582aac526ea625244ad46906f9d30a418343ea41c5f48dd111e27635ee5e3da9d0426a3f6ce3ab1cc7cb8c43ac6a89e61192757037e2c5467743" },
                { "pt-BR", "a142e8853db5017b8524dfc27d39bc492284f9d7aacab8dbdd4d71f13bdaaf1a4d4817204b890d5d76ec5c064174773fe7f07f0ae00df01bba6d67655a2a3b24" },
                { "pt-PT", "a3159004b021f5130e6e4065dc4df55746cdd607f780cc08c39446b212fe8cbbd97f8bb8828a1ffbb32f86a91a5a7fc05d29463d55329ca12cc1ceca22e5cb43" },
                { "rm", "98b259477b5056aef66b7401725923f95267da4076a0923d5ff41932ed7f360bafbd577446a80e6016bef7ebcdd64ee346f5b47e176e5bfa611d9674dc1f2682" },
                { "ro", "dbc9b298b0aa6ecd6ca23d4878fcdb1305ce6bf0c6febb1de25029986c74cb9a12583b4341c455bcb8b5aca8942edf385ee3554bdc1dc59d3d95c15aa1ae880f" },
                { "ru", "6b1ae21467c0ffc2b2762275ac7f42b8f87f9ee1c9bd8882d0f12b00311e0caceff4578b3727ccec4e7f5c345b82a1102026c6e6d91282a43f26a3d9d348d9a1" },
                { "sat", "4298bf4ae0e2de01d7708f497a18a133c8d17b8d2b42ede62018b6cc32bff47cefd0f9740f5c3eecde3f4cd164aa46ef9f1363d5709db4f5f3129d7bf50adc2d" },
                { "sc", "1e8b897893f90668cf91121a1bfe7da63c40e76bdda9e8575de4130556a10be33aa660289ecdc63b5cf519c38a6e7578f8449e73b2a92765de3e290e556a118f" },
                { "sco", "afd41307c11ee9499757cc51ff9b5d5e8e0ad0d36834ed87fed96acfee27a5f3ed20859ab223bf7a17400ffc67740dd6a86e46c78f2748c7dd83cdf1f114d6a1" },
                { "si", "6d0c324db10a234ccc0783dd23e1bc301c77e13747a9162c123f62798b8de3fc2ff67fe04334dec56ee33eedf72b3159d8c97fec1fff39fe329845192094786a" },
                { "sk", "d90d5949f2ab5808bbfe86fda24e05658b0d0121af8c5922cada2c96337d77a731d42333f93a94f7b1a4327ac885966b3c2baaa595fb576decab26dcc4d9e3c2" },
                { "sl", "c17b4430ec0f5a2750879af126b7302b907380fd52fc98fb48ad4e648ecfc7e1eb01e98e0c74f1eab23d47ad87416d7ff0a4c7397424f84ee9f58af119c7df57" },
                { "son", "20b09c138bb380df66679bd6fab6b9c9800d6dacce4e43625f38f97880874ad247d1ff4d0d1c8dcba60dce27de1cde9ed58db0ce39e37a105447a9c2ffaa9ce8" },
                { "sq", "7fe886a7dd6ba3cba306568c67240ab5b23fef66e44e6d8a960a1b6162f0781757fd86b50be636d2417558f91c02b73fc5b06f1043c7e1eecd056ffb99ebc52d" },
                { "sr", "34b6ac6e0f97a0fee91520be318cb90a0b26ed0d690260ceb9044ff59aa80df284107a723402631c69c83a6fb4598893a01d1c1ee9692e72383c789be964c55c" },
                { "sv-SE", "58b148d663615e612982b2a00159ae087b785efc687a049dfac261b476ebe65655cdc82cf53253c5cfbe40479eb091803e6d8e98fff75fe4de02c4b39d8d5209" },
                { "szl", "6c4eec545704fbad394b015f697afcf52a12dd985993a03cad7fe04bd24658807d912c24adb3fb6ece4bae03827b7bb9bf9e471e1c04dca43f1dfe10f6648209" },
                { "ta", "f2453c477969e53b1d73e971fb7952cdee4612e8347ffddcd0a572a5677c4528c8d7a9d200151bc5192d07cdd5d2c0855894e65abe181ccb66d789fb5e1356f8" },
                { "te", "f65e8c4ed6388e513dbde738692e92a51b9f8e98ebe649bbd2d53e61be8d35420bb5bb2c0dd9e927ac3a8d47a0c83303c250a9e2a646e02d7673ae1c4a732f5a" },
                { "tg", "46ed98dd0bf7698a43d0d13bce0990bffe27c420ff78dc1ee2b0fcd7d6bfff3a5c7454550d8b47bf0d654939c8a42823df10fd9dd0d77e7ec2f4ff374882e1d4" },
                { "th", "5dda230ead67f07328f59893bf5ee21c321341ad6ff3943611ab3a5bfc50eb2bb1e744cad5065c3aa35edfecb5cd3408f7730b1c3a4bf35632931b04cd77cca4" },
                { "tl", "0491c6e30a2b7a586f4cf472fb6be9dc9d2e9b886188107b24bda06d715b2ae3f4754ad971093999f59c34f761f52bd84f0c7058af0f6258577f25de7f1c2fa2" },
                { "tr", "d7ba2428b34b06a6e7b7b5cc49a43e681dab15c46068a2d4271126b1a1db700a53b5c992e865c11a2720e859f138ae6268809d937aee2fd38a1e34f06cde8f81" },
                { "trs", "4ebebaa24f1522263aefcbe8a0a201950ae43eaed1416905a1cf8a30a33c19f7c4b254b94a792d1b83f1f80ff10ac671513a20f2b018ab9902cffa37ce3c5aa2" },
                { "uk", "20aa80abe219d7bde88b9811450bf4943b73e06162d6e38a8837ceb090d8a9ac4df898463609b8f760f8ae52eccaa6f4bb08b79c5f6f7fc1644ec976ed69de90" },
                { "ur", "b7b7b4c1e89234a2063cff757dc02c30e493d62d99ce7041306b04ab159370392b660e283fc388643925f128c015ae2e3a6776f806eb141e5ee6d45eae7ffa99" },
                { "uz", "186dbbfe031c94139734d52db2d23c47035f94647105d48c673944167d03fa35088b9a6b1f1c40d5862464604589c688b9508c0a51989b2b229a355d9fc40389" },
                { "vi", "3ff68ea1ccf331bbc0faefde942c66c874ffd10e6e5e99aec0f6f18ea3872c683c8acac862c3777eb948a82ce16e222f3328f40f4499ccae23428260b03d65a2" },
                { "xh", "978e4f49766f9d21889a074a2d5f737273dd7bb04dfe50e2a9d356608906f76fad5fb0aa57ddc20c7d90692a6a2833e5249b61b023ea9044e69ecef89226480a" },
                { "zh-CN", "d45251ed30380044a83014262fbdb53bd352d971d76b1b90fca7dbbec7df8873a80f8623dc3962ae268a7141a62739a6973827e9102b3ea50849eaa8d610027f" },
                { "zh-TW", "2b964b7f9c06cb5fd1d35e4043db074db2703005bfafd5d2a3f3629b1e57dd4f36948edd931a2465164e24de11a07a5af24fe7c6fe7390e162565c502d2ff759" }
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
