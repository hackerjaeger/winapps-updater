﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2020, 2021, 2022, 2023, 2024  Dirk Stolle

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
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=San Francisco, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2027, 6, 18, 23, 59, 59, DateTimeKind.Utc);


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
            // These are the checksums for Windows 32-bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/127.0.2/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "1a5649a60404ebd812a91ee30135ecaffe790629788c9ebfcebb764417e2a4ad51da282d3db506b27910a639153f875aa37e02532be9db0534e5137aaa3772bd" },
                { "af", "b7f9024d738bf2f4bb42ba44791f48b67de417a116385dee4e8925df37dc0d336776c5b912bfc15d54b9099fd885710bdecf31e33fd98dd65b36f1bc7c2f73d9" },
                { "an", "41a049fce71658b8802fcef1c95a80837eaf163ec76f5ab0f872b3102d49aeaf86e72cdeec499f2701aabb125709514a508f281a712cd400d724200b72637e1b" },
                { "ar", "8036cc12baed176a20f7b16f1b73f3b3b61c62a3f1099beb07db7d5213bad4e4f2e442782e83d6c04b5b7c0dbd5ce1815ea98401f4e9fea7a48c46b5543ff926" },
                { "ast", "9793ae93ac3ab9a326ecde55255b658083a2376f3df0d96c6c2d3b00a41057742645469d88b41778e1be574fd20896c72ce7e278b47dc9dd93fab3ee0847b7f3" },
                { "az", "30ebb876c74e481dfbae4d1ff4e0a668ec4eb9a493ae344c281ce7f54906df72bc111839056b36e8cb5379ba52aaf1acb190acf17bc6e5a71871b9b58e6a740e" },
                { "be", "b5f2535a76b7208ab5843159bd558e94beac925f11c07563a4166985009fe318d666dd0c4de740c9aa1ca25f6f3b135c1e32df28c975dcb35bc5dd67ba78d551" },
                { "bg", "31fe8c924d165e7135fae54e2847f68b08e95f60bf56ff0a7da61f64ee29998f0d345e37ede07da36eeafcede7f31c4fb3768be97c52b993f4cdc7b21f37d704" },
                { "bn", "9c66d40d0ddd4bee8d13d2d7f712f5da5d20c1658e460991360a4ef79e3d764118bc32e8e3db5a04c3ac392cfb5edad163c1b43b28f2a75873b331718fa8c212" },
                { "br", "7171a90676032f87729d2d2d8cb9be4e88e4cb3ffa79d258a8c3a261c2f39dcdf4af123291bc788eb2616953a81999991d0d2281af0d10b6db06b31117c93501" },
                { "bs", "a73dad31cd0d736d850cd28be9da883f00f832485f138cdab88e0897617665fab5aeb2275a2a898642fca77ef70174010e88519946ea547ed4f854ead064c5a4" },
                { "ca", "5fd90ff152018a64b751fcc2c2343c61dee970fd8a8535c9489e29e60c04701339d5128f71a0eda522c518c05dfb5098a2ed306791e1f148103fb1b164b8d31c" },
                { "cak", "da5147a70c25a82fdd9dfc5d3ca910d16b4bacec5ef02e34bedbdf91f90b635024d8d74e8331ba0a4eb80a741b36ce52c1fdd99178c0133f53e3164d098b4f8d" },
                { "cs", "947dcf0148ac9c09bb5abaccefbb643672d2e01c4f91ceb6b90a6b976d99c9a132a899c78c1d392706f762b9d9d49d60151d17d2c8dfc69530b03269123a48d7" },
                { "cy", "809750ad47b31ac2158c56dd5c6be6aa4594b3ffa0ff1d3851a8b74e2a1c1a54d72a1a4346a432d3775ea784b779e141744e2904e0c3ba768baeca63fc15116f" },
                { "da", "67ed01142865d0b1e68e3d16b218b8a885d058df96571310a08df0b6c213f658c97a1f6e771a90ce176a7f7c6d291cacb4d857c984fc0780534db0866cd1a22a" },
                { "de", "531cb2f87d053bdd62096a8cb6b41584645bbbeb25d5221096e368013189a216d4f769b65a77c742f227b338a5dee6e6377edc7ad8699a4b5927cbff463574dd" },
                { "dsb", "aadb094167ef86daf7e10e7a90e8c7f12d69ae0f82a970064028c71611395ff2ba88c72c5ca3d64ba15fd78c77ebdf89c2eff06f10bf87820c396fc81d3178ec" },
                { "el", "80ffd4e1f45dc8eaceea201b0f5951c5125a4cc6948ef988f92b809b9da2e864fe1043399f8c133c780d8d2c8d08f6c8c8381f188dedcee1c2ab1995a0973efc" },
                { "en-CA", "efba28c4426ef2d7e4bb9f1a06deebbf3a40ca6cfc71130bd6ba18565b070cbabceef47b39272e7f2231b41c7af740cc4d79d403980c5087ee4c5ef687275955" },
                { "en-GB", "09d3a70acf39288586529637edbbd52772d0d8b73da85d9f0546619e5ff5ca9f8313b7422385dd67a0fed695e07f827478fb014de41ad0d0f6289a7a79df60ab" },
                { "en-US", "afcf43f28351791206046d6c0ae2c5ff86f70cf7bb5cb72317f10d1f4dd48a75525660088f6601b85a64b99651c589a818a6abe6646e4b116298d4d82e01e292" },
                { "eo", "ea58732bf799c990966985df62727bb089d6cfc66e7a31703dc0f23afe212c6cdbcc9b19e6f10b1f0b6432d9dca8348e76e3f500aa81ec7ca5130b8b37f17cd4" },
                { "es-AR", "266d468d8e4be568abd5b51e1ddeb4e7f5b8c81da1a7d2dea4f35e8112db6b67a2550e3b38eddc3648a2306e485ae687e0896b363bc020dc3e3fe2d687f51cac" },
                { "es-CL", "58c4e870bad4ac89e7f6f9e965ea42b07c5a6e10dc8b8cca858c67677ef2083d6207a26cafebe088d499d242c4ff4b9fd5d99d98d373e710181774dca354cad0" },
                { "es-ES", "8cb25d5c94730a19c84a84ce9adbce62bd131dd4e0f97c3037972e1127bcb1a5a32d299ec4f9b710098d35a39cc768c77002ad094c170ea5c9e3b4b9b9040cec" },
                { "es-MX", "629eed174ee1de5d85cec2fb2ccaa6c200548dece381f29c8e90dc958fda46f6859ddcf566dfda48d2f45b40ec4197cc41a4fed18ddf48318dce0a6aeda7b986" },
                { "et", "9f063e600aced14fbd9e053de752d22233e30e0e00828359561e9983f4df1e5fc6216d7f80b3cd270e14a582e9bf526ad5fa3125a4d33dfaa67dc3388990c41b" },
                { "eu", "aa4288707a18bcd9f67f2dda2af433b61365245c3c4e8d9d063250d8a2840e1b4d01ef2c7bd33d91db08d15278b6673fbe295c17f081b1d4088556dde90028f1" },
                { "fa", "2e2da06114d810767da997c65f8bb84e924fbdde2e4eefd0c1b37e926db7b1c4ce9f31946ad39572aa0e021481e7f99b7e8646fdd6c7d91f8d72216ee905db35" },
                { "ff", "142451a10ffcbf178c6ed754eb622604ed08b547a0c6e91bf03ea847527dad1d8fb595a189a1274a9f920723125d55d024d7837d2768b7bff430a1b8fa3494e1" },
                { "fi", "f6e25278c95ca77687f2755273c2cc8e7d551fb5ab2551cdc9dccd8df89da2468f1122ea73bbeb971792d80d6235cd95caadee1c1700ed02211e70441f020c5b" },
                { "fr", "14cc13e078aadf0803930b3c33fbd251f926792991f9b4ce225e25c5273aef7acc848f8a0cde3bb61b8cb859f61d5872adeb8babedab967d56a49cdf71d950ad" },
                { "fur", "00a50ff16d8925b9044bfcb82086c6a0775af2a8edb667b2dd444ca6580f4bfb8aa71448f7e9a05010d6d874c9c3bbd22e9f589b176e0de685986f310c2050ce" },
                { "fy-NL", "cbd9c44e914ba8f85b904f5646c4bbe59cc3d74ac193dc9929970936eca2a962d42f2b7ef42984013a0229978b797617bc83acb3ffad613c4a41b22de7bd7aef" },
                { "ga-IE", "b51cd9302dcb411a97173da2b7381ea587f2b8e1e44ca0c2d70acc549b88976145b9a62d5f663bcde4649c331851827124118ea689efcddb686e24e3b6e8055c" },
                { "gd", "8be60257f7aec25852a5667db69221df622a614c2835a9abf512147734c145a05bc0796b0ae2cfb3a8f356e83768ea6634b20c19051406526a28dbd2d8d69d19" },
                { "gl", "5dd19fe4460cb5811ce168ad3e10403dc577d67edca30a53521d4a8281b35edc25b702a06464bb62e65e8b51d93e9fecad8e6e35331e4e0cf6f60be0fc3e0132" },
                { "gn", "699cab7d47bf7eb2a28584620e974d3b129a1ab52b6c92e4e3274a9bfe364d92a60eb365cb612e6070051b37266f0907fbfcfff6b9c36c7d2bf479aa72dbb942" },
                { "gu-IN", "0d04fa04737ba4797dfc855ee7da25357fab15a32f85c1767e06c9c32a056a5ef89ff0edc4fb6fcde302538a50f31077796e5da52b1d1c89ea337c4d548e1e6d" },
                { "he", "2866fec9cf3cf152090f4824d3ef8899e3db535c41be418e9f0e9502b531544260929d2d3c53195670f8920da5e8c87c4e574b639ced00e392609a3190d2d9f9" },
                { "hi-IN", "210008ec381ac839654a53ed48fd071f466cfca6a1e80e9678d39a5056becdd3401f769aba2003d1240470d8f698058356e5404d83a728cc9759ec37ff203af0" },
                { "hr", "af904db4ffbec9f4ed0ff87779dc85dbebd578a205e42de595088e563bd3d42cf033ea5a5f33cfc7fcf57a965364e6b5fa34db3553d18db80daf94b00ac79b91" },
                { "hsb", "56e3bbf0723574bda1131baf72ff927b1e629423d3e2d0b102c0a350c6d9be8c2e5d7a6f35b51fcd1d38fadb473d5965eb30da67aaf2cb9bea0c37e018c8a374" },
                { "hu", "305a90eb6cab9a03b0d22a3ada34bac79a8a775933755dba0edebf08ddd48bf69d610501b222b507db7fb458ee16ee0d2c916c0cf5dbee37b8b3be13c1351089" },
                { "hy-AM", "9bce1ecfb0dec31874b846f921c87b409a3f6e16f4f9478d953c973e3f5d547fa571dc5223c316b66ae15bce1317a420704998da0afbf31f47c2f4d03a01e911" },
                { "ia", "1cd4eb57d32e90e4160344a263df8b38612410c4979358168bebce51a57a652a081666768c058272998844b750e1d20ff1ba701bc9103d649362d38a8178b958" },
                { "id", "a1ba8f9ca2ccf69f6c2564b9e67438ee0b8a468d36c3445ddeff3bb98d5fa0f296c65737cae08940a3c1be048276e863aa5727c72ad42852ce19c298a7a5a77a" },
                { "is", "58504ce4038e09b4df51e441050f02066ab99b408bcb5e62cee92d00ece15501015b4bfe27ec8dbe339fc2163bd654c7513a9dde59309da360593d4b63edd11a" },
                { "it", "07304e9c069be084bd4d5ed0068c5263b6491d598b0f2724f65ff08bb74431926083e4b8421e9e49b49874ba1c7fa777ad349a73a1970895aad5b2a72b8d95bf" },
                { "ja", "da316efe37365c7f459c9d4cf02c48acacca8c9d50d6da53b497f306ed840464ab791bd2951b0889985db1798945b7697b08435bd9279639217fd5538fd48f82" },
                { "ka", "9daca8b037c6c521e4c30d95c77871ea3ed681857eb01690917e1cecd35c3360b7383c8f8abc303937d0f62603b765b30715f60dd43ed00abf99b590a09b2d56" },
                { "kab", "7bbe0db890a343f327eaae82b232e0ca0f922602e3c5586490e24bea11b86b6b64a9d25c8996eb090d5c3d896133ba4a94d78b8ce2fe156a8c9312be3b6db63e" },
                { "kk", "9a38a7d617affd74ffc617ccd0a1baab083235746e18694aaca75676e81f70cd9a1e53cb6eb5b691408635ce70b9a6f6e514bc6554dd7a631ac67384864818d0" },
                { "km", "37aa6385f706778d9d6fcd9e4662eeb73e0d2f4838d10c30400f9cf606c3a7066c21bd7c57779126154f8b3e1453584b1d4c2b65b0848691784600b8974a5afa" },
                { "kn", "ae4f1bf14eddc563810e221096040491d7bafa7201e9c3d49e5f6391edcafbb2faf70bfbe85b9755aafbeeab230df08151bbd572ed6e3c2838a678e584ab4dbb" },
                { "ko", "3a192446c2245f51836828dee4e0aab87057b990d29c5f8aa260b139f82576ddebf1a71abe67999b7152161ff2735a00102ab1f8ca67e350c34904fa4e42fe4e" },
                { "lij", "f5a4afbb4ac7a09c9f9ba1c878864c3c92501c4c970c47523cb0a83b79a8e890366212f9d41c485d799c1943105baa4afe5c44163f3450a3f60a3379d599d068" },
                { "lt", "121222727379b97620682ea528402fcc1f9e86a42bb9a04a613155e9c5d1e3838beb92c9cf3b4c7b35eae240773f360661a1f3feef35370f8c1ce26026c77866" },
                { "lv", "5cbfd06b1bd52491c8dc9553f69bde17d0e139b16ebc2bf8116d771572996fabf8507e742bc5c7bbd065eef2fe5455f6197823d97fa13e6652848952e5a3daa0" },
                { "mk", "0949e0fa015fbc89161e1f80bad64ed0aead613bfa7345e7e9a541f5dc78ee17639e8bd6b4faf49e534c0b76ba8e7ad433729d857a806c5ea81650b685be4464" },
                { "mr", "67bdf54067996bb70463855c65a5e98cbb2fd54ba45fc7dfd5900ecd6c539cf9518b59c429bfacc1799744cff27fa8664973a8f93285dced4fb3b6b1ecd85dac" },
                { "ms", "82307a2d44c56be3418c4023eca3958a9c6bbaa96e25f94316db4c02a3a8f08383913a2d3b72f357f21821e85147a4358bfdb32a45e23cc6eba8e73d9ceed812" },
                { "my", "e8d3ef29749f5b942d6dcc538cd445e8a93d68e38e903d3ae4e30970126a76eb7f9b6645516cf2aa30a6b8e6f49458ff9ca303bc573553716b2b4bb607d8c52c" },
                { "nb-NO", "0205062f89eecc710a799bf9a3cc1f9fd8204382aa27eba00a1431523f22389615182b4b2e625dd184c2115bdd119d993b2ccb487ec48ea0ebc249e40b66ff4d" },
                { "ne-NP", "578eeaba3120ae8a4aa75be7f16454d0e2bc35746a9c72988e7f00eb86265e0e7643196ba2d30652cf9c1bf498421124b6a40d81aa09885ff7e860077046d189" },
                { "nl", "84f363126599a7abf1d08299ab98135ea64bae55fbeee9aef4f550367f4c0aeb931c441459ad2b98dab0881fb658833a1817509da215e49e6e325ab8cbd7ee17" },
                { "nn-NO", "01561bf88e69fb894fa8fca6b1734c4930f05e81c5e0e9ba2fdb691366d1086f2608051fb4e41440a3ef45789c9504d80323a2e451044b720eab291929726c22" },
                { "oc", "4713c2841066d268aab910caa1a05faf433fc1e74b94e86a21951a3dc215f11804e8b451dc07ec8865ae9e5a4058a853bf613ee5cb1d17533e8e9c52bd7f53c7" },
                { "pa-IN", "55984ba58b875701c30a678791a6a6a61c894ff48d6da55feac44ebf96e546e63f453f2aa7bd6cd01c27094aa1b6505f9928ee20adebca5b66855d2a4db7fbd3" },
                { "pl", "a45ad54637517e5c45f5c19f511355e5b4decac56f19faab8b53cdfeaab4f1e83fccb00ad302bdfa5e07f5d07adcc9520dc71ecd61181756c144b9cad0838109" },
                { "pt-BR", "cdd457dd90cc53c278eedf93c9b511bc95b645873bbeb3d90f9e911b75f5214b3a56914b23eb0294ca0e7281c98426eec4c6069e326c1a45fd59d532b78c309f" },
                { "pt-PT", "8b4e31720ef7e1df35822e4cafaaab4541944b5a87bc3b78866e8a20183f8259038aa79de0c194a802deb2725f9e288de122c6f55104dc10d3834ba5a9526aa5" },
                { "rm", "c27f70632f7224d591e3de66a64bd71897254849c56064a90c2fdf00a47a03f70dace799e8a7438d68ab4908879a4706d3d6126be2480a77f0bf057997b1f2f4" },
                { "ro", "1188176870f43c8caa86c906f427ab35ff20870da2380035b241769b6326936cbe63f83106fe965e8f82a48c41c5d11aa848c71f224731e1a7ceeead05099f68" },
                { "ru", "be483da1c3bd29a5517c18ed29b64b57f77fe22ef07a9ba73aa01202848f72e9b1749613eb8484715c84efe466a4db7228db8975d963e764dd8aa2005369e2dc" },
                { "sat", "7b23657783af720caf8062f279bdbdb17a0e3bbb2a269d8e2a1d7a0e155e0de24cea18bcbd4dc2b3bd8d1b0a6ae03adcbad488ac0969e23b4640dddb5dce4b66" },
                { "sc", "8833f5e25cc3400cb100a4df0a019cd475bc6c07db3696ad0d537295fab1e7cdffba967118b0f699a5344c832c4777a996edbfb13f0e3b1e507b0d799c4c5cae" },
                { "sco", "8ea6fc815801528435543964013caa3de787bef53aaf3bce9740fdc958806e4dd323f6729a751c991213b335d8982dce4720bfe875750acd152a7c7f8334ced1" },
                { "si", "fb87ad6ef4a884d6a5fd4d4c3b210d8735b055b363506a15176aab4292c6e3626be3279024c642c1ccc240b554255e9a3fb7afcc64626469236ed0927c91fb3e" },
                { "sk", "c6f68ff24f45cd027333727c28b4bc9d0cfd62f3eeaf89fe406e1192e1a2c9e4a62749bdef44b78b3bf9b9e3b4df5a000106c771adb1bf7f51ada62736844b6c" },
                { "sl", "3cee6074b4859a4a4bd77423c7f3e00f31221ac1369618bd60bf8ca04d60ee5f9de374b2d6c00e7e9d68aa9c0ec6c81daa8bf9bc89c19215f4bef5b3d6d4e892" },
                { "son", "3830c281dbfe18d8f613ca953c6a62a7143c4af19cd733c5e9dc9a438044c1aeb0b0f7c5715ee4099e1701407b157a6ba94c5fbbc4eadeefeffa273195e2a43f" },
                { "sq", "83bd5da7ee25b7d60327410476a4bdc4358935fbdedefbb5a74517a262d75186dc0894aa3696dec88bbfbf6366fac64efc480174dbd83a37718802961a043fde" },
                { "sr", "924a34fa618451e9198ef667cae7595eb93033c2352153b3632f79343566e8816fb4ddcb762dfcca8bb16a6309a72c4559148d6772e5c5da2ca7affe5f048bb6" },
                { "sv-SE", "12befd23a610836f786ebecd2b472109bbc41fd11d623c3e66ac65f60cdaf2ae18f2758cc30bc7b2f6f399290c522cdfa900fd7d787ba77cde15c1b614ca63f8" },
                { "szl", "9580bbf58a1e16bd137cc7f2e1fa7e551da0ac820b8b365bf48af8aa8fc7c6bc4f4fe14bd02793f8e38388ea737df6b204c34148ceba70507addc86a48940dd3" },
                { "ta", "550c963b5aa3c0127ef6697ef44c1fbcac51cbe810772ca962636ca677a4874bf961fdf9f3d4b155be684a2b78b031c3996373dc010eaf67ea4b6b7e21818cc2" },
                { "te", "4c4581537b172bce3c6eb8e968f6cf6f4f6d736a00167adb42775b5bd17009f92df5ae7b3873633ee8e1bb69d076fdfb780e17fcc1b071616e875a8eb74ec6a9" },
                { "tg", "7b3c5c6233ec523fc6c691a0facd4892550399f7767752478a2bba49a14ae765b97b14996844bd94d943c490ca70b38ffbd80a38bc651ab841e316f101bc75e5" },
                { "th", "3bf9b8a2c69241a9c02c3be72de100a65299cba3cbb0899270ebfdba49d59fea2c8184a2da062bf0a1be5d125a248e9245d8d46437eb09060db7e35e035893f1" },
                { "tl", "4ee53011b92551f6761960e05a93123319cf64b4ba019dd2fc92a1bfbd8db4e2ccbfd03620bcec84992460aee397dfce23fada7e52a62cf5fa06df8053491310" },
                { "tr", "28956fc995f0ee0da879a7a88aaa917f26d5850aa485b3a5e9ee7fb6d881c6309a668b68970d237c1aabde7c0c3f8f0c1cec20b07968fa4539d6e48bad2eb6fb" },
                { "trs", "76b9fa5b49855a7297bf61fb5cf027f72c060cb506bc53bc45684bc9a8090f95559556144e8cedda90e0fb07377b457f7296abe0b25bd2640b5b5b27a71b92f5" },
                { "uk", "a7c5ba6f766e8e7e6e2bd641992dfe9acae3795bd3dfe49a0d96a341699e9d8c555ae463d8004e6a8184bc79955534a2b21502237d3d738e6fd9773157a00fc0" },
                { "ur", "39684b31656b637db5e1abf915b18d0cdf2014f12e8eb4fad2f38634196bf3b2a306c589227108cefabf96f9a2db18bbe1af6875ac9cc420586cb01ee96055c8" },
                { "uz", "b57f8846c76a9d4012f61006d39610836ca7471a9ba9900a5ede1ce4e2c47947bdcb5a91502fd05ca1dd985541b4d4eef631f4f52d2f33bdc4dab6d056837829" },
                { "vi", "2b343d5a37a01c2533ab1cebf701e4a7af1cd1fc809683421c7f00bc5c34444b2a0e1b9358b2d3dce58c18d49689bc465add1b06226ee48af9cdec09713dc90d" },
                { "xh", "45b6ec96bc97c4ed3e99e65dcad8d00a2a438994391c400eaa300dfc175595c43f4f2863b37722569ac5cdae99c9802d29af5206be36447fd09f685b7ffc3573" },
                { "zh-CN", "25193719060cf4fe7912d812e3d3e00973fb888f4f5d9a6f14501ad70ee6ad88d35252df64965e2d57ef84679aa740c96457e4e6d60f8f649a40a3b2414c9007" },
                { "zh-TW", "be3058df9f8645d4311d0e777fb9884ac8f81360ccce628e1bd49b72a73a0cb975d17e9ec56fc6ddd3b7c850fa877ae9d26c657541232527e8950425cc711af3" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/127.0.2/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "b286e6cf9fee6e16b5970646de82fc845887e74f58003c67662b8b5ac696745bd56f076fb70aa635991319554c9dbfa15684c5175c3a95a4f6307237c2af2a13" },
                { "af", "dd3b7fefcfc533e927fbd9f8e9a2a051b34efdfcb4fcce040e858f6ca6b75eaf2c1970d9cb7736263cde323b0f6ed50c0d5277260c00e70308cb698b10303316" },
                { "an", "5a4165344d5d1adcdb8a92f074e9accf37112b109da498814ae7ea5e059a7ae722a51286d27031c110eca587ba59d33cda15cd885e4ae4a258f729c0c5fa4306" },
                { "ar", "0d993cc09c22bcfa7058b0d0133189b435788c598a877e1401bda941a3346e2b15b6212c0314dce12983fbfa981dccca166f618a520d6e25874a2e45c0383430" },
                { "ast", "72492a16a52fdddd98292232e51b816c52e2b6fc72afc4820ab57209a0d44bb103f19080facb28a5b105bbda891377e4e8180883f909990a1beb5b171c1350e8" },
                { "az", "25c5b8d3942ba1131d05742b54579b745a50b7b19ec8822c4467cdca6b311734731a6d0c0d140191cedefbc3ab293321c56389361b9aa8509126bfd268d11251" },
                { "be", "2d59f709708f1f270bbec50338284cd1e5c57f4f3337648715cd14744f38c38dd0c13738d61a0db9011a41dbeb38bda0eebbc0dcdea300a333844b87a410fcb6" },
                { "bg", "1740d1aba8bb3ce0abe4fde42694459624796ab02b41d3c36c86cf0e53c346392f71c422d04e117882d2b4cd4d606b54e2261716dab8352d0980b59ec8dd05ed" },
                { "bn", "b7f2b87285819e1c3f8f75ef9b13c8a090880f714a824790eb8e4f9db9974e4b0ded4dd4192153ca9dbfd13e718529c92428fc072dfb5c9d2a42bc1fc745bf95" },
                { "br", "255592ef2982dde09b1e45f2db458d9da45ab30d61d99259eed62267c59922de2a21815563867e7f10b62276ee5db9a989c9ca90b012db02571c2783cc1c9267" },
                { "bs", "f0d18f75ee2001fd63dc343a6613a873c9e6a7b8679a894f2fe7ccf7617da291633dca8f907bf7b294e8929cf4b2c27f48e13ef574edbe8cd5e5631e27d18c7c" },
                { "ca", "31eebeb412bc54b0314c350074f505d819bf72dab5d1fdf91289dd0730f3b64041b5067dcddc59067a3785a92f22298fe2f7db2bcdec0852f6242a387a2ed8fd" },
                { "cak", "4d77556a47d31b7ccf349562f9ccdf866571fbf7e25f81713c496fccd72fa4e9b68c123022370223f4d59c9876d7a20ac456365911a5f1b2ba2f35a71e3fd1e0" },
                { "cs", "3e4e9b5a8f37caff530a1c87584d8913c1512ae758b35fa5f47fac8a243eff30c5bdcf7c2f40d05e36074ff5b26e6717f91e62476d106021f49af75835467ad7" },
                { "cy", "4fdcd89dab1dd658a1aff39c196bfaf38dbead8839615bf268728c46fde3c3f04272c3c252928b4505b8d253c4ac9da03ec67ad18871beda8048d6bfb7204d21" },
                { "da", "04da93d0989363d4e159a57b07c710739e536d0811dd7ed9dcdc495785dbdfa991923bebe4baceabc9c35f0fdfdf187acd95e0b27d460a82aae3c42b70f5ae8f" },
                { "de", "5a3db07d7dd3791663f4421282f6cf6aa179e1b8633a9a68ac14448a0e965c6f52d37f2797362ac4e1b66a94ca182192c58830234a42864b285088dacbab4dc4" },
                { "dsb", "d2d670aea05ec611e5b1665f22db88b961de24b2aca21f8679b45f3f3b417e92b599ff20071e65dadb0eabd4d298297c087000b93fa6782b877d1c8a3c3aaadb" },
                { "el", "3862531eb0aef7d098109ea28ed16567650c19b16bedcd9bdfd203d1a38b4856db511cf70ac1a90c64c17a630e458b7a0343985706f22c7af82d0f3cf1855c89" },
                { "en-CA", "8e2bcaf4c0a136190e809b6b5bb10e6c9546361ef1880e4168033ff11c733b34d407fe4842c71fe658a704b40ae2e45a4fedc507338eda239b3c39fd7cd21eb5" },
                { "en-GB", "5da51441dbac03fb30ac3c649e7d659818060c9b6dc0d489648d452cdfe95109cfdbe9914dff9e66a2aba3cf97e896022223e4bab83f0e58135bc47b173f3cb0" },
                { "en-US", "6d064d58da18b56b34d83daf269ec958c547adc876a01a6f3c19b59bea2acca08bd5c8bc280b281539396f557babb4136ceb57179e787393c8c99df44efedc64" },
                { "eo", "7e68091b8d05e9a56d4faa84c2eda466d1349d44af168d98450a04e3cb186a9af6452fa7acba7157e61b2c7ff772bfdb7c13ad08d764cf6231cdb84d533ef39a" },
                { "es-AR", "4d5a1c5a68c41bbaea9f6978468bf0bbbe526d7fc7bef9290bdb8d201e358b8d8fb7a2356a92d66a58d4b7cbef74cb1c45df35461d0badd6aafb25d6405fc5b7" },
                { "es-CL", "2bed4255727cc0a6962e21e658389d1f3986824023ca3ba63b2409473118f63ee25295ddc3f5297574d4f09ed8dcb335c2a77dcea19286c607f7c85ddbdbc41c" },
                { "es-ES", "21dc9c054e51c44c6737805cdbe9897263960a7f6c5a9081baac44f556876f1a2e58303a680276dbc1fec7b047c4aac77dd6588719e7fe52b893da1ccfd84505" },
                { "es-MX", "87ab04f4023e2876ae52a5d9d74b3d0a39b73adbf1b3323078d035b9260ea7870daddc276f69e467266e831afc688a4d618ba7c86d91817ed0c9c07f77bffc61" },
                { "et", "7f3afe2876d2d0088ff5c746633b71cbeb66a299797842c60693dcaa46e56a852abefb0f27dac5bd132bc934f254810ce01974117d034d46466a29efa1144075" },
                { "eu", "d0df145fb15d47abafe07c318e434dce834b02937d2b73772cfd27cef2d21d1d3ab62565cc269ba09990ef7f086922dccbe9b61447902294a4df676ab05b65d2" },
                { "fa", "67eaba8cc2e2bc2c363848f74dad66b4404b1310604c8680ccb3f16a158e6a98cc41a28b7946d737160858a4636d00a6890192456f88b69bd8aebffee9afe1b7" },
                { "ff", "5cb35f7e9cb2f2219c9133e736be7fd901594aaa5d44545a016773d09145e0c2fdce85252735a30ff82d1de5a22a65db5fc4d51938a827fcc3620628f6f38cb6" },
                { "fi", "997d6eb5ca21a0ad93b4b7c7cf7c3ad475e972e964b7b3709f5c09436621be239d645aa419fc8bfe448addc126b29d9a6a151c5d3343e8aa6b1c8333d5676fb6" },
                { "fr", "5429d8621884fd19252c36cd5f6d612dd73e1e4a563a46660eb21646e3628c7b8bf748850db5afbf4f35c1f09d18e3c473d74b41b66b36773cba74959a108d02" },
                { "fur", "b20fa200c3504780be530bf2b0adc354e280e8701881ac72f9c757f6c3e5ae10aedb52628c1dd5ac864032680f127be427bfd33bd063dc211172e27079e6ab0b" },
                { "fy-NL", "f90a813a1d3c9d2ddd88e751e6c52e408d4a5988f1c41f00f104da414c19474fd1bec6ef3b620b7a656911948a17a9fb195e719674036c761efcdb21fcfdbe21" },
                { "ga-IE", "905a6befeb4a625982fb1653780e929c2331a8a3e048798b1476a185e37deefe7bb35633e979f07b09ed0fe8672f75c9ceb3bd2af9bc101de60c56797ce0fe7c" },
                { "gd", "c3a30b2a21e4c8380198af20149f9165094f1467322193c8ddc2e751cb048378ea2d6a591b5771ef8207804523cccbfa6992a488944c60ac93ba68a5d4608b9f" },
                { "gl", "a570d65b346d7a55c58193b99f4e45fd4bd61e12eefa2d978275cdab98d827710d7eb2ff9c0ba8e7fb3a477554ebfc38ce68898e00a985d887039dbd7163cde2" },
                { "gn", "a0ce82eb0d839fc9a0bdd2fec768ce99e23ac54f49f48b8f6bed2e9a323d3c7fe46600092bfda074ec6813d01aa25bf7e47817eecd592aa14d1fbe1b0a19825f" },
                { "gu-IN", "faa571cb24982dff46ee74e030361aee7835e297bd2d45d088a46280692029f545bf812a7cae83d6b6e7c541216acfdfad631fb9191e80a8b9c70560cd79f4ad" },
                { "he", "928d55167202423a36ee31d867faa435ecacd5d8c6494d0f5d7a95d7c244afdcccefabebdf4c8ef1b009f6e839ab1d6d1b296710c380b6e1884747f48f7f8214" },
                { "hi-IN", "002e2b75c6daec9351b97237edabfee2e7a4439e1c71373107a231d91467f463af0183aef4274d055993b264f31f270f3a1a52b1db1ab54fb32f4c47bc00593e" },
                { "hr", "698ee7d71b579187469118205ef01cc5a6aaa47aefd605a3cfe510bfd55bfec20a8e1952a693b9cc363823d99d8b70dc5aa155cebd89c6424b4dd00f14072d12" },
                { "hsb", "49447bec3b2dc9e86f7ec6c08ed4bd03157b6ce1f68b183201a0790b861ce32b0aea41b5b8db9d579bae16a33b4eb05ecfbd0ea059481f0fd999f9c45be8fc1e" },
                { "hu", "7733c3f1f499bd24b7fab56b362c62ccc13dca0bece3d59d865bac0207427a304d7cfc8cffea71b896f987c37d12fcd924668dc1115e411c07ebd8d330b6ae68" },
                { "hy-AM", "b7d691e07067ed4e57fa8cc5d4120fe88e4580ee5389a5c6435f9da5bea1e5e9a364f3dafeee049909c447dc5a9ece3e0fc79c469f902cbac5f641940467fc46" },
                { "ia", "7ec401d25cf3124ef951c07f4d67f4c5d8eeda5966112a56001d31afecc2f255ce937c1894a04ae3552b9735b6c3385b9cd89d2919260d77833d8cd143267973" },
                { "id", "65aad9783cac822e7467b641e16053f57cb37ae725226e6541979ff0656fa26a11a14e7f421e1456379e0683fd1f7581c18495b93cb400dc660651306ae1df07" },
                { "is", "c7ede4df7f32e076ee3b534323d3c41530beafae17081b1ae5281c79f904297f916cfe7cab30ad1f6776fe623b7142548f354b7d2b4f4262bc25650b447c1472" },
                { "it", "c22da69131b790cac665c9d039aa9d09f423fba4d6e9dd7f9bb272129af8dece9004a2446a2442d1f48f4f74c413f8950c8dad85e3c7409c50e94eb0f98ffb50" },
                { "ja", "7f5a7f4ae8038a683e768acbc30e43a35dfdeb1ed55a0842c869ddb2fb68c24f7976586d1d2aee386350f1527679ce66f18b8ded465b62059539490064b31a21" },
                { "ka", "22f6aebdce9b60c62c59fb09a0bd12f4ed134032da05164ba6d373cf331d37773717b79f66d014464c5943f322d05f83bcc119e6f0d25a51c3ac3dad02507a77" },
                { "kab", "d32d6ea00b45260af7d7e67162be1b53274e32aabfd8b1ff2eadc6120a3a256959ea131465379506874ef967a0022404b569c34a0e218817af4ce1bb4631bcf2" },
                { "kk", "d70275dd287ffeb8a3d3ce4ddc4fd754db8240ae2f380118ae2da35406dedbbf4ccbb26b9550f09c25fe2979fbabfdc12cd1106f5ab85324b1437c5361b47d28" },
                { "km", "51821541b0545e8041a66f740f4c754fcd46e0d0a905181573f7109361675dc0926c08984038c9fa8461de0c1dbafaef5a39c45d594d11a86e181d8c3c757578" },
                { "kn", "0588ec8af593db92941e2c7629c383fc5f0e4fd487ca18752c60da5b16c1dc300e2eb3eee843cd229f3b95ef1666521ce2e9bb579ed63830f0aaf4ac1de85f64" },
                { "ko", "0fb405e8bc591d387c74d6f9fea57ab999fc6387d0d1fae74b3a5638fefdba0309bd1c96520a64b51ea2795efd29b2b7d01af169e691a2f80b2e7e65e4bfb433" },
                { "lij", "8ccd09528cc5dcb595d79df5401eee5e786785e1c4711c277ea75fe1dfaad34c65dd93428834fd443ce4f45fe96d39ecb2ed9cf25a68ee4d4a192c1d85a5e853" },
                { "lt", "e4cfe8c7f5b2e8a926689192a48dc0ed86a66effedecbf40219e52176e7db75c1d8db9e5dae3811aa749ff7a661d4d14eb805c3a7a9f3f458372bbb14f0039d5" },
                { "lv", "d74dfd77f8e01de6adc5188c7414e3d3a2fc15e05dc365114bf2168c79e8f4bfa930d4bd62982e1a8776544dada83aa14699dff9ebd82361fca35290bdba14cd" },
                { "mk", "cde8b2216417edca32ce358c8dba6315aced67afae0da888b76db197821a0a51b48470c1e23dd66f13c4f41bd0d07b46882d4c0e99329c9690441646f6a2da51" },
                { "mr", "4039103a9829345aa58c75cf473c77eff896f675f26289e5e17bcba3f875e2dc6c80c1b696c6336b954dc8f39b504db158aa444f984a518571f6a5e5ad7a9bbb" },
                { "ms", "bd72b367dd03ef103c60c51984058efc5838c987987b00e0799b319bc1bbbb3e986e5bd918606092e4a7c0ce928e83b1d15a18c7690552eb9670d345c5c401e6" },
                { "my", "55895aaceecfb49f0ae3c6e9b59113e4aaed711c1e19005143bd5989a61b37d2444d5a7e59f97668073168fdf219949e8744dc974dd8e4a06647702e0bd31363" },
                { "nb-NO", "dd1c6c0987548019ba633edec3fbce394acfa5cd8326e511a6e82eb0efca9216b690bb6a18d9dfaebb1a577518fe4bc2c192d5f70691e1eadd30ba3de26f84ff" },
                { "ne-NP", "a3ae579f1905de44d2483183ccf7ebf7a17c79bf41c4aa780db7a29274a94691695ba284851a12286ec7756a978d236014f57551f69247308dad58805a80b71e" },
                { "nl", "e91bffc04e870af3cdb134832031425d4ee302b7ede202e6b1b10c5fc8842c97c38d17eb1019029f79e65ac86d49bc21856bc9478bf9cddaa44272b1749ee06e" },
                { "nn-NO", "11d43211502f2ac8aa59fa865d4f3044cf7d272b1ef99cd468253ffd37ccf016cc1b16a766057a663748e2d5bd80c010eb11d0e3903de85b1478da0991e4d726" },
                { "oc", "4903c7793cecf76400ccdbb67a4ff22f6435a5bad7c16639c3492a5ff41eeec23d464c846d2a3a17d3e72a76b3c69ab4a48f86e082330a0cc3bbdf9aacb63221" },
                { "pa-IN", "be5a113a51aef6295347aa18ffe38afe1505e609ce79acc47ce884aadb4777d590676a11b0f65692e18bb5baaccb4cbd0ad0914c131f8821e2afead3722af24a" },
                { "pl", "39bcf63b3809ae36ff4359dbf39dd32a392c495caffd337f8fdc4c95e8d5eb82f98f072e82c3127d429771df30ea51be50c8e6f31d4c18950becb7fa3d0122ca" },
                { "pt-BR", "e2d4bfd7e77d5e1b8907055215e7c47c1eaf27c9a3ed204cf566bbeff94ec0666d58b5c94f0258a003fcd09afb1122932677cf291ad0e6f57891d5deebff045c" },
                { "pt-PT", "70f6fa1f9e7a4474306fd59f53d404ef0c71cbe00532f13bdf51b3c1046e8e58d9f46b9aa4adacad86a37bda2203f5857c08255511423331cc1c8e2d59811caa" },
                { "rm", "1208e0859d0cb1848f51c186b4fedf36f46a4d6d02e0fbc10379f3f6cb9e13ae67f8dd98ee0e914d71b8c6ada77a3adedbf1a7eb3ed388da6be720f020020e5c" },
                { "ro", "32e5a3a2b6fed225bd83836a07383f6becd045da39160c9236bc37421338650e70175c446f416beff80f97f94bb87724de9d419490cf43dfce4c95bbbea4e981" },
                { "ru", "7a835a6a3dc87950fe9344c67fb434dc82ca4b9bc5c737ef47617221c37b3e2b2cd49e2cf775adfe9cd01ac1ffbcc415a433341dcaf384d85f150f6b83a7313b" },
                { "sat", "01e9612ac7efd9896a223e224598f202b8533f77dc6ffb3dacef7c8a427a2d0481cc186a834da5cd748f8ca18d987d43d39b9c4631d3d0aa4cd0b8636ce8e58a" },
                { "sc", "ee2939b648c0a40e9db115eda6f225b7edcad94f25710eae76d17cf2674ba19238b2773d0414d9078bc8fa772a56cbaf6bc5330632993d861fdd6120ed5b990f" },
                { "sco", "6c81934c72f37ba37adbaebcc2bc4a3caba20eded03685c918f2be9e48ae4a1fff1e08e0006b998057ceefc8523ca6bf74adb13d2cd39a8b328eeed27eb653e9" },
                { "si", "729b8bd5dd709fd0f8c36e38fa7bf6fcf805ed1859491dc03de9aedbbb3a552462940284ed69022081e6635214de2436cd5392070ce3c9f7b6f0b4b277a6f3a3" },
                { "sk", "308f73873489b5c8322e9c3e133449d79c160eb05c2df0e19774b90213ed2855f2908e706bc4b4ea4bad7ddc18b446b1c6dd5a4c178b203a690a75ef8ae4a6a2" },
                { "sl", "059807754df971550d03c632ee19061bb0cd76cdaabb3a9fecc412bc0f7d19f9d8fbdd9e1858da8ddf9c9b9713eee91921711b93301093bef7816e37f3b1fbf4" },
                { "son", "d6e4849f3bbfed7666db31a84c1bfab2136e48483e29ffbc8cf5d5bc04335b35ac1ee1e8a05d2477932f7deff43fc2788e727a07d3054f08675bd712be896439" },
                { "sq", "d77d276a6ff840c1571fa3f698bcae384991e8263895f6742c79533c8b4a00950abcdbe2ede4a4589a72bb74b99b714cd5b69dc5b238bd78da7067ef37d4af22" },
                { "sr", "993c0c506f89f2c7b2b492704aa50d191a24661463fbc81b7a8d570fc204d7ce302cb7cfc3676f40e1efbc966ce083ecb5e1fc3bf8d4de91c5b8d760541cda0e" },
                { "sv-SE", "12c08e3439af65251118edd8966e48d7efd1fb8ecfca761b1408cf48409c5b685a5345b5a04cbfdfff689ce283f5c60a366394b035d97ef8db28000f027ee59d" },
                { "szl", "358beea9fe4700152a7645101cbb3652a2a3b7df7b9458dbb4391d5cc7fb0ed426c3a10257d03f953d37d394c2a271513b53e3a06963d80e32efb8371a29ca72" },
                { "ta", "979c213a00a0dee1dbb36fea9e60ad95ee5bca8b2a3e3c092b8ccb9f976068a92267eafa5a638aa3cdab17f5baba06e52007efc30e7b9cd61b6c7349d643dd42" },
                { "te", "397ac587c0d43d7d280fef2dcd247d6ba87548c8895fe0bb4f4d6a230d4c16fe36d2c45e90c3fe1417e28ea7b2cafeb3f81725dbef1bf3f129f66a1461411518" },
                { "tg", "c98439f645ae9098c9065dd48aec921db0e1d5d9fccea408fbdbb298fbdf68bd837b8c13b15ea6c717f66f032fcf4fa1c641af96ee652d62c06a1941567fe3a0" },
                { "th", "194e3caddbe7bd692dd28ac8f6c1f9373def5e07270a63a5bc3b0b8f31db86046b7aed4bd416144c6209d41afca63df30240a15c9e9b83fcaead1cde8255b0b7" },
                { "tl", "a2a6f53e1287a809b346476a9357c1d0d8b49a55722f3cebb9b1aa8355d65d5267e5b25a07ded6a6eb090668451b93116744a449e77bdb8f796b54e5d1032a9c" },
                { "tr", "e19fc98e2220e04e7a30b4762458843202bdb300367a96ee2f43dacbe74abc4b92b7fd16a3fe030c1f8fcbc9cf8c8ce6fed4e147f3aeaf1b8e804f6bed21fa4c" },
                { "trs", "1ea260733dbf5a96dc0f475d4aebc646367cde0e7fb97aada9ac8e91a66cba67438e32a4e749952adeb33909b6d01a870ff9a7286ec774c8edac6e5196c3bf7b" },
                { "uk", "b55203fe0be6a66d7c426e649ca575c1def845c994e3e4106703fa9b74b92f6dd6521f32dc5246999fef4704701c51fc359586a2faafef7903eff20970a69a9a" },
                { "ur", "144cf6ff64e106c944b45983c09cf6be83f22d247a2f5c34f0f3c562fb5a9d01485ecba7d6be3f25968cdf5d57ae264b6f8ef0edaa12001ed7412a08a4d997a4" },
                { "uz", "3b838c04109ecebd3e9e67a2ec8363e0a7c3b814b6d3120252bd369d7d62381a277ebe33da315197b52404afbab9b18a8fcab844cc400329df5f6e8754e7d16d" },
                { "vi", "314f5201fa537d2e8effaf3c5f07f7840b99041bf543495ad5475191e09298cd26a167303c085c3cd0489a8bcc27645739667b636bc6df52fc9ae464e8e78a03" },
                { "xh", "36f707838ca55feb6e4753e38f0af6d4d221ca5cfd724598e74e14799fd9365246b6e30c28bece3da4def71837850c2024011804f779f608baefb93e4d094f69" },
                { "zh-CN", "76efb6cd36346515de12b12accdac13fa8aaeb754bbfe69de54aac582f5163ac7413692c66b7d60d2b31acd5b434d50304c522498a968c816a30b125251fad20" },
                { "zh-TW", "056e7700641e1a68fbe3db3d366889f64c53500ed1bc0bc71944cbacc883b7c65a304c4cf1afc573e311d54bbe4598f12a2ee14049ad52e2653ab99320bcc198" }
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
            const string knownVersion = "127.0.2";
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Mozilla Firefox (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32-bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64-bit installer
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
        /// <returns>Returns a string array containing the checksums for 32-bit and 64-bit (in that order), if successful.
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

            // look for line with the correct language code and version for 32-bit
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64-bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // checksum is the first 128 characters of the match
            return new string[] { matchChecksum32Bit.Value[..128], matchChecksum64Bit.Value[..128] };
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
            logger.Info("Searching for newer version of Firefox...");
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
        /// checksum for the 32-bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64-bit installer
        /// </summary>
        private readonly string checksum64Bit;
    } // class
} // namespace
