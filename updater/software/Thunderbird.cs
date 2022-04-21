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
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using updater.data;

namespace updater.software
{
    /// <summary>
    /// Manages updates for Thunderbird.
    /// </summary>
    public class Thunderbird : AbstractSoftware
    {
        /// <summary>
        /// NLog.Logger for Thunderbird class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(Thunderbird).FullName);

        
        /// <summary>
        /// publisher of the signed binaries
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// certificate expiration date
        /// </summary>
        private static readonly DateTime certificateExpiration = new DateTime(2024, 6, 20, 0, 0, 0, DateTimeKind.Utc);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Thunderbird software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public Thunderbird(string langCode, bool autoGetNewer)
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
            if (!d32.ContainsKey(languageCode) || !d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 32 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/91.8.1/SHA512SUMS
            return new Dictionary<string, string>(65)
            {
                { "af", "c7ca822ac65a084c8c700115019998356380e749e1e39e2030676432c40f22288494f7173a3d09fee6036c17f7ad5578095823cd9a56fe05aabf17ec3ecb4499" },
                { "ar", "460cfc34920efaf18582cfdc263e8570db8df85df989e7ca96d05099a27d3a701aedc08c9513b97090b10ba942ccf414701f12852728fb5206ca656a2b3b2bed" },
                { "ast", "36dc61338478bf603d6ff83387d2030d34d08273361d8d288b1c06e1b76e6a631a8a8552f5a34a497f47ddcdb4d022b7cc3640ad8c65de6fa8740fd593239894" },
                { "be", "9fc182ce7877e72301b88ceda460f689937b546f059ec557eedc49af2634ea8ce5907580ff6ba33e5afcca82017ffb721f7cb6b1cc63e7f05402b11c17ae45cb" },
                { "bg", "1dbbd6772e7a6b9459c844b51a83b41547129662ca01eda64f7d9ce54ded091c8e2c2900bc1c33db124f87f8c69d0edc994dfa38fad9623aa0013db6d6c80f1c" },
                { "br", "727b3dff34f2acf8e231c275fc2f17c44f6cf378dd8d221927ff77291e126bba8ad6d38cc196cc376c10c1e4b28e70a7bfe7fe5a89ebc883ac31b83743865ab6" },
                { "ca", "b7e0aa5c3252750a90f357aa9f28caa6b6c6b622e29274ddd76c1cd9d4d9758cd7fa511e1d9dc406f76471125705bd85e7a45181c39e845f8c03b79ced8add04" },
                { "cak", "4f29008cea1c845dc3f40b108d44170e59ffa1d33031b939408d9032d245c57242319a847cdfefabe75051526aaf2cd5f5c34a00bb7636023f5b45ce2f6d9886" },
                { "cs", "3cb89c1690adfae16374db5b17dea27fc001fe12eafd3b15e4ed1b48ed573a4b2dfc950c6375d544132a68ca4d68f62a9206290df8a9cb16828e768590a394ed" },
                { "cy", "4b0e3875a014038c22cac922f0f7af19565221135aee0d95b2a50520763d6386c9b538d3886112dce9d36dc1354598642d2cd5f0a1ccb68930a6cfaf02d3c28c" },
                { "da", "72f19a54300ae87a8b11b7144fae02f2f57c7a06e8c10a9da02d9c82f020c10c33ea0dcfde59865f8dfd2e7951c71c0dc2288ed2ba3684993b811652b374ddd3" },
                { "de", "68f0b64c6ef1392aff0d03683ac0d7124efa3aef692b02f1f9cd48cc66114420da89c08216f28ca4ce22fb41fd0eb9c9c0433d99e65bc401e12fd41068e95c1a" },
                { "dsb", "f835acef141098ee622d4bff12594e7b1fb8ff6c6e78fac602f0d462c38e6246174ef41381e6e2cfc806240772169f03da12fa3500142b027e91871bd32d7be7" },
                { "el", "1b7480df428965ba9979bb7cce2a3bb1a7c52dddc620715451ada9acfa5da1da3fb11846e993073f0cc394297f2c842e27acf0acada2fde584f75cb811d45d6f" },
                { "en-CA", "9966290d406e91678b8ee8e9a4fd4eca6373acfdd2962050422ec1012fc2693214161fed22f8eaf9724b33a9f1a629709e693c7a9a807260eeae902205a631e3" },
                { "en-GB", "6a3cc3bb9eec16f089bdd149099630131056686b64d5efba731b71046b9aa5ce4a3e440274633be18b76946bb9c7d12f3a013606479624d945af360e0bcfd0d1" },
                { "en-US", "1081905ef62d59cf1e6bbdef8fce37b1716268c59941a99c10a2238091e1eaa4fc9227e13bfe3168cb1a2b015a1220fc445db1033e5c1a605aba70bc4b5244ef" },
                { "es-AR", "fa5228fd62cc22a31962dd40e28133727cfe7ab07bea74a52f78da3e199f7d967babdc4af162b45ae78ecf76bbb6ecb7808eceed4d40c67d0cefa6cb3efc6394" },
                { "es-ES", "5684792bcceb9f07cfbb0fa565d194baf5786913a035b619438560d30abafd4ce64ff58e2fcc1cfcf1f0da3836a74d820d27460ca94b3589b4cda9c56d83938a" },
                { "et", "0d32c2c6d98bebd8e442d2582284df254e496f463d5662fd97b9f6456da2c2c37000088353b4999e4833bd08045415a0c99eda98ca85953f35b763c6c8b5a688" },
                { "eu", "09080e5b5a0d2c520fa3ae68042276c84a1c4df3898b94995d6273bbc26743ce5bae759ff8d45825797291830f9108bc8d9e8ec7a34cb3400900e1030387f5d8" },
                { "fi", "ec5548d5137f51cc1e6919bc7f406b0ca561d32977944f020faefe57f1b40674aca4ba47f9b4ef6cf795619a790f6d5596c00047bb7154339ed1b2ffc07aac53" },
                { "fr", "9e7f70f700c821d5fdec83a5f6f89d9ac947375cdb696d155ae12c99b7368df614efd0cb30e076c25c5be2628cafa755d7c61926fe02d88f8c36b7f833c816ea" },
                { "fy-NL", "e0108b74db93d219b41e5060a6fe2bad82ee8e0ebbed8e79c32906cd4cba9b2e364d3abc2185db46a43b89f782145fd08043652187b759945db1aba1fda6f610" },
                { "ga-IE", "59d8140a09514032da040737c5bd9b2f747534e3f124f25cb7fcbc4d648d04e6cb85e28451a203489204a085eb4c3c4e845fbccce4c13309734509b78861ca44" },
                { "gd", "e3c7605a103d193041e19a09229a6a0b8f7624bafd18594352cd2eb874607ba3cdb069b4a4bf0f77322e934ca11a173972f549f23853443206b3d737d8f2b68c" },
                { "gl", "5fe9b9ae3e46401dfdf6582f57a479278ab8a7eb701959a6cc2f76761bb98a92704c531599dbc7990c59888549fcb287e209b67d36950ff1eadc724ed82d0528" },
                { "he", "13c5bc33e224dba6fa3c836736c4c7c21be372774c06aa4440f77643a4a84a1e81e882e5162e98eb6065d30e849fd7658ec9e120882da8d0dfa33e6bf691600d" },
                { "hr", "a4fe98f4c3f2e369d3a78f0b1e24b8082da40b127650de51608116ac1ed7cc0c4820466588a369871605c8bd9ae5d4dc6f2992ba846068581963522d8a1dd3e2" },
                { "hsb", "c98ee6ce236bd4ee047d1622617977fac06aee36bb538e84050b98c50697725508af3f30facae1eae30bd30ddf30470d1adf81f20742f0dd9ed6f6218b39a4bb" },
                { "hu", "619ce7cb15bd8dffb93e41f3a6aec69ebf1e6058e82a00d1eaad4d1c3fde839d919252e15790eeb604691dda51432448fc1ec5fced05d494aff6eb9674af0fe4" },
                { "hy-AM", "4954e09fa3deced7be8c3c36a83039c4211d13723b5959f4406a741149eee04cfac9ee22e13e23367b2dd2d4d8c6dd1c2950775da9c8ef95e8ab046b13c88a0e" },
                { "id", "5dc4bc80f4963d11a35025edabe91f21a8c736b02b75d8950b80fe987070f73f8c99be427d4cbbaf58620e22c01a70d47a430ba399c14c27115263dbdaed4c3b" },
                { "is", "3dd8c10dd027b7b659e161cb85472424a0ac2ecd8beb8f327dffdbb8c1bd9a7f8f61509dbb57ff06c17ed7b6a9a807515148e36cd63cf3c838b4472d22418ef9" },
                { "it", "d8bbf37f79ca8223b7c10743b0af106f1da3f214e94a033b4d66fd303ddf1c6e3d7905ffb6edac405bd614bd8cd4d58a4da5bae9109e2cc00e5446a082043ab3" },
                { "ja", "49ce4f83efbfa1373505734f567bc03168d15015827da55f290ac8bd484709877969fe4addf9df6c6a8854bf5c69f0fc7c9491251571296ceb682eac0c402b74" },
                { "ka", "723cf1bb2edaafa6284384336d65e53517296fa7198a95fb378217665243406407285119d762e9cec0a037d256a5d08bf384a14ae6d0d8e26980b67e9cd3aea3" },
                { "kab", "0f61f652f186c57ce2b03a9401a9ae6eb27dd80ed4cfc565cacd8b660a0a0ac0d4104854bbf891b33a11ca3816b36092085fb97d56a6128a783997c4441e43ab" },
                { "kk", "129b7106c995d61ab9e918e5a4b7f4ec371ae34d11c106b6eadfaea11ee8e9a38bfea1b95bd5dbd2bcd03cb716ebf0dd7868470a7723dd9b4ea3911fd3380607" },
                { "ko", "f47f7d8fdb260ec66db5ae5b5d9d7884f899f07b68c3b2c39b3a1c4c272bcb36e5c3ad331ed129cfacd12bbfd4883c1c0291f25338684d46ea200d7ad8daba6f" },
                { "lt", "b26dd3efdb430cefe8500871eb9e9b94e8101f568635a3ea0aa0c79c9212d0a0c90f0a665eb267d604f34a42e4f5fcba6606c7f608af9ca06841258414c36c49" },
                { "lv", "a4c3f492ab18ebe409d27f4052963134677f0e39ac5fad0003b3b4223f80e7a7ae31d8c0709828fc95f202682361355f2a0f54cb07eb0a2fdbf02af73cb5660c" },
                { "ms", "63a4aafe742d6da43c897e8608e260322663e46ea14669d572e45f8a594c8c26c0b1eaf8114fb3fbd4024478d6a461eab119c6f1bb69910b78e67c9b541ce4cc" },
                { "nb-NO", "f52e4226b9412e480d4d98e8a53b98f1e2a60acc4df240d0fd08a90e6c2597cac7b4e725d689fdbab337f9bcb46dc981ffeb179b9044a292316596ae2a3e7a2b" },
                { "nl", "0eed4782fb75db9a63ac93cc301387aff36160a91b8ddf588eebfeff98a46a7da60618bbd5cf6894bf59aab818997957605f908ee42b79b02f8ec9dc8c5b5f3b" },
                { "nn-NO", "24f1c2885f21b098a37ae249bcea846ed73730f6e70e18c000ed781745bfc03e37fdf50184b796c0797c99b00215e4a8dc9cc856783a844bf5b8ce2557bf72e7" },
                { "pa-IN", "f23ee5d9f6c64896f09ed964dc8a8137230098396bae16e06c5d63fc65db62b3246704b3493c3b3a6731844740ef807768e89ba8372f5d660e0d558286b3da7f" },
                { "pl", "525eac64b5fb6da9db5df680288a8296b1909b610026b4f54f969869717faeba96cfeb756e660d9cd7c3b2187b115620c67f251f016a1344b8e5b641915aba22" },
                { "pt-BR", "2fc94dd4650f85aae007e457da47969a714946145d4808214295186f3379c462d87474b052f7b9ab3cec787b13fc59d5b55a09f4f9003de97e39ec5d41c3634f" },
                { "pt-PT", "08d715f22317b04a289618dfdbbfeec7023f22b7c9189383b9d58538f5966bab366412177562cd892a5f9ce6b115f9ba18e54a2032161ba2e7d5a746134d8fb8" },
                { "rm", "4af903ce68b56021f527f5ac8126c0316cb57d86f1c656b42626bc801174eb1610b95fee176b3c9dbec8c9d6aefaee9f7070b83266a78087af1b62ce87e0ee16" },
                { "ro", "bb5a3ff2234ce88419f2b0774ec79e5a02723d1f63ab9920e0e1e742dd6a92048b24d15e5f6915aa9819888cc8cc9b4c49e4d1358d37bfbd4da8d5509348f75a" },
                { "ru", "83f3ea8f6ae4771d718998684d9b035b31ed1b0cc5e5c5a1bb49fddf73109b213ce0501a2209c148ca258f241cf8032f3161cb8ad7c1cba63f71ed951c7ee8b5" },
                { "sk", "a8d836a519d141888471345db68f4e6291239d9c151e3de45448258abb9212307acaa2bd96c39408aac20a55d1d7f36a04ed2e203fc38de4766b2252371362f9" },
                { "sl", "1f20403c0694f8a122fcf781303abc595f50b1334fe27482e752134dd6c207b8213b60e3bb20501b56cf50c5556b131a9802164781f45b112a7629d3d860d058" },
                { "sq", "777a0711d0cbcf79570214d2c47b39250a1be0b32b509d4884dbe7695a7f58e440496f43e1903142b338e95d5a50957283978b84db01087a6a61e471ef09acde" },
                { "sr", "c2453aa24cbd7de2f4ca2104a2a50a50da08877973f35826e35748a701503ee810cb4be88371595507e7e4d9b52ab54dc28bb493e3a5c87300b501297e1f7d90" },
                { "sv-SE", "d651faf68707562523b5e60e5ea57fc09732ad2965bab0bba5cb5adf15d23188bb692fba7981865210afcbf41549a4c438783a4beede6cbf6d0e3fdb3819e1e8" },
                { "th", "62e87cf72ac2364b18c8bcc641b8b3c4ee8d22915c696cd5816df3377c26aa523b9193464e118ba571411a084a2c6f1b92bcefefe15eddd73929184d9032927a" },
                { "tr", "de4493901762422720be6046c82c37aa0dcd7240763270991c6dd0041717dae17af344ef00a416c2592b841cda379589901f53070e0eece767a97fd5aed57b6a" },
                { "uk", "72ed7097ae6354917dfaa0849d54f894ef3943f23b9bf88e2c0192b1ec56017e5d77078085b6d198a4ba33bad663706da6b49980842d11d2e4b83fe68e02f7d6" },
                { "uz", "a4267e159d1573178472b1bb7027f708fb305c4c4266472423b47e0409f19a6b4581b38457d5d088e1adcf66618d010232917925a1c63c6c2d39af7020757af8" },
                { "vi", "2b5c7640b80edea3f52b0b881c6da523d2d0b820340e30d4dc1534fa2adfe9727531424ec2b5e9932e43f88d241c2aa2b3ebfaad8d4decc501f204ed548fd5ac" },
                { "zh-CN", "86031368c64bc32692fa5e40c1219375dff33df4ec569e03ecd0790eb76621b64ed3287e07ce75809434282888cefd919d32072d45664dc70d4df70af8416e51" },
                { "zh-TW", "dcb36fe09d2ee9a2c0cbdbf6b3fd70066ef2b4af1d6ddb96a286fb60f9da70510e3f2dad346fcd7139e84b03e5327eee90324c11616536ca63cbcf1b694a3efb" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/91.8.1/SHA512SUMS
            return new Dictionary<string, string>(65)
            {
                { "af", "3c74fcea670a7e17be299030b05935dde32e92677bbafdffcee1a5e82c02264ee24d19f46a4a738b39cdc6fabecee25e49cb3f6cb08f883f4134548ba6a2fd4f" },
                { "ar", "df35f9e119ef718ccc3f14c91df5093f0f493a2d115f124bc6872649ca793b752262ff35c481f4778b1a172b5ed69bf8fd3499aa32400f7e001c5ce317c64edd" },
                { "ast", "ed0564a12a547531f18d108da61d1573f9912545670dab937c2877d761a01599a6c25c2950ab6d7a91572c19b83f1e02a27acecc3d4094f62c4de0fcf289eac5" },
                { "be", "6721f2cbd0ba5c6d86e8f6648c78cf1256ea63df78e06cb5228f4b6cdddd818d7e85c8ef0a1c094880a79f869eb825a22599a2573e04389b1295339c8ec73425" },
                { "bg", "cd2430972e6a22b25f4fa36da56031baa27494359be2ad0618080e53479d340a043ed40a769b5c2764e4fcf3654d45c7f8ee108fefe52aac85834dced0aad05d" },
                { "br", "6b328ba4ee87bc036b85c1cede66719dbd069dac5d45cb6442ea47e5013764d66acf872030e8bbbbe10538c2840a99ba64350a3b5e40462ec6c867dff19ecc2d" },
                { "ca", "b14eae25efbf15f53f3ad924114823857acb8cff4d36307b9f3237e6910e6aa901194305f35cd10d1547a5107bc8c5f6cfdf8b3b9d676b23cd5c7e24128691e2" },
                { "cak", "eb346fcd3fe7b9cdfa2828eeac7fa68399d7ba1d6a5507098493f97a6434e3420d955e555cbd6ec1273d0e8667b5794956f037b3284f651ef27df98e8a600665" },
                { "cs", "0238c73bb4ec5e9e13ff6106866b29f270feeb1237ec7d5414f2bf7e6bcb39fb7dd7b9ab6fb3e4a8920894f2f26ac0592d1b76c7ae2cddbf2a071de6f7ba1339" },
                { "cy", "778cc8cc1177bc2b31ff4045c4b90459b757951b1946e53b50a14359220bf3a3fc16fd608556337a3ac92e0264caaa07941732407a7e3227a7e91e3b639ce0f3" },
                { "da", "80d0699c6002e82a8dd0d66062a887c8605aa2182b46333e21a132a669d244fc3c68d9169bfc3db95e425a15e3f964247c207b4a9bb481e5f452566a2da824ea" },
                { "de", "aac8d3f7b68afc7acbb54b6fc7d68c5df234b7bd636768a418531bbf2e9fc73d0ef214784cae3027a7ab8e71ac3ad44adc8e4999a2ca84b71bb7b206517f0e9c" },
                { "dsb", "3181df6505ce09afdbac48cfab9491bbcd5dd70a4de53f35a502e0ede12f47059e8a2d5537b2c8b72c04fe650f4727660826708fe883b6edb708bbed066c07f5" },
                { "el", "c25fdd4c15d018a8ab8c528f677e59bafc678712e92cf1421e5bbf51346071a2edeeccaac575324cf98ca775e7d61b5c783c648c270a08e76181819e16f67bd2" },
                { "en-CA", "350a77f2d408b9e50bcdd14471523e7d51f9713e7ee402e273a351ac15863e0f3b5d415a475454d8f1ee8a4c103be293d3928c94afa108ed7944ed0c83fdc33b" },
                { "en-GB", "46cb9a0ba55d0acba2686f5f21057dfbf073f278d2ac5c4a85319bbc3433816b94e02831676d3cfa43faa30680dda38d09f6097c7789339a1f7f0c3d04cdd195" },
                { "en-US", "a84c1fb25c4bd6613ee24ad3af5a71d24d135caa06017c9e38d360488ba9737e13207b6c07c6877c6f588bec6d9804f04e8097a498693c9992ccd3aedf65f95e" },
                { "es-AR", "e2b8612ee276c1508801238754205f9bf77c0a567bc45143ef9762d6ef54e53b31dabf2cb4f8a464e95d39e036a4ef3342c90dc935c97ec76727f2eec900110d" },
                { "es-ES", "8c51faf308299a83fe80731f9cf9560ddc4fad67f622103d874f358def2aa261dc03bafc3d7d45a11b119294d6b1ac5e1883927e782c8e82686fbbd8686781d0" },
                { "et", "a640c0f7c3c8bf0cc8dfb3ac20160c1ccf286bd200e0a238223312fc5739784dde3c408ef910b32c13bcd80350ac9e9f482c2c23a2280c42a4bd2bb80f340229" },
                { "eu", "ecd5df599d21c13a599e8d6770971a70bb8653ef0e1594985de2a49eb2d996ae2d55a7435a841f7a37f87ef774ee6f78b7f20d44a60c38a766f74728df2b0ad1" },
                { "fi", "bd77f238f6bed563058390e0bd2678ffc3c14613377580f65d002305a4c970d3c005b4cada4a3f619dddbc646f40799761dbb586a1f041aab2cec8effd3fe1fe" },
                { "fr", "99c62018e413fb0c44e820de4400827b7ae0e6c77056788bac8bf60ec2e7dd3e38202c5e436a7cb7508fa5c220d8d0e428c1fdf2dd85e18850267ec0a397861c" },
                { "fy-NL", "30e96e657a216afa657a73c9a954be2eabd5377249850349baad5368bc49577021a6c283e06b338f6708bbcbf9a522e2a994910a501a9ad54d0461e593f931aa" },
                { "ga-IE", "ce822d62ed77bff65115511d618a391b73826cdbb076665e57870acd2bfb1f0a0e5e0bd3f80d013b6c761e11cf9e210174c659fe47c459ac2b24050a431fedca" },
                { "gd", "ba9701acec48c5f922b76018d783e935a994698b649bc46944bb844ee802169c58fd249f0898b4f98759f32f98392cf635682ebdc755a5785cca397074354ed2" },
                { "gl", "f2c2dd3c91d717c746178d8566a67f8014cedb52f037215bc28776429f6c37452be060ca480ce59da04c1fa788414fc514e070c6af82e03d61ae5d2e8b5bf6ff" },
                { "he", "3a9815c104041869e225d23f6f992029e3123d768489d1ab909ce465885d63a67f44b22d7bef0bd978cc8ac2893edde8a325845986454343d14a656d36e32558" },
                { "hr", "b966360d9c923fc2c8007f5b4e7b8059546fcfe033c618ec20c9313c7ab176fe1a5e331a74e17df3c74f9248c4d5b397b97bf6bd91c8ff80d737b4947f90e60b" },
                { "hsb", "90a04fa56fc856279a3d120cc8bed9d60ade25dfb58edd36f859a536e1917a23051719df397c7b49a9b7cfc6c2bbbd7af32d8d8c1e47f33dbebeeca68adf84b6" },
                { "hu", "07d40923cc417b1ea24fa98c67637960f28dd1e0a6ffb028473eecb37394a6dc0188d36253fe38b184586849bf52d7cd9c9c41a6a977ca0d305a31042e86b055" },
                { "hy-AM", "3989c43357bf8c63a7bb0a4fa84d4319e8b780e244a77fccde98c2eab00a731cbfd4c200e9de27fc3800b2a127193a1711a67addfb58b86af6aa92a169b85383" },
                { "id", "6907712ccf93b2ba226e17de6192956245a16c97d076119e507bda4f336d6e5c4d7c20b09d649e24194bc6f2bc263404552d638186a4e8ad02bf73535e00dddf" },
                { "is", "b37a4a9a5cd3ddbd394339a44d49f2723d01d557d77eb5eb1d82ffe4e0587e6c509b49985cb3b657bc090a0c634e8c1ca92768e848563c1e200226431f693780" },
                { "it", "9af264296ffea3792f7e3b1a4115d78790383b6bb03173b70bbec26f5b19df1d750445c07ded363fbedd73166d64f604beea2a2a77fee4dc41a6cca5fbd87fca" },
                { "ja", "ea8bb070c1c7bde8cddeafd2b8f218c0b09b26553239a5e43e30236cfba74352f47f6d187025896578a2d360d3da4cb12844ea683e9dd3a5e857d0ca87e84e97" },
                { "ka", "7ab9aef9e6aa1e7fbd7a3055c90adf80c071a1e0932e7e2196a1600f8584eb4e719d54964fbfbbbb17a5d34c0453c65d4617a923cf5a15706e7606ad839aea93" },
                { "kab", "020da394ebee5d2bc34450f1e8dbb1d200ae807da935f161acf569cadd12fa03212140f89a11c41c3f24d94e17ee70ecc75c0a15a3d5f0a07cfd553960b95e96" },
                { "kk", "b66adb7ddbbfb3ce5c56463c57006d50dc1d68131d807234fbb928cbcb7582ce2fc0b3cd393e923dbf765802525b926fc88d588ac420a749f871ed9fd1a48c3a" },
                { "ko", "bbc67cb17f65fbd5b830b7d78a82768454feee60893cdc0b371eced3541a20b7c7df125a32bd767605e9973a854445d2d76ad476e410a892fcbe7914cdb12a7c" },
                { "lt", "1b364c14bb86b6e65754380a8164f510044465fc0637697ae3671125a828d4af0e6a2b2f3610d87d806a1a85e090d27e1fadf67ca039b027ebe3d04fd4118bf5" },
                { "lv", "664b08e328d049acf601b8e1e5e966e859fd10c7d9e4a58b74176a129b8506a820809264ef90d4d952d3350d35c006823f5cee0f7ff16acc85d915c8d140ffdf" },
                { "ms", "9e2ccf42643fbd30b0ce7390e8a1f4727f381faed0525752d9566d6c0f595242a2939730a1a8f8a983051226dfa1a1843a5eb92c6b0a6ce3b097337b4e22f87d" },
                { "nb-NO", "326b4e6c11a1551eadd39d7bebaac92388eac858a71cdac0472345eeabbf453b2dd622f68f5def2a508512c2a05949d662185d46447da89817b1edef0f112d55" },
                { "nl", "854a25a50a79d3dcee5729791a0c7d14e411cc222e204a62298097ed7e4f346d416b7de94d907c7e99c0e29bb41a2b2c5f783b68f504e94d5b2f58fe9b423c14" },
                { "nn-NO", "9d8fa0d6a104473837a070c39947608ae380e7a502a3fec5789f6df661a81bad70044a3547aa9775ece656f3283b784308cdb4b09ec1bde9403021035d79f5f2" },
                { "pa-IN", "a13b72d8dc728cc9ec974e7534f3b30f33a062999dfde839ac36c3fd4cacbb56590149f1e8073cf8805031508f32537f2e88fab5920064cc5e4c7981654e7c4f" },
                { "pl", "fab2eb1d102bb687ba3505f04dffcc46d2e2c442896a59655c8e5c4388b6787fda16ce023f3f1b8a41ae9b07aeb278868e3bb8b781b6a8eb10c967d7eac58ccf" },
                { "pt-BR", "ad9e28e0de0954a5d2d55568e1e45ffc0a12e76e7af2cd8e3149a56bb43daa569f3da8d209d98f9b49f7d69c53a1fdf5557a3440a3372ccac390fa6efca40800" },
                { "pt-PT", "875559f46ce86b838b1363315319d7f600e06988674218e0fa67654de4942fb2792864305735f8de0074695e82186d4df0a35c1642c8543190fc91bb5b9a5a6a" },
                { "rm", "248c46b83f164595dd3bb2eb5129c60ab0e85b45fdec7f3c0c9c1677a4a72b6c4295c00acd2fc28bd267b1e7e8c2a518096582810da3e1a03b42c2c963b521a9" },
                { "ro", "b14a448713a456f7aa67b837ce9a474005c617f12381a6c188d036a7732e1d9b41edc22058545c84666c2dd7f921cef29a8e796e9bf1cff8638461e11033ef5e" },
                { "ru", "66eada22ccac743cade33b8eb90e92b48fd0c5ef60e6445d23f69c0b018fccc7f21b0bbd6207cf4ee3c19d5bfb81f7fa19ec00bbbd74c1ccd4c5e37ecc574f49" },
                { "sk", "f03a79746f3334da96354a5be9810726d97d9ce19821e8b5d1cfe746458ffbb7cabcaeb18583e3b325bed2c8a8b08fac1e1c06f52ee89ce2f96a6cc8b7d38668" },
                { "sl", "881cd399fd78d6d526571c8e00bbdbd1549c164b09cb0c405b9086a0db22b2c197c9cf713dda936b12355636e5960777d11b2b2629966714a59af13042d95b23" },
                { "sq", "d9f0e0af6ff08aebbfa74346e013f16a45be65636a2f20141c8af0ed24a74e43b5b40fd16e9fe417b7e476cc98cc4aec052038a565b38212d1cc516f01e89046" },
                { "sr", "d1aa7c341f21fa730a8519f6322feeb972c07445c7ca28551db07f3b52f2ea4dd89728823ce781dec76098c5be9dae6ef7d4b4dff8d9ad170ec482c3263f504f" },
                { "sv-SE", "2021bb194a0f5fe2746fd623b2a4ec37fb989aaf2565bea84972131553867cc665fd7fcba7ad92946ca76bd453f47ea74e3ad8a163b8a2a9e0da894ee9d82456" },
                { "th", "6fd2b9a2d504b8514c2d75789f5c81d5c04ffed22101507397e8ec9deeebe84c689f447b62e7c66ba7578c5b5f3ceb5973ac82c75e4cac4e37758402c3aa5434" },
                { "tr", "04052f9e5bada85665e8711f8ca253b63bc36af72334d5275484b50e7d434c3b1864777408ccce12e594eb6ed6d69971eb9a7c5e022db2dfa742d0d5b6130b3b" },
                { "uk", "23809e354268757aee3ea277f8f4f9316382eeaad3e44556f62d7fe91e8e3c0d51f42f98d58433865af5295484d711d3b741775f54a9f5a944126858052afd91" },
                { "uz", "91126d5f8fbfcc0cc5c242eedbf30d4e7cc293e224e564be4370e7b3ef80bc3839936f53563cfeec1dfbdd5acff2dcb80d703fb31a4367d3e60992d74265e7f0" },
                { "vi", "325c472e485342c24e189612185c78ba75fa03904a1100d9b175035cf7ed75004eb947677a9f3403b03cb0aa2d0f0bf07ba3da3f0746311986871ba6f5d2b384" },
                { "zh-CN", "7580841e137a7e04c1edd33e5879641418d1bcb877c04820ef4e7b351a29ccd8edb8d9b3ef76996e4bf346468492531cc28d6d52abef9c479cfe1d6c9480ffb2" },
                { "zh-TW", "34e2e4907ff38933a5439e776cbd11b22750542b85f9e1ff2e9bde1baff030f49d47a157e7f00de2bcae7e7b27ac8c61a927162ff20fde7cb337fc285bf33694" }
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
            var signature = new Signature(publisherX509, certificateExpiration);
            const string version = "91.8.1";
            return new AvailableSoftware("Mozilla Thunderbird (" + languageCode + ")",
                version,
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + version + "/win32/" + languageCode + "/Thunderbird%20Setup%20" + version + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + version + "/win64/" + languageCode + "/Thunderbird%20Setup%20" + version + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    signature,
                    "-ms -ma"));
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "thunderbird-" + languageCode.ToLower(), "thunderbird" };
        }


        /// <summary>
        /// Tries to find the newest version number of Thunderbird.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=thunderbird-latest&os=win&lang=" + languageCode;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Head;
            request.AllowAutoRedirect = false;
            request.Timeout = 30000; // 30_000 ms / 30 seconds
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers[HttpResponseHeader.Location];
                request = null;
                response = null;
                Regex reVersion = new Regex("[0-9]+\\.[0-9]+(\\.[0-9]+)?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;
                
                return currentVersion;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Thunderbird version: " + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// Tries to get the checksum of the newer version.
        /// </summary>
        /// <returns>Returns a string containing the checksum, if successful.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/thunderbird/releases/78.7.1/SHA512SUMS
             * Common lines look like
             * "69d11924...7eff  win32/en-GB/Thunderbird Setup 45.7.1.exe"
             * for the 32 bit installer, and like
             * "1428e70c...fb3c  win64/en-GB/Thunderbird Setup 78.7.1.exe"
             * for the 64 bit installer.
             */

            string url = "https://ftp.mozilla.org/pub/thunderbird/releases/" + newerVersion + "/SHA512SUMS";
            string sha512SumsContent = null;
            using (var client = new WebClient())
            {
                try
                {
                    sha512SumsContent = client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    logger.Warn("Exception occurred while checking for newer version of Thunderbird: " + ex.Message);
                    return null;
                }
                client.Dispose();
            } // using
            // look for line with the correct language code and version
            Regex reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // Checksums are the first 128 characters of each match.
            return new string[2] {
                matchChecksum32Bit.Value.Substring(0, 128),
                matchChecksum64Bit.Value.Substring(0, 128)
            };
        }


        /// <summary>
        /// Indicates whether or not the method searchForNewer() is implemented.
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
            logger.Info("Searching for newer version of Thunderbird (" + languageCode + ")...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            var currentInfo = knownInfo();
            var newTriple = new versions.Triple(newerVersion);
            var currentTriple = new versions.Triple(currentInfo.newestVersion);
            if (newerVersion == currentInfo.newestVersion || newTriple < currentTriple)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if (null == newerChecksums || newerChecksums.Length != 2
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
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
            return new List<string>(1)
            {
                "thunderbird"
            };
        }


        /// <summary>
        /// Determines whether or not a separate process must be run before the update.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns true, if a separate process returned by
        /// preUpdateProcess() needs to run in preparation of the update.
        /// Returns false, if not. Calling preUpdateProcess() may throw an
        /// exception in the later case.</returns>
        public override bool needsPreUpdateProcess(DetectedSoftware detected)
        {
            return true;
        }


        /// <summary>
        /// Returns a process that must be run before the update.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a Process ready to start that should be run before
        /// the update. May return null or may throw, if needsPreUpdateProcess()
        /// returned false.</returns>
        public override List<Process> preUpdateProcess(DetectedSoftware detected)
        {
            if (string.IsNullOrWhiteSpace(detected.installPath))
                return null;
            var processes = new List<Process>();
            // Uninstall previous version to avoid having two Thunderbird entries in control panel.
            var proc = new Process();
            proc.StartInfo.FileName = Path.Combine(detected.installPath, "uninstall", "helper.exe");
            proc.StartInfo.Arguments = "/SILENT";
            processes.Add(proc);
            return processes;
        }


        /// <summary>
        /// language code for the Thunderbird version
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
