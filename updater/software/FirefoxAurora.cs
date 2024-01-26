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
        private const string currentVersion = "123.0b3";

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
            // https://ftp.mozilla.org/pub/devedition/releases/123.0b3/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "b2252302d483dbb3edb43e60317485166e21e4b06b5a9217a428f0038d52c3fecb501c6df75af95e134d29e821a8fc7de893a09e0bd646bf1a9775956206736a" },
                { "af", "53343d5d23c0ed5a3b2c9b987322ecbfcd2789d41797b3fb792db3ae05ed6eab7d5150893cba41c6add5a6b59dd484684f6e40895dc39326b64fb79052bd7de3" },
                { "an", "5f1c9a5a8693714fe49d0c62fd6b2728f5b3f616445c58ef93832d6e2333dd6804c2da5bba1209cec4ed996f688dcbb8a39ca9be0524de42459b8aab6607c3bd" },
                { "ar", "fa6a74815e62ffea98e7821b3a27e3b089def40cbb5642419a1532fc02e8b92cc5be49ccec26fefdb48351b428701bfbde21f2623d1fedf21cc90146d7cfae3e" },
                { "ast", "a491c06a94a3213f849ba26a7f14aa7434a2def9c90a574edf76c8226972c4f2498d568f78b87950da2e57947cba9b72f755e908da079c3e548563a202ffac78" },
                { "az", "805549879cb2ff88811b99ae96fa0ad3a8253f745457b9a74fc13d8410cb67edcd73a24897f053b14635d1c22de63db7b3fbd318f547ced568138bccf538fa95" },
                { "be", "50fa1f653ffc7c57b33d4334e8c8ab77bf15b1e760493d3451f1e9500bbb892bc4fe7b35bc5b22b1d61582d7cea78c5592a39654d33328a782254f82c3884c72" },
                { "bg", "a13cf4d8848e151d8021ecd5a6bfff377e43f146379778026f905d74c9fd487f5db7bcb6a949df7c9b0111c153fd5dde80f2a7bd5f711262ccf7ef371099d74c" },
                { "bn", "f9cebd559cb6f3942832e991d4e4e11df172f647c8de79645ae4682c739d4288d93942924e1d20856b38719abdb81722a16adbdd561329e9ec88ceaf757f7b03" },
                { "br", "d783200d55d670f8724756447cd17e2c61c08d697eeb9f784d8d5a41bdd3b09d061562cda1794aaf482403ec490d985a5d5868334166cce5a0f6c74cef29ab47" },
                { "bs", "7574e16cd0351ce607c5d0e326444cf48ef3f9649b48f32e54e91f4a250e6307522d3a1829922e91241bd51813cda2e6f27079d61aa1f9b94ffdbe3ab076f7f6" },
                { "ca", "3bb63dc3641b49c0e27645415d85161b8fed83b952519d1afdf1c5e44d9e144c66b9d1e7dddfaf0a8368ae21b42c6566d707d6ba89f69949a4809b1d3e0dce19" },
                { "cak", "ae768d00293024c1e1a577cd84c0e83422c06fd93ca390da929b76bb476cae781783b0438a2bf1868c3dc76a06c14a0fa407b6ded29872366459e165bebdf0aa" },
                { "cs", "a2fcd9e38c52aaa7bdcdbd586f603bbcd2c86bb4a4e9ad7a9d9a11f07a7a008a4fb3c4f1f1b54fb6f6c564a7fc122a15bd9f9c89017ffad8065d4c226b37b641" },
                { "cy", "af984c348c918bdbc3b9be1fd8aa116cead43dd6bf587735b27db98e91f0bb0135c69a09e95eb2bf49fa39b9878db533456cda07bb1a26286fecc41333672893" },
                { "da", "a11f5576dd7d6009dbe2754eedfc8c4a48409bb8d273d3c62fbb0ae1dd48feb11da1d903986b79b02caeaea48aef983454cbbfe61ca181bc801e5dccaaf2732f" },
                { "de", "f8d31e5eca23830ea5dc502228e404ce2929ccbd8ff85bb1bc38d753b8bebe9698722529c41352a77a90f17c7d9779e2f7753af4adc6a9e0a518b5f6835d2e91" },
                { "dsb", "643825e4cbc66ba008220abdba49b0551b5e5142a78c717bd92a15d332918dc0b9e9ce0ddb0ecac99d88797bca0915b195828a5fc8528b05a4faa26168c86a52" },
                { "el", "fef19c497ad1eefbfe1c3ee1123c663defedc966ea691d4daa46dc34c28b4a031271e74b2c04160eae23c3183e5c11be659b436444adb89b9830ce6281c44150" },
                { "en-CA", "0ab7055a5bb3f9f6ebc67c11f8411a5b0e213ad57635baa1e609d1d030c56c4a6d55bab57be42978113e4de5eb8d8faca5255c0b1b6768bf68479a0fc238ee36" },
                { "en-GB", "3a7fbe403e0aa7fc728dc21d3e5dcc6c095f320b408e53e89cd519193ac675659a0da1612fd8f300f288d15a6dfdffa6b0cc7e81f9c41052a68e1b9fde313dbb" },
                { "en-US", "0544ebb343c7904823cd0a8e6224b88eda716237f5791afcc06a14eac65acb290a64c1d6790a672d22f248e3181da14bbb1a6d00e3418c8ea5a200e5fd58bcaf" },
                { "eo", "f476f147e12057ae5e8964918cfff228286b4fe1d6d687c6ab7595efa83a40bb76422480ded9f30bf58f3f1cf76c00cbb3d66cb24eab597965430ed82d6c9f5e" },
                { "es-AR", "b167058159d9672de89bce9288e395287d8078532b1be7bf83900c51af45b25744f954b00ebf13221497bb3e6395aeaa0b56e8faa992d442358b37fecf2957d7" },
                { "es-CL", "7ef1e26587ac36d3c645fb8dc9d08430dce67a4914f9df26f939f7ab0385ca278c201ff2d58c6d7bf0662d0a2e3f49ed97856b83c759ebb82f6b309aff8d4aac" },
                { "es-ES", "81f0f74d0a8c549abfd1de6b23fa2ff0cc358293541ea31ad12d83f9a4f63233505691df3dc4864e54d333b4b4b50f2a335d94803b639aed7c832af2c643ba62" },
                { "es-MX", "b5126c8f7b709180c9806a9d32d03e2650e080b36dcf5555cbd6271d2f528c372fb3e9d8890f59fe3bd9468adabef02481b81c240c2687a99679d4433c58c130" },
                { "et", "ee1e6265d113f695d57d062e7aaf6d8afc0cdb3f4906f5e9744e7211e6a552d647dc8bbbbe10176fc757a1ff6fa7202385e2b75dda84c5fe5a8a51c60e876c14" },
                { "eu", "4343688f3a418046e8304a123993de3389792249d8c9b079c6a774089703ee987ff419fc0e60074ee17d8b853c2b6109c83c60afa50febb755003c14f3599f99" },
                { "fa", "4b8f85a43cf5278da153f680484021945026523be39d5e20194ed1bc1a028f55c4f01a8dc48fa298a80468c12b9af802fecc67c98425444fc9dabac59fb2db63" },
                { "ff", "ef4ebed95c9dd95cba63a31ecfffcd16d434bbfba6c7b642621478146d5ddaf6478fbb07ebf0eda60d63cf1ca1c491c35ac4c87efb97b548bbd22503a395d4d6" },
                { "fi", "dda76c77af6e7d40f5600d30b7c68c669b810cec60cbbede0ace46be920b3238ded4b1978f469796a67aa55e4dbb6377b31c3d7eb2848f3ddd9e3c04be38d521" },
                { "fr", "4250da110c4cd318bd13bee63812f2a9b4ed015eacdd1c54c04a445435d74d835f4647cb8d2b01277dc7a09350e34ec6edcc5a96c31d64d689366d00fe1a23d6" },
                { "fur", "06781c287dd7cd338cb0b5ec5c1e419e41c8ab3c48a3e0036f47c0c55ac81a4a8bba05bf8422a11f92c377600a48fea79d297bcb19f0bec90dffd9df6ea9af05" },
                { "fy-NL", "d91796edf4def67fab0807e611801754b7ef423e30770dc1c531647fe02704d6aa74eabad8ef6a1b0d5822fcb922e9596ae6c59ef14d1fe010bf4f8310cf4822" },
                { "ga-IE", "180ad7e24c779fba41d485c799ec0d2baf697bb54f88b0c60fee46e0c2369e7e7ebdb157d24f9071e17e9e8d14d70657feac10acdd51a1b7ceddb88936e7de2d" },
                { "gd", "3e85042d53476b2d4bc7d1061362184e759afb91254b524da69dcf39aac230c6c93111e950070920fbd131bda5107dd549c8a96dae2112f7909529f7a89837f5" },
                { "gl", "4e0d0722be393bb0e26cd50674e8a223c77ff25a5032a24e6e7684523b3350418110d49e53ca5216a28a5c5def2817d788d5e6794567c88dcb00c50e8bd9d988" },
                { "gn", "f317ed05e706c5ed23eacd7fa86afdfdf1d94e157142ad008febf9fe35f1ff8a594a67453f12aa18d9439ba1573a270e11dd5c298df110cf978d41cbf65de065" },
                { "gu-IN", "81e113f499ca7293cffcc31aad3dff8333d48f649b7fc80d95b38d9093b6eb0044eba8d370d943517bc3a79e21963cda594d4216ddfc5eb053bf3091a4858aaa" },
                { "he", "f2ffb1c68086c6085d4aeb9c9c17227059e1b8567e53ee781bf09e767c9998e8a5ee0747d6fe2c9066b083b1dab418c82702081edbbb410b07f619a9ac50bf61" },
                { "hi-IN", "3893aa974a503bdfe16a5906b3671fd6aa7d855a01f838e543c8c553bc801fc7d0581d729f04562a22a0f0a25820e80157f9e68318b24bc0d3e8ffaf8301e128" },
                { "hr", "ccac1be08c1a03a241f60fbcd4834eb0066ac2f07880d97a711a804c86d58330de80d2f67d2bf018502e56c9d7e0862ba640090c5599d13e0cd21a5ef9671520" },
                { "hsb", "82eb98e67e9e231dffa01cf2b067cc9fbeb8957c1c1a5bc4e9fae788b06c7dbbaf5babb7e1d5aa9c8e6c00cefdd576e87c626059e2dfd01f243379eeba9d1fd5" },
                { "hu", "0ac07e3e91d7ab989850bb08ef1014429ecd7816794cedfa470c3b8d31a36f2a68315b91e2e5355bc0cccf2c9f744529244202c3c6ef356eadcaebd283cca759" },
                { "hy-AM", "5f31c8d22eda0f5869d408f44dedef4ced4cb27eff7841293f194941a8b704ebeb9c4acd7167a7b85f945490dbf990b1d119ac64aca217ac7272265c9a7f3220" },
                { "ia", "69a356f0f1a42a6416c355bd78692a042e88fa08bcc20f0b76492604f89a6e0c35af1cae616097065b9198a2ac987696c3581b033d5ee3d8b4109eeab19b1a8f" },
                { "id", "d720868d4869949f940851475616de7c6b5f097545fb59d100ec429319929755fe23d4abd3aaf1a52e1a8b6c3443e0448ab9b3a4322369aa15f589ef1f9596cb" },
                { "is", "0e3d12aa2713203de7355540a0495b1fb97a6758a6e9cd64c60c14fbb6ed45f2f3bf1569d1a09c4159106b296328d1db947308373fe5f222cd2b75dc27b9816f" },
                { "it", "65f30f3d45f72d3fc357be352dee9584a4d18fff44e1969c472655f1d6c036d6b8a3f8c40d2dbe2b26a4d17b1e92359ad03cfff39c37b64f869c85784d89b87e" },
                { "ja", "89285ef6d51a4079ee2162a311568d11ef7ad9fcfd13dcaadd1c9035153d2f118d2bcb7481d3aa0b09baee34acc3c4c86ba42c33123c0869ea6dc1695f66e2ac" },
                { "ka", "d7e24db4eaf5ddb9675beb2dbeeb7e350a3d75cc4b67d1c5a59fd94936fbb251e0f8fc1b2ed1dc772bcdc601531794efa5ec09e2c572dae5c0c3ad52ca47c78d" },
                { "kab", "612ce235cdcaff3bc41ea1032a33dab7c1be3a028a0b089004945742d5ed8dea8ee6728a748f216162ad68ad4bf752521269ccadc381f3c2f35b62e366fd08af" },
                { "kk", "389b9ca545621f70dcebf84a0e5fac74f284fa2a825000ff398fb21767b497c12902018bf1a2c3bc3ed2a49353848d9f8b7e932c3b9eb1e2a962704e6e411419" },
                { "km", "478c63995961eeac2441da24152677bc22596a1cd9eb7e2c5285d7005465fc4db9f7bffee491007641bfabdb21dafa4db74febe0ce79447868559c705732f916" },
                { "kn", "cb4879136d5d36cb8ed4370c18b9fe3d8003645b9134d6388cf91ba586d333dc077ed1ebbd174c270afd896541aab86c0fa7a3ad3150f266c765dff1716df776" },
                { "ko", "65a6c50a7aef4677f5930787e8a1cb11433bc8e5d0e833b014fe37e2214afed14206a2716a369339e172d60e2628838ca41e35e67684e8554b9f2cbc2b70a773" },
                { "lij", "db211733db27c7f568ae5b4dd71843d2489443ba749eb00dbb477224ddefc0646c8fd7d902a9f4e0c716648ee82e72e6466fc581b8b5074e714e4a690167172c" },
                { "lt", "6d2d464604919169ceebe6c433a00c1deba4b3fe388c50b3c92ac41a4c0de0f08ddfa2d4d259100f1b996eaa1967108e07e46e0712f5ca0d78ddb91ac56fb79a" },
                { "lv", "1c96b6e145ba712afeadef1ebe65d30f6dbeef56270bacbe8bbe0ac709cbd37afa2f2e9c8de0cd3b6305ee21c94ad19ffca885bd9e043b5266184b63089ed18f" },
                { "mk", "85e0bc4d3e50eb8b0e9bd149161c338d5807c2b7618a57decb3d7cddd62dae72502fb4e81d29986788572df9c7e5e74a98a7aa688d876163c76c5882a476f4a2" },
                { "mr", "2787085557449fab5e202d36ff47443697d4d40572c174d13dad52c6c2d3e2b46d21b3c6040992ee474d8bd72fbc9d92cf56ace00aef49212e299956c96e0f84" },
                { "ms", "c8492d507f63c931a27d6efdac54e58e9bcac855dd8ab09c0cf92d03ff7848316388c79dd3730a9cf887a1f03ed534145efe2b484b9b63fb91b3ae92475e75fb" },
                { "my", "e7a462c10ac6564ee5fab00dedebb4962c61d96c665b069b918d98d19210d3d0d315066be50ad7d24d4f4391100f7b37d61712102ebcf22fb6734b755b7c70ef" },
                { "nb-NO", "54775d8c9869b0014e88bdfd24cbd2e343f1f165be8064ef17749d957dc8aa1815c8a4c706f8fff30103ee8a5b67d8b04999f152d29ec1e9dc2270538eacb11b" },
                { "ne-NP", "8ae423a1667f638115e960197af6d8ae6bcb3a97f7341452210e534b8b9ff4810731ab0ab92d7728ba65ca21781c29cf306e8c5cc997aeb801aeab100d0187f4" },
                { "nl", "922ad1dc149bbd0f00219b6aab3ebe53b02f18c5fdb0bd9ff94ec443470ee66ca6285a95748666231fddd2ba2f31527003d0b9c976ba1a2e5279ba13a5e5d3e4" },
                { "nn-NO", "55b0aba891a2653f6475200de7078791f58a1fb1f831af6ad9206a4ab30bdbc6014fdc47e824a5e665d4bcc242197a52964b2a5c4cb9abf9be88094b629a024f" },
                { "oc", "e727eb9a7780188687febf41b9667ae7dc29b52c7418598c7d9a36d4689fcc81206e1943c6c28b8bdbef2c5e68370a7334b66099120b254e1969058867065262" },
                { "pa-IN", "960f67aee00d7e64d525dd192f32f886a94cca90c2aa3ab44b63dd697b8510d7b35d6df2c9b9ab8b6d659ec7bc202ea951751dc87ae8abbc4713274960693f4d" },
                { "pl", "8a90bc186f2a2c1d94f9472f11b61a3d0af1c11e546c45f2350fabb1fd5b1afdb8775d2a0f9445415c510e6162f38cca9d7c0662d310b6730837e6594f8554cc" },
                { "pt-BR", "b95b16542799396d1871d5fc8754b670fcdeb37a04d149f07fbfe072ee0c17cb5f606509929b9dfae4031ede401d0bed30bd2636f277beb8acd0f1124c126515" },
                { "pt-PT", "74990597cb77483185726273f0eac3a8555f7fa5b3318dca383c3e15094978a7317f326d09a940bf62ed3a2c7cdc242f6e877744504b6c9c24cb6750f70aaf12" },
                { "rm", "2076afc0357156cdec25bea273a8171bde356188051843488b085e2cf89934fb48c62ed7a96a5996f65131658ae9ea6dece35c7475a0fcb001c630278b758995" },
                { "ro", "e3e1e0ebc93261ad44d1683a3b50da38406622a35ab93c245754c7211a4d3897da76262e30dd02ab729a6d7934e5f72b3820d016c0b31ddbcdde76ff76ee48ff" },
                { "ru", "966acb533f52e3cdbea431356931c0ea8fb64d1a3556c147fcabe374727efe1311f29c0fc60216b9b0a7cb3018c6aaef7f237b419e9b3ae31d5599ef7aae3309" },
                { "sat", "48f3e96a3b89e0ea1e30914b9bb6c34f169ab4db4d2b8c929eccd85583c368510ea6fadac58d6bf7c14f4fcfe2ff5a041c205b0909989a8312f7a402920a0061" },
                { "sc", "5971b9f8ce49058d1f2c5cd7347f6e91cd58b8d5e46e69a2bd8e168ff2b04d06b699c9e02644a5e9218a6d39f1d88a37fcff4dd6c34ff9b62b19d7b95bc6d56a" },
                { "sco", "1711e1932a4c957b0a3616e1cb3e88478eb50b898f94ea59a4c6ef96d9ff796c9c0856abed764a6427c6d488536463d9318fedd920c2848a4ff88b5111c1f603" },
                { "si", "a0542ed5e954612e72e34f32839ee480341a7260270a371ebe240c209e44a92465e037373a2608f8456f42b7f51ec33d41f19e82e12f6772575a9936789dc98b" },
                { "sk", "e02a417dbcbe32d92378d2918d4cb0d33aa8750fcae20a717bed55b3e4e26d28e32156ce061f577450c53ed673930a2ec2ef4bc1c215fadef3b3da1e55bf4070" },
                { "sl", "77f96e2aad19296934f277976d7e13869d840e9405521bafac96e8da808cfb9777e2ac8970aa37bb8f6fb921215395c9691222a0fc3b1f937457e2d6b2d2ac93" },
                { "son", "0153725cdf9087cc86b48ce263abc55ea5eceac9659f4b54cdcea88ba81fb96a2fd209e93723e6ebc7c85f05a6f3d615a66fe9b5ef907ead25b627e715e3381f" },
                { "sq", "e07ae9ccb7a6f20fa0fe44f73ed6f09fde088cb89f6ef1a859e269dfdc3b5dff7ae542d8894716216bb03a36504b31487c27cb7b3f03f8041fda1b03e69f3fa5" },
                { "sr", "9b9d684beb52734f9336f744ec77dee3230c4e7cd6c7da43cc863863ef7ab277118a347c476097b32bdd0dfbccd23a713afade577f25f7765dc4055ed9b2adf6" },
                { "sv-SE", "98f3ff65ba344b8b95aa41b19877ef8e5b8836be3759b356c18f2a8b1b38e3ad473117e89a0558b5f905038dc8b87db2bbb23dc580ab49456b0943a39e02900b" },
                { "szl", "332fdb7df8869c070a1743af861f7613e86679faa5de22dcc4d5be6c23b9f2d349890c60b8b8f37eae228a80352be70afc5e671c26d2e7aec7c29a82cf9649ba" },
                { "ta", "8fb7ea7b693a107574d0ef019d29d41a5f4af8b1487d1d119ae182276c88ce1a395eb44e663c4906ff3f25512e3de601d199e536f516e6343cc6e8557e6d24d2" },
                { "te", "e9779b133f953b49db6455f8c809c5f1ac9f46272df76ae887e69f8853e636992ca8aa9ae4bc310e5ed19088e12bc4607e9d833b3c03c7ecb93fa3b4bc3ae2bf" },
                { "tg", "56dce264741180beda37a5286382de2875e41f52cdc93bedb63bcf827da4a5cfdc8c2c2ff4006131b9e5199f181a155f729e725ac68fb454df72a291a8fbc4a1" },
                { "th", "c1c1ea1466bac3c18df6fab2bda5ef7a95244601aaecb40bfa4b8849885e6a1784f74dbd7c7802d68bf18aecf75190457f875e19f7f7d718ba2021e1b8a2c27b" },
                { "tl", "84af1690d7294bf881d5ef5c15e70253628aac450c5da072edf885d6db57f8b41a92824a031d3d85e7dec6fba7ef1bcee81e6cda46a6fe0c69895d7b0addba85" },
                { "tr", "76a72a0fa1adb6cd74a9de2bb835eacec08223f95d20c938c20cc712d86763ef8a04cff760fd181dba9a76a2db0db46caad816b1fe56d14c7ad0e866d0e8f9c1" },
                { "trs", "43ba79bd38bcd57f3489cdf7d0a459b8ac02f4feb3c72522dfa7f074e11722b724396469af3edca1a1a46b2ad3679b2257f98b6b338066b30d702b50f174e814" },
                { "uk", "51ec5bb4a7010b9a296ceaff370aed60a084758c2d7ac2740a301f1923b05a94d407e4c764fc6b31b4a492867f5799217ca9257a07dec4a322484d0d60d6e9bc" },
                { "ur", "ddfce3ace40a9963d5c629e261d2ac5b0a6f9fa8566dacc514908348b8c0df86e6d873373f623578a60474817e20ba426fbc4b1a57e2bbe2aa45997d2420df5e" },
                { "uz", "06ac4d212e8af43d52b805e41926e6c82e54db0cbf7a8a697f572250877b7b6db6aba7eb1f93f2102bff60b31db79d97fd980543053ac9b9ba8f880ab1b849b4" },
                { "vi", "fd62ac695f7f15fd407c08716b2042fe31f00c449a01b223eed59a3072094f5ad3d7341c770c2b6a443123eb5d882234725505567d2e35e682efacda9cddc276" },
                { "xh", "1a4a53d6b0dd5a6cba4c9df912c02d7c64af69833133338e84711e170efc00fa433743f69d91f1c4ef8a27d8ef4d23dab10df6ebe0223b28264cd7bbc672c6db" },
                { "zh-CN", "33edd26a4d55825fd3960c666a7a15d686297bc3182ea0ed0f6de9b0a61fa58b48f8cf9736d106b8b93ddafa61d53329daa17753d655cad4435955c604806a9e" },
                { "zh-TW", "2d364137b21b94b9e2681385cfdfa43263cbe0c3868a40a04c5223ed5ea298cba9937b7b26fdaf90f896090053e4546ed0dbb05775fcbe5bd86e1735356cf9d8" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/123.0b3/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "f69cc59eacf59cb11fd32a90674045a20b5b1e2260dc2215cea64164bb430242c9b456d686e244b8dedb528b0e922aaaae845185063632f45ae43c199c9a2c57" },
                { "af", "012f0157e8f24aefd12eed4012f88ad489232b7f6ae7d07bf826eb6186d8b78fd468eed60daf663fe32f700d6e0953c46e36cbe74085ae4e5a34d07cdf871b08" },
                { "an", "6e665b350bd35e189d2c7df03596f027f44086ff4cd860cd7cf1f542a64452ba1fa997746efbf66616fa2754e0676f9e00d83f8788a9833d0a3f046d9ae2650d" },
                { "ar", "8c8229ec31fd0abf9677eae14453347c551c5304a735af7195d48e0616a07d1785f63e039769210de1720fb7ba0fca5e3b00c3e2a52becd1073ed7108fda965f" },
                { "ast", "3b2e860ff6625eb8fb3d39c1f1335a9f9c11978a911fe6e1953320adc6efeb826bcd903084ec4dea498428fae672fc7d56f25d732653aeceb22fe39d3bba55a4" },
                { "az", "c5d51b8d1851ddc42dfa9f8260449697ca61a15d57305677ce004f4af381f750b727b4cc49d397eafce320d140d5d70e0742c0bc4684af6e94846a2994c66699" },
                { "be", "7261a9d518212b85e5678fc086838613e938594770b17fa137edee6fd7a6418c5d9df8e2b276899d5af12954a11f1ef4afc4ad0643575d09edfd9f87f32b5344" },
                { "bg", "2723da18525f28e90b6ee4210f04abc678602c199c68a023ff8682c8f471d2bec1201430b1a9a744d8e96b42ae0dc731f6460a2cf611404d680df29d5b63d80a" },
                { "bn", "5c1689454465c9a9e2897253aec77c7dcb100f3b1358aefd633199a50d3a2a68f671f7ea9caf06e7ad51500fe30032db07db83c07627e2380877f6ec7c8e5385" },
                { "br", "78ab8c78c8be69b79b562de0cacc960a08f04f8b1307819c779a5eefcf16b41b9a54043c3705715b2de4ed9ead1a0a00462bf5fc24962e89a4d3f48f3baf8044" },
                { "bs", "42bbe64df4c48450e30ce7577ff0b56e114e14fcb21b5db911cc4946c5ee6401a9e89e0b3ef1c1a3477703cc61fe29d50e23934a06be369a2a5a1cb712fbc71d" },
                { "ca", "5bc049bec8876b1129cde723ed21c8bbc33f6781d60dba2a309356e29396714e914778da7026ea12674e66295316f28acd3c2cdd972356a49e3ec3206e83bab4" },
                { "cak", "5da1688d7be719c372b68cd708284c9a6eb50d0906c33dd2cd0105f13d018ae85abc037ce20d8084ac4483ecb05336a2798c470ffd9db1beb181bd0ecc123368" },
                { "cs", "a7037f12cac611b8c7d3ecd26ef0db9bee8b0ca652595f23137ab9aac1f48ac203e9fc25001ffd7420b832191f0299172e3ae4f8e419f1bd75a96ac8d646f5c3" },
                { "cy", "a8d9e47306dd8eaecb68385a851111501df22d6dc795d7e30af68aff08e4800ff99d07031f854705d1581337feae5ed9c71c8dd818fb35cf0172334f546f5956" },
                { "da", "4195b0cfe6d33ced45a5ade8fb82db283b2c14394ec8bcd47a49617b3f160de33ce5ba6686fc94153af298f74943edb3705526321da8e71e06361f4923932a3a" },
                { "de", "231414811fadbc862b725b197c38aa233313c6091a7e24c966020ba26e0bd231b6d24ad41872ba52546cb749f58b98f9374227580243855809ecdb54a885cfcf" },
                { "dsb", "3548a6fa64dc5b2e93ebb19d49752a39323d9a1fcd305a0fc9653fea01f9ef0440f5f18cd5d2c41705c5c033ab3a1e4c145f31499f03a4f8477ab275dfa718f8" },
                { "el", "ecf6d168d79a6f648b8df03f0e7630449956038246532729578f62df8d31ed0e02683b50c137ba713d8b1d4293f1b6a419f6a6aea9dca594e95546433fa3039e" },
                { "en-CA", "4dafa4b9bda430a2599e1dab3532d30dceefb1873cc1edfe2b806f680dab486509a26c58b0049579d8324b6c046169340b66d6915a4c3a9f62c224a4e0c0ea8a" },
                { "en-GB", "bb68c96991429edf5aa7fded5318935d4d61be81089ae59cc9b86961501577d916f4f893bec54a0abb561574ae36bd0a8d7936cedcd4d9f2e7f93226aed16116" },
                { "en-US", "50e678fc7726fc8af84cfdda9431be39b2bd184e69083d5e8b65c26d41509857686fa5eea6b50f145694cf336de7e09d8961ccb6213fd752d92fdeca56fbd8eb" },
                { "eo", "2e45441f206336fbb7d4e9e79389989af5315011989a7f1242aad2078edd7cb9967c098831f5dfd56bf55730d75c0a0fe90373b774bbd379e9e9a778ccaa19ac" },
                { "es-AR", "7725aedcf9b5409983bde8bc3f85e3ffd93e622e8ab3435b61c1113b1891c6c575bdfc056f15d60472ba0b7d4dc43b0122cd6ad99ba4a9e06d18e33088018bec" },
                { "es-CL", "c4264eb71412c0fdd20aec7f1b57d22783859811c374a7fd3d7a78e875acbc896df972c4b751b04874cfe675150ca3bab55a4129c4fb4d8abe6e24a21a657d5f" },
                { "es-ES", "b226865206a9ebba4a5b2b960470e6eb999cfdc61fde60997bdcdafa0afa4470eef065a06db88aa56605bdccf5615dd42484c70d9a39bba5961f7be745c62600" },
                { "es-MX", "1b6231526a8dbdc221eed3b9b1935027301569f11ef8402f89c607762ce43a568929c95efc47fd60b740dbd69cbcab1ef58d6f35d3d49856c2aeefdbd8fc9c2d" },
                { "et", "e2bf332f1a4e6f38509d9d6e011400a14504d1505fb515b1edae9ddd3a963ae5af2405d46703536a611f26c2ade746f544ce2a4ac2859c38e0bbc33e0b27e072" },
                { "eu", "935353db54320c28cb1173c98c8c21524ff9f26381e3034364a426efa8df37bbc2bb5e015747142241c0d54e2135fd679f7e270e04ed0fc2d8fc30adc1632a75" },
                { "fa", "b51f25a2f4aff6f87ccbd7ae08cf3ddde6ce027e1e1d0ef9d834593df6d981477c372c35ad15a366758e2e58b8da7e2548eb1490ef4cc52f2dcc067b293f55b8" },
                { "ff", "b23b9b329004eb513cdc3ece21fad580e106850b79676b4b3b20a302b3f0a626434091c0ca642615bc89ee0b887dc08a64c17d3e3e55c8fce5b060af8bf2e7ee" },
                { "fi", "54cb4646f802edb736676d3248eb728d6da9d562746f1eed11a3c8f3238ee3c9b7abb8e9cc0ed8a74e4acc8e3ff0bcf8f694e517f054779bbbb06e06860f45e7" },
                { "fr", "805b9f8540523ee55bf8eacd664694fd4e6b536e292d324e98af80a26e9ad00d4f5583debf0e0d20ee3c08bbe0617c62a6b3082fe073923335799b742f3eff61" },
                { "fur", "0a12afe6243ea912284a4850823b2989b2aed6416a34f056d62f4c03c244e5a41df0bdf5b3fea4325ca27a93e5a89f6f07192b04763b6e582ff5e5931b680a98" },
                { "fy-NL", "dda4570f831ac4d04f86c43f00b755a9c70cc6a79de76f3a70a81c91147605269110d441e53a3b1c03baac5597e0a954b0df632ba9ccc1db8991a6208269296e" },
                { "ga-IE", "47b02162de68a9970414e5b0279fdf5809b91ff929fd0d10170abc359250d4f2a2ea439a90538a193b398569b03a7bac8ea5e91ff06dc9437b2812fa67aa9df6" },
                { "gd", "13d1219fcf52a980fefe3746b304da605e4f5e7c79dfbcd9d4998b8d88a17414cd9f8508c6640dc7581b6749508991b3c6e74e256b8864304f9bcf0a23ade255" },
                { "gl", "244bfd2b4529f4ba147cb071a9d4fdeb34d4133ff6c570f5db8d8f6b0807eff5d67a6b42f016a030acc4a4a4d63b4c0b7986ec703a81adb3e342c129b4ee6c1e" },
                { "gn", "dd804cab9b38b06d50e578b90aebae978d262845b2b9e154a5a533cf5bd9a1228983b62b66c1ed3e71e2a9850552daccb1de0a1fe3bcd218c4b229dea83ac1e0" },
                { "gu-IN", "60c4d3d86ab7e3006710ff0a668cdfc23ef2bf247613b47381e98f94084d443df2f919921932bcfa2899dfabc1e783a6bce551247f67b32f1140dc7dd334e166" },
                { "he", "c883d5a745b9b5b3c87fd8de0352d7c14ca1309830f64efa49f06411357a035aa9bc2364dac98b10a8ade778eb6004f4263d4080d6359230831aa6c4edfc7ff0" },
                { "hi-IN", "25f52403232e0dc9cea468b7199803ba6a59fbdcc91d9063c4f52488d8e924bdc17009e6c60a68a4fed4b24868c94d2b99a4e92e5d1c0eaa0bd05244519e7b79" },
                { "hr", "affdad25ee6a35af1a683fc05e1295604992c51b642474fadc7fa40d43a300ec1e8cb6e1093a610938773bdae30e4ae5aef50f7664bc70a7e3165d58652a939d" },
                { "hsb", "3f406b3565791a77b81520ce0c0fc4059138df2485af8fc681cb5b93de9fba7b47b18a98a045bfb690ff5cf3a97b10276e58308738a713493f3ecc267e2d4c35" },
                { "hu", "494fd7b04a324a62f06cd1def21764c04a63f92037e43c15ad9aa4242e0c04bc12f55787217bb81c1c88f8a8b491febae9264ef10d71db5b16a89359764c3f04" },
                { "hy-AM", "d19b42b66bef1e4d6e63aed91287888f86d96bf5014a51ddda60f4ff24ea3fbdfd229997172fe703324839e69300b9cbf4c95df561e451b961c318c66774e23d" },
                { "ia", "26f3fc191c85ccfd95e492771e665b553b47353021185c079172fe9a1f9fd8b4ff2d63474970373d5bca041ee4ae480e27ba2f92ba522430801ed4306bead332" },
                { "id", "edf875dfa5df0b5f0448ac7460c75180f4bf67f203889cc24786932fb44e4ae84db603974f13c6b7b87e379ad66f0434e43d8411cfc44c07654b834c49ef067d" },
                { "is", "32a182ecb6b16f9fecdd832eb74bae54e9f57ef312d3a9e274807298fdf0820584b1ad99d0e7208d332a750feca377978c5ac23d52bd83e248b1185d71aef2ae" },
                { "it", "a702deb437cc498742a40c324362574152063b085da2c9525d7c14c85743275400c7c7e3d688e9472e572a4acb491e3a6e26e8785d450d2ecd33349acc501d16" },
                { "ja", "1094ee50d649892773f6654e07ccac1320fabcad1d14b9f01db85aaa82de3bbc2c28046e075fd1d60f3dd6a7c81dca5168cfe4862edd1fd74466e4c7dc6b389d" },
                { "ka", "67618d6293e80aaeed294c062f1bfe8c4cac532ba6900c9ff615b2f54a7b2dd035f6ab337e0c02ac9848e2e4c17bb4df1ede90e70ec1969822b976438ed627c1" },
                { "kab", "fe535dc69b50dafbd593e9b1d7523b1eda2dfed62584b48d68f162579c5e8789d0172d06e5a0d96099e49e1c731166bf91bdc0dceb0be2e4c6c80e58653a4c8b" },
                { "kk", "fec024c3aa1cfc20fb00a4e44743a3213fb3d467fb2b00fcefcc6ae84291ea4f01f190d489742be7f99079be26014d7571b46569fbccf6e185773865c3e449a8" },
                { "km", "20041839403f1fbd13bf4b02b1dcdea641ae124970967b76db64bf732b56ac7141fa804ef49b8a3dcfdad48520c12b1836fcbf541313378a9c940e55f945c557" },
                { "kn", "4a2d764a1bd5c8bf005a297116bdbea31731c87aeab79207907a60bfc313bf1007c72ece1e9ba7aaeeb0287ac070ba33e024ce3868ff9fcbd57e55f28c6e8389" },
                { "ko", "76e6ba0289a2e1bfe83169658175532734e967fd925beea3d2a49a1e803fca4901cb476f185272b1ab8a472b12ab842fba121239357b138fcd2d93d25792fcc2" },
                { "lij", "a904da84d782e6c1e04c6f14700d5b9b6fce2117c94bb3dee84814216bf676f0861ea995a0b03037fa886de6677ff182cffb817d8a1950313724596730958306" },
                { "lt", "00bf3447d05ba6d2828e90ec71e35f6e2205d46aefa7e694bf2720630f889660dbc6afb4f6bf402ca62989240c97ec29a0542d323c7394468cf136524fc99a3a" },
                { "lv", "c2e4a678505e8c24a4d4a716fe785e44b61fd4dc2bdfc59b3c197e57ce4b6ccf6bbb866445d3c006254ab1a70b28890e599ba64259790af70fe03720877d798c" },
                { "mk", "3af7539296d516702c9e744ed9d6a7fcbff7a097d1168e34c15b1d6fcb26db2bac28aee7b14352d40ffdc7af97dad288e590db3b3fa12ba5b8cbff50674ee1fb" },
                { "mr", "b31bb01957f9239bda1c76ae83bf382c832d880a79ea46e73946de838baa482b64f9e6ae7600b20a484c0db92de48969ecf3dd6dc36c5e0734fe9f51945faa70" },
                { "ms", "d4c70ebfd5ead2f80c9ea81346e6f1267547b3a8ffb74e80813881f4de986b7250759c53d58686cb5cec973f78f4f86d16672c38c9dd5c77e4a881c732c810f6" },
                { "my", "b8265681029307529579e5de9c802959c4ffcc581d02e026d1a389ee516ee13ab622771006b5fd4bf37e2552f5f2cc04dfa7b7a6927bbb8b3724f0244b5422e4" },
                { "nb-NO", "92dda1f164e269b4f02843164e32cda2eefb552def7a039948b37f242f157761a128d1d9a438cb4834d9fefcabf2845192d576522495dc956670aeab5aaac210" },
                { "ne-NP", "f93627ca385c5b1dceebc0941159eb9dd9a6cd134419ac3ac61b05218ef29591da527c2ccbfa5e02e384a9c7c03965f2e6c3363cc69151904937092c228e5a54" },
                { "nl", "eb8b3482d86017a7630dd561d0060130f28637dc6b49ec382a3e4a9ecc76b02ff0f4676c4519b687ff56334ab39f78b44e1edffc031136d40384b088c5535249" },
                { "nn-NO", "17c048861bc1856abeefe5aced558cfcec274d4488c24338addd5e0edfbcd877fe816913914df0c284a41aa2cfd9eb79fe3e1edf9f566269c6f4e8f36b0c2f44" },
                { "oc", "55afa737c80edfcb9391980ffd7101d1640041f8f093306f03b72881a52d71143e07b684f073a7055c6cc1413205944746e707c03726e6ef8f8d69f9e66a27b6" },
                { "pa-IN", "8818e60204b1ed52011869382a929ae640e8d2de08530eb511eb06051ec71ed74b3e1bb92e138ca5a509a3b7a0b852a634fd548f91d0228a9bd392f3fa340757" },
                { "pl", "fe3a7e16367135be3f9fd4553865e084ccb7e012c89fa9773bdf92f987e28edbdb823e4c1919aa54b5c9ae626a00fba679c47e5601ec8d205974272ce34b55e9" },
                { "pt-BR", "eb7bc364ba0243e94c6f95d7eb1a4e3e6ede7d87061abdd6c07766f927dd10b204475248f8e1b214292a2690981bdb4cd9e1c980dc8a96d8f0d29319d7e53e76" },
                { "pt-PT", "95196a5b71e16e34f24f8d2b3288ec879336f83d080e9ffaa8c2e8b255ee6aff6b5d921a8b90af02c0edb767030e7230f746a7a03a19b6b910ee0f446b8ea597" },
                { "rm", "7659b881d4f82bb3f8dbe5cb842e344c64c722f093afd62375c78753619ca56f6d4343a17814f1752d0de7de6bc52cf44c5f4cd3f03ca42be4beb220a737155c" },
                { "ro", "6fa12aa243672e7d085f96b0eb821edb03f2ae227c51a8cb062dfddafd0ca0285f60885dba7fd9dd20af62e7c1ba8f6849d7353c0f90ef7b8493e943b8369e6a" },
                { "ru", "4b03d36b7ba2560aee6a45741a5059d030997f9b9e1dd55209713b5de68dbb385f7ab93fdd637772ba2b8f92de0da5995e6b344a686d200a5fd4a1e317fc1de9" },
                { "sat", "778578fc74151e84187c59cc9775fb33ee7930bcec08fd29418125d74baca19aba21dcc4ea1adb5c5711c530119ee1ab61c5d8d691852b5e9106796175ecfddf" },
                { "sc", "b0be047542a00e21c982871c7eb0e778d449d213153c327a34c4cc94bc4d1a75fbe9c60314c5aad8bc0588de1aea7619a742f747cb9b9dddb8dba89bbb7b7d92" },
                { "sco", "96a4d8c6aec6b4a19ca461f5fdc9e5d58bd421362b0fd979ff16c529a37b8ba366b1704c12edaa784eb0019252067acf4a8108f6bb6d3e60f94a80920278c15a" },
                { "si", "abb0d2ea38537b55057cbf32ece9ed550fa394cda7ff3b006b108a99768b52b1204d9c5ed3979a070d73dba5b2d5e766f01c357e197d8e34910a95d6f9fa8a54" },
                { "sk", "6d5b5937ba0806125d0765b41ecfb229a17a64eb038778a0eb339a33669adfa6c030c4eddee1909cf2debfaafe5ae626bcbe6517c69ea4be8549ba0aa8117775" },
                { "sl", "4418050b2873d902e217691c539aa872bc7af8622d8f9bff9d419050c0e9f2cee05eaf3389231662638eca1d74a5bcdcd117f7696bb6711e8f05a1c76af44419" },
                { "son", "f409c64233d707096576ef3c3ce24964796db7fe441ec12220631c34b721d43486310efe8048597af95e37cd2ff6471d5c1d443525b99cd357c6f6cfcb1e7f35" },
                { "sq", "193a1296aa9a21e22b4745618a691c5decf8d5c92d430507929da3e216d1d4a5e33b0a5deec65df4c0b8c51bf07472412afc126550bd4ff8849ff86538a74703" },
                { "sr", "d8177912efbdc3b8b2cc15717e08261137ffa61bf0a647ad781947545001a8e7f374a6196c30420d4c59baad8f755ef2d5fb75a9d7ff6c8fce839afd81f7a242" },
                { "sv-SE", "d5bd513c9a7f99968436356e7c6e376edd7f69882d0398059f1beaba764f292adbf8e696c1003696512c507bd466a95f6e424d588a9f90d96483de14f6134dc2" },
                { "szl", "ec07278fb14a8f19e35b80cc6d28be3a99f348f8060e517bf310f93f3462575c9af4bbb0517907d17df04cb60d2001cd8dba011277525ff6ac25689c212be8f6" },
                { "ta", "cba90f4be959ee2459b47ea205ffb56e412018f1df4a46e555a3438f0bd412c88264293c0f1ed8ff69789b7e1e2e03da771edcee82f75cf1a3fd951b0768163f" },
                { "te", "71033028653739f590aa22f75b1d669bbe578e1cf05bde3d862992677c96b56ba44db44a5310801e62d49a4a3d1fb7a6dd9d8734006c5b2723fbcb37a3da8634" },
                { "tg", "9e7f777d70339da85ea85202ec20c63f7daf436949fb4e9a5ea101b30c6f6b120766ba4a2a7c5b447e82b9319eec975d3aa4edcd8610970ea093cfc543b961a2" },
                { "th", "8fe18ef91082b0599957f6859b6f62226b7a8caa3c89b9b9255aaf780aaa62760e1b6cc4fedb40d3f8961cdbb68c55dab44fd562d9786d9694d16a504692de84" },
                { "tl", "86c34e88522e8f59f79cd87675a0a11561fa043e536cf6b86bfc6c6f3f4c61bc817f9a29965e0d22c1317c5b100286771f49a23cf6af905d315451e080f48f28" },
                { "tr", "c0540b32278e7ddf5064ad1c42e08612daa2ed60d2e018605b4914daf4bba5347a6582cf8fca8fa6dfec8494d24a78c0222d9c4fc04c313baa2454bdd1b73279" },
                { "trs", "9dca70fd83a23e44504bf5628639b5c13793602add19e1933e8ba62f7bdbf822152471d7cfc39419e79c5d6c577af48dde11899ceabab9af1587cf63b0fef486" },
                { "uk", "abf1af4f2c7fa3438110622c36fb5e234dd2a01c179fef910a8d12196f3ebfc4e1518ad2e43f27eac39919fa6c82c63481442f1402523b33f83a3d1c7b91778a" },
                { "ur", "7daf2cc5382d439c472b98ef00fd136b6da8ff47bd4032763f3e8dd9d5cc34e32345d1e84c7bf9866b47d8f73a8ca5850709b8c7fbaf2a8a0b0177f5988fd127" },
                { "uz", "a5394e573117a5b59704a7b2cb7b341e9626aaf309e2cd69e37d9cd060da07e3f865b4e321a132b09f134f337cb33d7b4270d6a0b3d7af2955fbc0bf85834711" },
                { "vi", "7712a189998542a539c0b80b65655d2b5f6118bad54813994ef27090614ed8f67fb361d9dcf2e1a43d74e2626558acd59c941c76546e31d8891c6987af3b15fb" },
                { "xh", "e54a722f3194a164f1b6e24f517ea8f348bf20836abeaeceb4e0a64682546dc05feda7597dc483ff83cdf76787a43d85de10a955c7a777a68ee84eada1745a82" },
                { "zh-CN", "c5f356d4bf6e47a755e9f8893edf8b4eee311ca7f27bcf022ddb2e33944f3d0c60a78d2cc569a4c18f31d40fd8e310b4926bf245391e40bdebb050001e36beb1" },
                { "zh-TW", "d9caa22a4dc4c7318783f5fa79cd011d6b361df7934dd61682a8de77c7cea57a77e051281369afcf0a4e28b30c49d89d1bb54e7e9b44aee7d1506b3469a74e78" }
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
