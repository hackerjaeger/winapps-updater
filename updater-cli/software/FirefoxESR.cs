﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017  Dirk Stolle

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
using System.Text.RegularExpressions;
using updater_cli.data;

namespace updater_cli.software
{
    /// <summary>
    /// Firefox Extended Support Release
    /// </summary>
    public class FirefoxESR : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for FirefoxESR class
        /// </summary>
        private static NLog.Logger logger = NLog.LogManager.GetLogger(typeof(FirefoxESR).FullName);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox ESR software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public FirefoxESR(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException("langCode", "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var d32 = knownChecksums32Bit();
            var d64 = knownChecksums64Bit();
            if (!d32.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
            }
            if (!d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// gets a dictionary with the known checksums for the installers (key: language, value: checksum)
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/45.7.0esr/SHA512SUMS
            var result = new Dictionary<string, string>();
            result.Add("ach", "565e9db65b474ec6a80de1e28d01313b8a55b57434cda1df83b60d6253729abb5764ddc76867a04044ccae0aeeeaca57c8aa1569482b562b086133d04f5ea89e");
            result.Add("af", "412dbfa5e9d9b0662858f147c9331efe16938907035b88d52f482e2a113dae89ac931879fd6103002175d9eea6f71d60069da87f92e49e744044a8097fcb06e6");
            result.Add("an", "7a237e840c27645d3d84f84bec38099190607b3bb6fb1a806bc296dc76ba69f6f1ddee7c6fba171007492145eee493b2492228bbae1ed1cfc3571f904eea6186");
            result.Add("ar", "2355e62b02e8bcc2626b034ba4136879139b29ea790af2d811ba4810fc36884539c41b4edc032a1b2485fdf2abe79975c0c277cbd3fbd103e1c5306f884fb70e");
            result.Add("as", "c71f6c0a9d33dbb031ec86f3c8e3f92cf3c65363e8c70bdf1eef24969091e1099516b3caff6ea118850c2b952e38de6509815d0ee943e1b80fd26b12c9185efa");
            result.Add("ast", "4e447f65477725f6e413d1c8369f7386fbf6c0b18175a63dda58338b6b5e8748ef69aa071a4764b867cfb467edc4f959ebf4fc08a31a000ceeca21bef675dacb");
            result.Add("az", "bad2e97af260b1d15eb980193661206a01a5daedc264a161ede9c32173313a6eb0d953755ada536f0b3f10fa710bbb7c76655de533fab5a075bdcc1edde06417");
            result.Add("bg", "7b0359a2988e4c7ff967e7e8b391f95e78850b829330f53fab07c8957bd5c30f33d6e3336803faac7aab59258a20d4d684f3ef3bcd08f3c1d09a16d80da94aad");
            result.Add("bn-BD", "b3925c9ea82fd7d5e239fbc340f2b9cd697677929e926406c9f173ee7c25072b0db16ed8575b76231e36f65b83f3b39c160023a5e7a785c136e87fd04bc19201");
            result.Add("bn-IN", "fd0f7cbc620b3272a36ef72937fd95c7687c3866196ec089ed3d10c3373f47b8a3e37df6dcdab59989841707333eddb96ba0247e2bc55be868591f09e03036af");
            result.Add("br", "0b40c6a4bfb7eca34323963f679fefcc0a9281b67e6b2d48ba7b0dc8376bae15db054fa4275e54e4578487a2777a02aac25eb48d72e79d061bcf556aaa568fc8");
            result.Add("bs", "e68b9349b6fdc88f9b2112d08ef8190e800d2d9a1d1d9ca4b94c90405d7a24722ee9b9309a7f4fa366ce57f6529758e939350a385cb081e412d869bb7880e743");
            result.Add("ca", "a4a6e98d969bf190bd129bd2f9087b5f5a2097d609cd3014fb676b07a72bc0b4dc6f45c3d4baf5511a7bc890bbadaef81349a5e7d02fd8ff2582fa75212ae87e");
            result.Add("cak", "f773dc00e7bf6501e100ef7662ed6134aa0ec01a56562095f5040633f3d626c1a2510ba713386f151dd7308cc2c6f8d1982e978f58d0c1f729a2556da2952de3");
            result.Add("cs", "2bf1dfd547df7eed2415b199001f7eac20cc3c91a631c59571e940ed1f8602f0da773cc7346dc0a178723c48003f9af2929ec3caa23803f9310b18dc32cc83ad");
            result.Add("cy", "98ae03ca63e70ff278753d5feb3b0a3675cd43cbcf6d9365beaad39f73f0053e4e3fd035a38f2b6ecd10e6d68617b6571acf33ccf34b9a67505d87e9d7a25746");
            result.Add("da", "2aacb05631e3f35b14487cc25c6ef51c72d4354785ae6d8aa74ab0912f37103eaae3ee1427f7bffbdd590a3f6fe2702256e6596f2baac48c414f4ca28362f643");
            result.Add("de", "9a427cf67342ce27035214de2459c271ff21ea178fd6a3fe9eca7ab904c12cd90e16c5a218d66533b05585f60f4e3dba365144f454e17e35dcfce99785e394ab");
            result.Add("dsb", "18639b9018980a6350b58233ccfd877cc5b22d427d466dadc2f5ba9136de7cb36068a9b994056e745d5a39b62d8987d9270198cc3b5322fa16f92f8143267bf4");
            result.Add("el", "66d2da22bb94acefcacc7c2b8c13d0418dad92531cc434aad8a157bcf8bc5ab1bb91f6ffaa452e31ba6ff0bbb600477c3e29c86b8baedf080eaa9cf189bb8282");
            result.Add("en-GB", "cabad67fdb706804a2a1bf4dd7563d679b462b5927cb1256b53288cb1c2242922f88a253fcb5e29e034f5ee53fe560f94e6ba096ae5161d340b25e1dfce60dc3");
            result.Add("en-US", "a6396d1002513ee6ec9889cd9289e4ba62c8bd8b7f860ee59f9d817a8a91566d25f8a79dc16d810386f806df8bd402ecb7ec03136f7e621769aba30dfc0b0eae");
            result.Add("en-ZA", "fd6867d9420d36fbe43ef75d531c8eca1ace7f9e502ba635e48934a2f23c9349f844ee22f5265d5f9e4d2c3a3ce33326b1d708e3df0f86f4f538124c280064d2");
            result.Add("eo", "23ca68737e6657e846f237478a390501b839831465d2fddef54f5f2adc87e742dd0fecfc8f9e80a34e8fbd46c396d9c71ddfbb5cdbf0ab34535ba20390b3533c");
            result.Add("es-AR", "0108b885a40d8db67a4e875ce1a5a6407bb744123fc4619b24b0cd21841cb2b70cc39a191083db89684839fcf2bb8a7a93affbd88fb191f4418f037801d93800");
            result.Add("es-CL", "bfe2d31ff21c1eca06705acc6b739201c6a3fffc4224220184a3fc9050ee28c6004683fa0941416b443f6a658872715101e41c72307b3c481911ce5012a166e2");
            result.Add("es-ES", "80867a630c99158a6254bc6ed0d5e93089f16d00d68ce2620aa9d598f8e50dc5c7f65ebe8dc085e053e321534f197cd97610a935a53cb20cc77d069fe2f8a31b");
            result.Add("es-MX", "356d19d746f18e176b7c4dfc518ebaf39e8e40e9979ad03db77d8fdc9c7aebe4a3155111b824e7ae5f51d366b47bd4d832337e4454b3a977d46afa505c1b25cf");
            result.Add("et", "3362e90c3405c7ad0aa13deac01beeeddc9fe5ac605b713911ed48c3dbb33d11d8ef17da0cab24a87980a428dc5b3fb5011878a78612b096f197dfed47104705");
            result.Add("eu", "fc54e5408eb699eac2f8ea25fa2e9e741961a421a02b736403cbe34697abd831c12d8584cc25dc42de102aa66fa050ab90fe31049837a16df9ee014e2a2c66e0");
            result.Add("fa", "4ae12efb8c42809ee737806750719c2aa474b490348cd7b73b238b43261b57341d0b057beddf8d39c9f3dd2d7ad4b65049f40fbc24ba0537156ad28928306fab");
            result.Add("ff", "5ab550b039f7a47b82301f6a2bb7ea0da241e1c813fb32d8c415f503d5dc8476e84f00c180a76de9d688990a4bb8a0af610ce24a01f68cca39fa2781a2e147f6");
            result.Add("fi", "c7126f381a676f36019129b3b86640ffc0adea8bca5832921b0a80ba0d9aa3591f2851170580027250c405ac6c73b05c36dc06d3046f012db12500947a1edc6a");
            result.Add("fr", "a1c1cffe2fd9f069f567545674b33390036915b64674a7f5fd152d74ced1d216cb70c2a7b97a757b62105e6a58dd16f75e7ae7dd2972ec0c4ec379cfc288fe1f");
            result.Add("fy-NL", "61059c90a21877a5aa6724e366fd51787d506732925761000258402838fcdb0e9b457dbe6b5249725f67cde899ec6efb3babe1bc01cd9f7f3462880d970c172f");
            result.Add("ga-IE", "df64b7623639576a429d55c41d20c5d222b08b816e65c46253874b93849bfbd2a429e2797a7abc666fc3e2e607414b3186b35ec2cc720f619c93fef1dc1be372");
            result.Add("gd", "ccd98a404ef89cc7ebcdb8e511ccb3b3b2bd48ac3d268f18f4d5b96c07550f6ce6db45c60c47d59c93f77f1e3954b4dbeb19924ac68a0e3ffdf1443a3291109b");
            result.Add("gl", "91fadcd6797d873c7e517a34fda357dcf7a9c67aaab6196c5744870ab91cf364ddf0bfd88b65b601370c06f4c268315dc4e9e4a63fb679b2c62b29682cb40c00");
            result.Add("gn", "78fc174aa5bb3fe9b40f24766ad037fadb3c22a24eaf3119cc8c05bf5cb56145ab25c903ab524eab54015d92fb84212d90a485eb93f88ba5df3d5fbcb5275ce9");
            result.Add("gu-IN", "6f1a0f54283e1699fb33754119197769cc4cab3e198a4c4a93af53609b658afedfd29336c60d2a987dffa4f42bcd1be4e17ca8e2cb78d8d3727eeeac3a0a7d86");
            result.Add("he", "e7a3ef10a0454dcb39d4e876458677a9d2e5e50c8554983d03184706f52d28b72faeb44aee8aeb472b607a4c8c3415baa24254c7c3763497dc990b225fcf2c53");
            result.Add("hi-IN", "fab65a7677a9fafbbbf837b29d6fe6eb6ab0b78127d3384abc10efee82c03a2c31be68c1d6963d518bbca7c0e1758585cd2d3e690f152351ef95a4719c681f16");
            result.Add("hr", "bb685b6c7fcd35dfd79acf09cb9600985156fa70f276eb9ccf61825076b0b3a272aebe30f5949434691446333b7c781b757ca330af8003916b4fac750d1a5ae5");
            result.Add("hsb", "0aa03e82d5492419dcf584d55886ff2909896ab3216afe4e9d609bb00653bb1d5bcaf2d0297d00f9dce48a6574001f2b296fb9c1ddda222f3fa23965bbd3e691");
            result.Add("hu", "c755af459f7240f7e1adea3ba0eb1e3d3d23fce15c3b01a073b3eaf63853c4070144693d8371627fa612d07ed7b54daff521bcecbe40a8be23a79043de45a241");
            result.Add("hy-AM", "899acd662e6794f955085b212c236ba57a39a10c09f7233f04d08c95e0b67d412ad8bf1fb3fa6196e5edce60dec127da0e53765836194692e6fed7958bcc2e66");
            result.Add("id", "0fd4ac1122a90f615252bcf46694e17c263ff0433a8e565073506265b0e9b1d296ab9e8d3fbca338d51787bf9098a3c7e39f5805f6c271f8fa2b01d9555bf35e");
            result.Add("is", "983f302733e13c945500d324b027e10ce564be8d0d7bc9c4ac90e304fccc24f4cc7268f32463075db11311913fcd69399af74e82ac33fc8dff7323255e0bf6fe");
            result.Add("it", "16961061e4641a629c0595df95bcea95c010d795daf37e65e46c4caa4aeb687781ae57679c0bcf4a9024fb7c8f927236e86d231985bbeb161c9c8194710608f8");
            result.Add("ja", "a552a2dd4551faab461e3f2d897232c5be7306b5683634e592abf51ad5ebece5979a7edca0f1a371dfcf7d747c12c2921c03175e7a1473198005769d63967a43");
            result.Add("ka", "00bd01f581e2c8521177fd7d2b576af0cb59d43b714bf00f30d5164689432aeb3dfafdbcac9d4014d1a05c2ab27d11e7290734577e458b80b20a8debfd4645b5");
            result.Add("kab", "0fd21d48b3fb96a5c868e86afb8b2aec6ed8edf81a5b1271effa65cd99b2963b6111f561a77c603fbf7a6214a91070bcf33c8fd95104dcc4c4db5b70cdae9426");
            result.Add("kk", "a8512fd16d8ea3b2d0386194bc759c59bf7173066dee16f3de866afccbbc3a61ec5fd902e18d845fa322f5db4605292bc01b96c0ad8fefe12a821afc9955f1db");
            result.Add("km", "54d1aa9f94686c56c5efdf9fe45715c254c6c2f9d63f020de07591652dc035bcd2b8148d4d0c1cb9b330f93e2d6d73838c796d1f85944efcc0e4598e6043c29b");
            result.Add("kn", "c37a5ac3252fe308555e119520bfe45ecb7e13dec6606454dd723ae7dd9742f5787ec3011e03361eb04b1cadf3e96c11e915870885bbb86ecc23e69cd9046b27");
            result.Add("ko", "b72149b08fa2142e32b5ff01a7e29d24959bea74cc68da16ee59c6b7007fc1ded85f2f6716a2ef0dd165063d1a837dfb9898b1d470acf4384da8847e014b5196");
            result.Add("lij", "77a2bde0d2085795ddfc08ed89327212650e6dd15fb50d188fd25c4a7ede18b31572b233fd2d70a1cefd53a52bb623546a53a78e03a19a2d58afaa2c408b8436");
            result.Add("lt", "6374d7c4226ce105eab92ef83bdfef08708394d0dd3806d83c71f8ff52aab07325451671101fcaa0f177b1004b1b8b37b1f7f3f879374195e8a1eb11411c4ae8");
            result.Add("lv", "0cc13c610a580acaa94d9122ee2b88df4c06c29e1422ae3d7565e5f51a2667a298aab472c5bd344ecc558f2f9b5ec3068214b6903fb1d7608c3d421d9693bb70");
            result.Add("mai", "64becce66c2b32ce5ee18c453a233c922d306f915443ece082a2f99c0e35bae00481ff3ceea51332716e63549bf9528121b7b23237649cbd4bdf9ea25faf0b7f");
            result.Add("mk", "353fdc8f4210c8dbe15388c4ae261eed257e42c8968a6e0551b35cfe37ddec271634d86d2869df76ac0eb9e7760eef96e264f55f1f8cdf4503269e40319c3541");
            result.Add("ml", "b9dd0d093edd254714d5b64977d1642a1bf99304bf0873e437070d225af237429b397ec1c9b86157ca1660c4a6b81d773e37433d43990ffd5b0a09c72ddf750b");
            result.Add("mr", "a90b262d808398b315dcc47403efd727c426d3524f78503b96251b8f7dc9197eea88d6ea4f1ff18f777e65ccdabb36e5805d2690e86d3d97e9a51387a49a84ef");
            result.Add("ms", "ef8fa8dcfaa3c8f0869fb1e14413d5c531f21869b126ff762b4e371ae6409398ceb5322bdd2d1b5817753024214aa53ded582ea1c5f49a1e8aa3d07972ea24f6");
            result.Add("nb-NO", "baf37545ce1f3553cc459a5bdbf23e84853dfcbd5e2721f0b2ba3b210c3a9ef067010ca12d3bc928608c144b8d47449e0c53db9373354520c6f8492ff74a932f");
            result.Add("nl", "43b6d2cb816daadc26fd84a6cf1f3477043cd0afff739d7444c4cf90df7b1a53bd2ea2bd519e87c7bcb3e345220c42b56d3508a9728c1def1829de57d9af3347");
            result.Add("nn-NO", "df079e63c0eb6fd4a73c21b375b9dc05657a03fe0d3ccd593c541f90be042693ccdb6865b6209f1bcc114fbdbb83a82d840e23c45a9bf567ebafa8044faa4624");
            result.Add("or", "8c37490fcedca841cd433284ca6b5b519948f124d2b8fe3feb11e927f878eea2d3187ec0528415764b329d5d33ae770e05c4212e6cbf1b40d6f84e030b98077e");
            result.Add("pa-IN", "74bcbd56245b4b77bd9e71ab6c5fa18663f6a5307116f9cd9f7d017fbe4bb086334039e42638b7485fc1237431a9ed8e3bbc366c8786a7607b55ccea97d3589d");
            result.Add("pl", "de60684ec98300b3e630a670f33bd4997664847931a490f1d29c09c1df75fdfadc501dff1cacfe0a495e4caab297c2cabd309f440195e547c68f61ff4c3f0148");
            result.Add("pt-BR", "7866b0840ac00fe7901e872cc547164e35ab0ba0a406b659fb87e56c2c23517373b51e930ed4a6f678f3b3824cf634fddeb4e7a1d220b00f8a16b97870ec387d");
            result.Add("pt-PT", "aa38e3226dd676215cf3a84cf850c2a1cc1792525c69ffd0dcd1a49c5aadae2c897495ff26cf16fdd7e218048a687b5c632e68d9deb287fe4c4dc0a4bf3776b7");
            result.Add("rm", "75285176baee53473f4af2195cb7a3f38083bee5e6ccdf4dca614a83dd5a429aa2f7809cd71cc03c9bce8ab44ef277deee2b219e877f2f468e45b2c97cf012dc");
            result.Add("ro", "3723f9b6c7bf715d59203115a129da89ff96b91976fab41366e2331405aa008f4747e751d69084bb1f8af1712e1245c02a5b9cd947d7ff790d5efe00b2a026db");
            result.Add("ru", "c96ef65d86b9296ec159cc175c55a92f1b243b38413d8302fee7b6fe7955bba39e62f65cc88676f12b3d5c9f0867c062c1fd0445453192202e90c11689fe642e");
            result.Add("si", "cfbe6987a324beba3005d87efd9519c027fb689a7d206045b24773f747eefa0d3601c65c5e375a500799be73ebabbcbe1eb0e8d463bc05b64347e9015d3a089a");
            result.Add("sk", "1e8813f96be8ab81edf6a4cfb3d05cbb1c2404d377255ab43a794a1d511d1e0e01b2a545b2afc9530ce980264bedb55b69fec1f28fd3fb0070362615cf263409");
            result.Add("sl", "8ca54b0aa9042e891bff1f3dc597ec44ff615c3e45abfadabe4e43b0578ab77fa290390c55ed0064214c30b454af989ec335d9b0cebe7aa5e4bf628c4f544d9d");
            result.Add("son", "f968c3ec1b7f645e72fb8faca368608fde09d15932c3244261c801f0709830b51b553d7538edb9a3c02822b4d47463ff38c2300d8c04ddac03a3b837990b96aa");
            result.Add("sq", "1ec0a4590c0ca153c31dd4bfaa3c315cc33c633e39972a74ebe168ac78ff7d7714b02951e3ab9867f87a320c40101b502264cbd1a4a6b5617cb762ce3f23490b");
            result.Add("sr", "3251de2e325517eccec364b48c512f68107d8eccd2f27c9e7bcfc9dbb23e75ee4ab5c558d4c93500276364a92c304ce643d1be3f3695c8726f9f54fbae32821e");
            result.Add("sv-SE", "1b6379e7504f64e3626e12e24ab26e9b02a97ebae3656698e486b38e3f86c32a1adc4bf0da60bf0cccd6411f33d7b37bb2cb8ada6884368c27a94bc42633e8c0");
            result.Add("ta", "3220a96c59935201afcfe4c60e765c23507b8fefcd26514706be0b6914183d6d694020fd428c12081d198e0945821ef9e91c3576992fc76d977e60fccf64eb44");
            result.Add("te", "a63f5a9b1ed58c3c0a07c555112f2b1c4d646630969593027a66949af614e674cc594adb512930b8c271e234d1c26cb82720f743486703dc9802ff866949ff7e");
            result.Add("th", "6f9a11fcaefdda6759508e2ec9f1dbd616f789681cd11209f70971f41db61a478536fd35674c1e9dda8c6f81dff3b70908a00f2fe7212ac1a151b088d681b924");
            result.Add("tr", "2493f7b0305a40dbb0c177e2776dd9fab37ddc8572f9697ef946e236ab57f5a2d6ed909b6ae421091e7e50c64012556c26f73c7f4c06220ff1cb46c5a99bee92");
            result.Add("uk", "2c7180d59900784d37cefe90536d4c3e1fa99b9a4e6e458275466376627785b80b8362b397c6e4c7fe4bacc8316ff088b83b7a28303afb23e7b6ab86c9f5bfd7");
            result.Add("uz", "d3595f9c71d9ed5072b8716dfb2c6888d4a467333eb0c3562e41418f2b52b41d3cdb3dcb38ed061163d872f992a8627a842d8edcf1556eaabf1a55892e348c70");
            result.Add("vi", "e2e150153798ea5d34feb5c5e18ef59c4650df06ca8ce92365434d98823733b850d275f3ebacb878f37f8218d83e9f73f20483a1d89700a4e1db5042cfde904f");
            result.Add("xh", "4a67bb820d1207ec917029f9559f41a2ca9447d70ee43cca7c2f9ed9fc2c8e6b4bd8be03be5ec6f50e0eb8469e77fa362f0fa9f4f760abd63778aa7db4977548");
            result.Add("zh-CN", "14f4f301cff2b136935845ce7418f70356d7d1f081a36b24c8e40b4968e58d9ed9a82c7ebb91292b948c1f478ce3911ef10eb5471104a2655553a3343b0628f9");
            result.Add("zh-TW", "e5c509586e8e05fc96cef0e9f35225781098a57524d62e9e11bd3ccfdd9c631bd1dce86889689a77c7ab8d02c2cca946c1042d0cb1dcaf07fa1ed77a15de26cd");

            return result;
        }


        /// <summary>
        /// gets a dictionary with the known checksums for the installers (key: language, value: checksum)
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/45.7.0esr/SHA512SUMS
            var result = new Dictionary<string, string>();
            result.Add("ach", "dcc011c3dfaf9578ad210bb71a9c1d0c603b9ec6062884cf225e87dc2504ac2afadf47fb1a04cb8ad662beab027e31ef429f15a2adad74fbe8a77534986d93ab");
            result.Add("af", "3010a5ff76d8fc895f0d33c2bbe38284579757a183815a79bee3270f60f428ec1a1c3ae077365391aff952d934dcc5c9c2e0370f23f39c8ab06abb1b75433750");
            result.Add("an", "72b77a4e61ae84a148a29554c2c24a74b7479537a30e78df56a28827d58d9fab48ac2d953bd03ef1d6bf5df8edc820508a4a38ebfa108817d1dcad7a23f88696");
            result.Add("ar", "6aa301d429203b19ffc83442ea7451182cf360910176b64649439f8adb8c2cebe28eca4820c13c932b5df248ef02f52f9de5aa35119fe6ca62c91f90a0377e69");
            result.Add("as", "776c55f743dd88578a767f97292a266d75e8ed47b9d1f461014945f52f60b427b280321b86d6d79b852321f4109832fb0a658d1167a12a3965fc2df8e4d06513");
            result.Add("ast", "e9a0d19b57d2cc1926882b5d81300f570100925a744d4d980ae754c558194b45d5193320e19dca4b06fdfaa283eb9f302455a3c7c595d5ae95486bb036d3e4c5");
            result.Add("az", "0cd59f059b93bce0a20be45c9f1550fe631684f0765579d24a5a8c18be72bd7d02dc13625b1b6b34eddf0fab3839a67fd25c68f6555efd801c780a8ab7b7b8d0");
            result.Add("bg", "2910b63b6f5264d5e02077a8a0962662947db62fbeee64e0c2d40f5962f8528ff0120ac3f763a60a9bdea433b99c163f3bb7bfd343245074c4249e4ac8b5bb89");
            result.Add("bn-BD", "172e0f4592ec44202777afea4fffba538fef6a4dc4fa55ef42bfbc6dc9b0a0909d24cf7dbc395f8e84fdb24b1f79ec651407f10573a4318220336776cbac7325");
            result.Add("bn-IN", "7a8cf3bc2aedfe87be4e7323ed8e8e9a7da6ef142501da68d787caf13e0d1d1da9e3bb0562690afaca07508299ad185d0f286e1b38f1f697dcaedc6d54518f20");
            result.Add("br", "881729c2a58ead3a3b84126e281f7addde66454f2d4c96916cf77d9f73b8ed6959522bf461a99a393d218b4f37f9c4e5e8b0e08d99ed03100a6f1e96301acdf3");
            result.Add("bs", "042656762b84f4480dd1c9663aac5f638242e79ecf6511d570f6116a7b94b0743b801df64dfe7976fd6ef87ee2a06ff02bf474ae20c61885b36bf03ccd3ca38a");
            result.Add("ca", "916f96bbdae51427465d265acdb78de49025618a9db234ac0d439a74af0bd2aeae3ef8faf5ae193b5836a5a54eda5a6889192d7a4c9e9c0316fd24f33b20f2ee");
            result.Add("cak", "618238562807b86a3527cfb8a9839da311d4f981a951bc9fa0352bce44c7cc8aff46530637b8ac13cdc6223f51858c548a9aebd4987466a3d40d967cb6c08cca");
            result.Add("cs", "c5a916eea6057a48da207b106727441190b4a701e4cca2df21465ae0912f2ef8c74d2bf2df8c156d873dc8f0017d6ad188c313c35a9c857180d5347e5b6ee191");
            result.Add("cy", "3c20582fe8d0e72fcb5e6f06a67b6ba2a5f9eba498be9320fcd6a43f9fdfce63aeec2ffd345f28bd0e9b66e5b1ddec9144d58e45b9decf500d2a8b0ad629b9f1");
            result.Add("da", "e8335991840b4c3c1586cd8da0b5744281470776a4f875dd57fc338f4392c52bff9fc9b0ebcc53f4a2c582c6b9fc5bda640ba647db9b2f728f40099a9369f309");
            result.Add("de", "727823d38dd6aeae8374a4125d9c8f26de1a5f3c66a5aa9a63c2df29e454b1a2eaee567400b48acba52e2cdf5af3cb76d9b463a32ab2b57b5555642798bcf138");
            result.Add("dsb", "8ee95e69c53de7e4452ab88302cac0474306016bc0fdeb70547e5eaa86eb22f0fb99e95045227af265b1f045961c5b33177f392feeea07eefcc18f9260803096");
            result.Add("el", "5d0a9ec29c6db312d5386025f8fc6d30d7ad4542998856832873b6aeddb33433d1c5d37a67e3c4007d5e591df9521625ff050e22501aa942b38abe961f71b285");
            result.Add("en-GB", "677238b326a1fa8c1895c4481f9722e49576abe12907d52e0d1c2ea301ed092044a7d3d4eef4aa449e5be1ef63ed7c139508fbbd0485bf58713b13a7ef8791dc");
            result.Add("en-US", "31d19f8e4a8c9d38df3b2fb651f9e6aa86c47d07c31640eab503d90c4bf36823318be4902f3bd3ab76c850f32158e7721a899ed5816c729b33b4fdbd841a5656");
            result.Add("en-ZA", "b22ad541861bc8094926cd38f7b9393b3827556d4176bb849679932b9ab3f27c82128d5ee00deb51127984b78d491bb261d75240d900805090f38f17480794c8");
            result.Add("eo", "6580158974540265885dd9e9c17d115bf7bf95b554f44974582e0a68696118906412289a1ad4289679159f35ebfe3be25f2c0881a42c5f7f1defd9d5214dacf5");
            result.Add("es-AR", "51ef284dd7a9560a7309dd0b0a57c9f849642445fdee2c938cc3c657665974a55a6ac9f0bfa13a01ff810dea2d256ca95d147caa5f99bc35dd1ca63b8968caff");
            result.Add("es-CL", "17548d93d7793dea41d625e33e5d11a8ab5076bf6ebc51a3144a04e56c823c5772cbf0172d31ae4a5c210cb2d8641f89273867d1842f0d31f90c98c5cb3e434c");
            result.Add("es-ES", "234c28272d268658aa5b7ef6ce6ca36948116f6b4a7517a5fffe76d9da556b4526116c64dddcf08eb6e73db27590ec914b1e8a48db4da945991bf71631176f2d");
            result.Add("es-MX", "bcc944956295e5490411b8c73f6b3d383d1022dd64a17f84e4ad51edc5ce407a3523833f41e15f32846843f27622d86a3f3fd8887deff81b5098ba91bda9a8a6");
            result.Add("et", "98b581bd4da54bb8bd12457f5e0213bd1631597f8d508dddbc3cc4197f64f916c686782ff458af4b543fa69085dc9c2e88f3c3d14490742b1e4d9043fc119ef0");
            result.Add("eu", "04eddcfcd5922deeeeef07bc5fd998e23ccc0e6b27a70453e13555f99087756426f16e6474f64cae57d09f65c4fc1b655dcf08af08348c4ac0886f543df6a2f9");
            result.Add("fa", "a11cdae59be8180eabe9ab01bd2e9b8e6628e18a6be7fdec65f8253eaee942b5f2e5ef0e273cb096fcc450d9efa7a3ed7b1362be1ba45466dd911f07ecdcc90a");
            result.Add("ff", "98045d868be21db0cb080549b0e5293c0d0467ba10174dbeb73c5240be6dcb771048afa6791beec4ea5453d955d79cb94104f55632ce6ec17b2015615964bc14");
            result.Add("fi", "bc6557ba213637e15409b821e312a573556cf6e8b1d1b719a927e6eb645cee3de84538d0898d329743459c84ea82a7d12c67561e6f5323d1e84aa364984bd399");
            result.Add("fr", "f1b12c6aaed23313d5476fcd62b016e8ac65be27663c24493b157b979dea5a1795392004305a716ee5ac6a576f52818295983861c938d9d4abee9a6df221df28");
            result.Add("fy-NL", "f89ed48ddafe59ed37e048b61cc3d2776cd5a25d3e7d063437b05265e451c1b6b27715ba6f1e7df056b67e4ae93382f77c975ffd2a71521e8b26299ac3865515");
            result.Add("ga-IE", "551de4c551ecb026d4428dc633a5cd36810546eaf93541247852e6c055a616c421492f4fdf1d61c1eb10bc20986940cd2d10f1875a1070f87864d686b639a1d7");
            result.Add("gd", "8da316e675777a5c78c4957685540919b82bdd2325ab54b37ab86da49cde9d953f4444ee2175830d8e6eeb62fd62f015fc73e1a04985985d7f0240591695b9c7");
            result.Add("gl", "0504b8ccbeafa3ef337691a089846bfe1a2e1a21708c57a7e17526f87c2c7c059b28b68327d20a2bc08c1210e1faa47e008f3de4b2c260cd5ffbb53fbfaa594d");
            result.Add("gn", "41a3218368ca268c6c84c1b837ca60f6c0a14841547be73a26c30fd9082355a68c6073272a9ebc770c9690296ccdb156c32545403c4b33ea5bff87ed71f04465");
            result.Add("gu-IN", "6d7498aced4907a9aedefb65d12c5301633aca6345dbd5997bbe04ad91f449203e16061013b49f046a08b7f9119dce741f3013e88e92f8da5b979ec9366efe6a");
            result.Add("he", "6c3a9fe0215f337f63addc4bdda3c6448ae4144852922639a3e3f28160e0b4ca59be927c14b31b51d2a443851b4857597027b7f58100ef75ec2b597958a2ba8e");
            result.Add("hi-IN", "aca3622b1d9a8d3739bf735fb6fa481ffda7e9f17704cd31b62615e2470aff3f279c0008e2e1cdc735062a532c095195f525bbb65edd250cffb86cc8255e472e");
            result.Add("hr", "74eab570cf60fbdc589c45e7eca6696e863fb312e5cadbe2d388d0bd6be502a80f9fef5af8f82c4695a8f392229a5a106ef4ee55ea680b96ee771057c53e6826");
            result.Add("hsb", "5869ea83e35d4832cb964b9e2facee1a9bf14e943a425757e197e0b3bf12236390d3a6012eee3d4c8400948c89ce2d6e75b5bbfc952cde4cfe6f6a00d4a6821a");
            result.Add("hu", "767cf4f43eb1b0e10611e45219dcd44cf783d92ca690eb347ac68c780f268c2a4ade37e80f70bbaa24eda0e28d7059b1b3392ad5fe85e19b697dc1464fba0b13");
            result.Add("hy-AM", "5852765586c2a637e1d80d9c23fbac7a1fbe1aee8bd6aae3beb5627ec9d019cb5bca88374616ae0d625c451e14f72af99d04ac22fca1b088c90af8de3220f459");
            result.Add("id", "6513d0d2a8bb53b65b36ae6d0320f4eb518e665f914142d92ce6ee40234ca2ad1cb37695dd81f14a2696af7e584bf81a300483b6f82dc6a99d322981ef7c847a");
            result.Add("is", "16f23a5598905992d22cd6038fb649bf8a7dce2d60264b11bb947403ccab29e9631ebdc10792dd2c8e5961a148136a235c4769c3754feb581af4bbc9c0e13ca3");
            result.Add("it", "529adb048c2dd2cb8c8431a9dc16b2b3991f7e9536bbaf090294b7fa83402494ebdd4af2312145445853932a5995482f655a34b91321824bb1834b36c8bf3622");
            result.Add("ja", "766bc208a0e108ea920335556b6d5a855bc424407c5856901ae159b9767c4997afa4f9db74c5cf2bd5d8a4e988860ac3d50378e8da09142dd1c30ccf5aefcc85");
            result.Add("ka", "ab6589886992b0c42ef0291e7055ab74548609ead285d50b95b71aaf09074702836303039e82d7f9121011082cfb8ea741a92e152752bcbd6538de67dee74b3c");
            result.Add("kab", "ecd99083e56f33bf86c270c018a1054936420e265f900f54574fbfe8cee6106cdd9896eea2d04846c81bcd02222724c43d44dd16fe976ccd8639d5d60c8cf2ef");
            result.Add("kk", "54ebff0595b0f56aceb962776b6b45515ce5c552cb22dfb159ecbcab2212bc046bc497d87de26be3c74447224fa103b740b2263dfaddc1c4cab3190179c787d1");
            result.Add("km", "ea3a8db78203c7914f2a3f2cda3ce1d7dda98b53f688428353ddb2a9bd92e0195981c22119ca891104086770e8d753c7b3d0e13b99a4ddc526147c39e2b338f2");
            result.Add("kn", "9ff8382bb914d60f502cee7d0be0ff8b58ad1cf14ee7fd58ad8862eeb5650590c11a30b38ff99ce0db530a4eabc84413c46978e287ffbc49b7e36a9bb645c523");
            result.Add("ko", "3f835d63afeaa4da7a1161ad9f236deb3b62bbdcdad8341f9274a0dbf05e4054e7e5190513c03f3d97348f171cd526c80fd0fdf89c65b783d82a2bbd88a9bd1c");
            result.Add("lij", "59d8cb3ea78572457c2600981fb9d7e093ea897dc7466e6174fe49c59f8967d4d3527869c9d851280f471a02d20354d0cad188e4844e057d789174bc94122d3a");
            result.Add("lt", "a88f5299d15170aa4a01545f9ccb61af68e068212c6e6b82db64b74b8f7b6aec620087708b383e6e6269dc59a370f3987ab4b521ec8fd482709820a7c8dcbb9a");
            result.Add("lv", "73acea081bdd3db1f394ce1c2c7b9758cf075c7806abed7f94495ed43c0af45333f525399b82b7499a7fc8696b4a81fec5a214ed7b5f73412610e935a7ecd723");
            result.Add("mai", "2950ae68f66d5fad48bdb6f3c89bfbb5d00171582776980309ec8108d4211208c1dd6a7e9e1ed2d584007502e5283ab8b7f31f32a1994becbba1c9cf9e8fb197");
            result.Add("mk", "dca4232ffa116bcdf24f9737a56d82ec1ab189226c19c54d4262e18fd77bf095d97e927a0e042c09221e134ab756b79d07fc94a486c052633b1758aaebbde135");
            result.Add("ml", "4a21099fec29d68b374f5cc694bbc4fee667750c1257200f64c429baf86003a81b1c6e8a17a511e1c4fd16d013d448945fa21df8c016529616b11f24ff948f99");
            result.Add("mr", "1517c705af08fcc738711e51445930af065e5e37d9385b5e9042552f25a45b95f0743a4bb35e4e886bd5fa4ae7653aeaf20ff011ce1180f3eb3fb5795d5767ee");
            result.Add("ms", "cc5dadb424f3e07889fc0e2a15ac3b084a88763886739abfb98aac9b38c7072b6122e5de88d9e251d4ff22c64297c8c1d36312b3cf77e8d2cfb31266e326af63");
            result.Add("nb-NO", "5fbd2ddfb377e44ee4871e69225c4e35b9c2c58919f10e85bb54941b2fbbd288cf83a8c29b6081726943b053f355aaeafbb88bcaf65c2c49eb26a16fbcd18919");
            result.Add("nl", "996a9f4b587c8e8e6db1e0cef4b61a0760a5338acd18d5a8f9adf4bca153ee6c57d1453fb94a7110224bf19951cf6f4de68b5196cfe19df58e1953130cec0c7b");
            result.Add("nn-NO", "62ad730dc5562edacca3c4e305cc069b7dab7443c6cbe52bbd0a7e909e1a1bcdbad3e1e8d678b47c4feae45506d3b8f336fb0f1a6180e4a8fb1ef0771fef30e7");
            result.Add("or", "bdd882994a15fad90285fe421be4dfe6f8dabeb8609945e6b74356fdc0181438020922abb384d84b7c1ef8f8d14c6573d6904cbec644583f13b48eb5bc81277e");
            result.Add("pa-IN", "02b4dab530d019df232c6ed1e195c6b5265b1475d0f51132a50d15e8f3647271bd8cf46325ac345f33e31c63829d8d3f32d734919a165b49135bed2bd4049fc0");
            result.Add("pl", "7b914bd0e2151dc17440515fc636656eef1c50a0809636a11494b187ff59db4914992e512525b5d6808b023bae8e8d401149378ddf1efcd89137368a9dcecda9");
            result.Add("pt-BR", "00a60ace962f1cdaf78f28b7fe95cc7e2829528597d083e3312cf67b32d5835649f763d9c678089e18872e7a54967ba7825c6ea5eda9c2ab9cbdb028c8da435b");
            result.Add("pt-PT", "cddac333605053ff9fb0e67c0da6616c8fcbb716fcbd6f2da094601bd4f3028305c0db488387359efc0869cfeffe02f9eba74e0aa4ab764b92d7cf95c293583e");
            result.Add("rm", "35cd0d5952b0fb857aea68bf3b346b84d1d76d0b0d7ad3898578ef03067a689cffb80444beffa6ed3de9253b61fc5acd54df735c613b52d087fd4f173ba6c5d1");
            result.Add("ro", "5995a72929593d8f25e896fc59ac3e4424a58e64be5c329dfb68745c2bed0e2ee349367e8695be259eb6ad166382cb492654c979e70cfeb7ea2139f9f399eb17");
            result.Add("ru", "d7b14629ea03573a55264a822d65aa4068b90fc9192f03c648ee627474d0580f11a72b0fd8fe74444e39c929e1cf82f8fcd7b87410d09de288fbcf451338671c");
            result.Add("si", "05c29dc178f61e5d8cd73f9e31e414ef3d7486d419b81d5fbbafb0915f871f408299e48786559bcc52967822b2d66fe1d2e0d3d56fafc697c9e30971ee696cfa");
            result.Add("sk", "3195dc735df2458d012962e90ce33f036d3e1761b89bbcc0e08b2d052ed9db1a0feea7f4db980687f409b868f54839a63035644fe48f2be2237e4d353f1294d8");
            result.Add("sl", "95810109291a7b2da26f0ed4545c14e4c816e4da629082a8db92e48837ce1bb3ea1fdbc126f3625bae5a9cdd6ec9912ab242abfabc1fa7b792ce3cbdded82704");
            result.Add("son", "11c6abbd7c3afed90e8643ef8d7c768d361488fbd1116fd17aa236ebee15cef325a57e0fa347c3c16ad2401b71527ac2aa091712b61c3afb0653030e2a4d755c");
            result.Add("sq", "20994fb7b77f7159f61d9cd8c8d60ed6b149e59c52d994bf05bc53f355a80b67936dad5274d038b51ae9b1b341144d2d0811b13783d1e8a87249dc0142fdacec");
            result.Add("sr", "a71406eae78a55fe1e2c688d4156f853c93c9af5d6c960c6c5a2151868c90dadf427a2a0b41caa24ecf6a18380e98b6b03366f1fee4216de3cf4a2fceffb75c9");
            result.Add("sv-SE", "6db740aa3c60bce7f59192cd6edbda3b93d3b7cb39d396e7919a2c4d82abf81a4d6b7104b58f10c90cd02bdfc68495ac3956a3aaa93207ba3957658a4ec3f978");
            result.Add("ta", "7148c1551db9a38b13575089d0ac306aacfc7ba26746b71d1f6af9384c613a958def171dbd4002f3b324c3dc0d8b4631ca1d4e43c275cc6b843cd424d2616237");
            result.Add("te", "aaa9694f79a3a4b7cdeeaf7b57aa6c10c071f10fd54113cb9842f50d5c611258764db9feb46042609274c3e7f1ec0e5cfc27a0ad64ef99a4bbb3dba0bb00c615");
            result.Add("th", "f3b1ef227f47761999763482eba8c80ac53adb3ac5dccf26f76bb343cb9dcd81781f7b7b25c7e63466a047f03d06f39cea1f66abbc44841445a6ee889920cfdc");
            result.Add("tr", "e937e595c95de9d01fc272dacc4b437eab801240c541307091209c603b9bd27f6ecd7485f9ea7ca264c11f4e151038b201f9d4d0cfc558b06c9160c06e650a21");
            result.Add("uk", "fb3957d43024209e0227be9cbe0e2ca16b17bc0f1bf0f02911461952883f7c70cf2abab3eee852c68ca3c755b914ab331c7f084896dd5fd79979722422c8a4bb");
            result.Add("uz", "811e750aa8c4a0ff74cb9f9335a2dbb5b2778b5436f11b19727f3dd8fda9a8176abd57c13488d55cc156d967b4419b113c4d5bd69916b1a841427e81abc0c820");
            result.Add("vi", "5d1bf21615bf59fbcab28f0f37954ebe6e55174c43a1cdc805aceec2885a558ef61957a4e71934b6beea66452c55830720e406733bbae62cf541b7215418e042");
            result.Add("xh", "2c7f29254ff23277ae8842af4ba6f5da551830a5356b5dcf5a2b2832341e79268853c66ae92626d6b7764241ba47d892dc6253cdd15d98e5ea62d920bca0d574");
            result.Add("zh-CN", "9aa310d18d72d967286c80b5cce1cf1cc24905db470edc5f42020aa5a38c3382609bff258451adcc2d6c8f2c82db9e978fdb87fe5cfaf31004e72cf6ab240e35");
            result.Add("zh-TW", "81199a9e6daa68d0cc9a57eb11df314b55e05299748f445470c1f0342b46909c7acb50d3845b91f45b43c138b4b5c79da93c4d0a9e29ca484a6c68c1ad024fba");

            return result;
        }


        /// <summary>
        /// gets an enumerable collection of valid language codes
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            var d = knownChecksums32Bit();
            return d.Keys;
        }


        /// <summary>
        /// gets the currently known information about the software
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            return new AvailableSoftware("Mozilla Firefox ESR (" + languageCode + ")",
                "52.0",
                "^Mozilla Firefox [0-9]{2}\\.[0-9](\\.[0-9])? ESR \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox [0-9]{2}\\.[0-9](\\.[0-9])? ESR \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                //32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/52.0esr/win32/" + languageCode + "/Firefox%20Setup%2052.0esr.exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    "-ms -ma",
                    "C:\\Program Files\\Mozilla Firefox",
                    "C:\\Program Files (x86)\\Mozilla Firefox"),
                //64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/52.0esr/win64/" + languageCode + "/Firefox%20Setup%2052.0esr.exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    "-ms -ma",
                    "C:\\Program Files\\Mozilla Firefox",
                    "C:\\Program Files (x86)\\Mozilla Firefox")
                    );
        }


        /// <summary>
        /// tries to find the newest version number of Firefox ESR
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        private string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=firefox-esr-latest&os=win&lang=" + languageCode;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Head;
            request.AllowAutoRedirect = false;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers[HttpResponseHeader.Location];
                request = null;
                response = null;
                Regex reVersion = new Regex("[0-9]{2}\\.[0-9](\\.[0-9])?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;

                return currentVersion;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox ESR version: " + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// tries to get the checksums of the newer version
        /// </summary>
        /// <returns>Returns a string array containing the checksums for 32 bit an 64 bit (in that order), if successfull.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/firefox/releases/45.7.0esr/SHA512SUMS
             * Common lines look like
             * "a59849ff...6761  win32/en-GB/Firefox Setup 45.7.0esr.exe"
             */

            string url = "https://ftp.mozilla.org/pub/firefox/releases/" + newerVersion + "esr/SHA512SUMS";
            string sha512SumsContent = null;
            using (var client = new WebClient())
            {
                try
                {
                    sha512SumsContent = client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    logger.Warn("Exception occurred while checking for newer version of Firefox ESR: " + ex.Message);
                    return null;
                }
                client.Dispose();
            } //using
            //look for line with the correct language code and version for 32 bit
            Regex reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "esr\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            //look for line with the correct language code and version for 64 bit
            Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "esr\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // checksum is the first 128 characters of the match
            return new string[] { matchChecksum32Bit.Value.Substring(0, 128), matchChecksum64Bit.Value.Substring(0, 128) };
        }


        /// <summary>
        /// whether or not the method searchForNewer() is implemented
        /// </summary>
        /// <returns>Returns true, if searchForNewer() is implemented for that
        /// class. Returns false, if not. Calling searchForNewer() may throw an
        /// exception in the later case.</returns>
        public override bool implementsSearchForNewer()
        {
            return true;
        }


        /// <summary>
        /// looks for newer versions of the software than the currently known version
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the information
        /// that was retrieved from the net.</returns>
        public override AvailableSoftware searchForNewer()
        {
            logger.Debug("Searching for newer version of Firefox ESR (" + languageCode + ")...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            //If versions match, we can return the current information.
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
            //replace all stuff
            string oldVersion = currentInfo.newestVersion;
            currentInfo.newestVersion = newerVersion;
            currentInfo.install32Bit.downloadUrl = currentInfo.install32Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install32Bit.checksum = newerChecksums[0];
            currentInfo.install64Bit.downloadUrl = currentInfo.install64Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install64Bit.checksum = newerChecksums[1];
            return currentInfo;
        }


        /// <summary>
        /// language code for the Firefox ESR version
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
    } //class
} //namespace
