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
        private const string currentVersion = "126.0b6";

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
            // https://ftp.mozilla.org/pub/devedition/releases/126.0b6/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "158ece5c4da09d6ce4f97a3c8b0c4d1ac485a5986f20e8101664c37a2fd897124164bd0dd9f9ec8225eff67d256fb4b377053280ec2a7844bdfaa9f0fa7303f5" },
                { "af", "5153fe891f8d7382a2d634acd193de582e5f9e01e4ff724d7e1bfcdeefc52daae9564807f20ec5da2528aa6c0342e8c5469172d6266cde4ac3ccbd2ce1c6a894" },
                { "an", "1ffafa6c0f6b199fefd4e37280737384b30225e019c0d0080e9c8311954794ef4519f02efe07ca02fbf820d967138083ee76b01487363bff59e08eae93169a6f" },
                { "ar", "f8ea74c69a33280bc35b9b09930c962ee5ab7a9162f92812d6967bd658ce7d6cd3f524e76b6e860493a4950420c39548e6d5df0caebbf6b9fb1e2b88debdd66a" },
                { "ast", "bb69ca5227433471eb7eec0ef25d8d5ff2296610ae2e23b06d6efc39f6706ef089e64d4734bac709f2106adf1f2e281243ffa18e6e33675fff8b4691e3e8bf2b" },
                { "az", "97706e15930046c0cb75f72ff79f406e9b2ebd5a623e0dc10f3f0fe0940380e72a83677830fafee133d5da9706f8e23c030cb7a7ed6bceed434ca2035b2d3557" },
                { "be", "86543be03302b3c53949225be9c83af384d82043357ad31e03b9451528f1cd532189443645fd8a12ba4be6e4fcbb2f10a928ee4e2a8b413a6e3a0d98cb42bd70" },
                { "bg", "36ac880a0163ceaa7f41f2bbb59cf1aa7442380a37cf981442faa3606c430af637cea198a6e2aa96b6104823989289c0fdd5b1cdb6662e1c8cae5eadbe966675" },
                { "bn", "606661b849fa7ebe307746eeab433ac767dd0ebc0024e930c45c11877bfb7af50373fc5feb413eb4fc002ae0a2693f87a3532f2ea9e9b4fa6beb8af453acff31" },
                { "br", "e6144bdd09ae8f80c6056cfffa1fd3499e6a8a699490ea5c54d1e4a79d0f8c137d570ab7c8e53fabc73c4222bd020ebe98392197dbf18157cf8d89cd4e63732d" },
                { "bs", "10f0deff9f26e42f48fe15d4799b919a13433ac17cab392bdc5d9ae42b6ac8b699d018aaec000b11eecbc092678b8a2e0a5e66af73154a067a55686d30b80186" },
                { "ca", "1196a8a137f72fe688aef81b8a38340a702d54ed3023c7a2ba5fc336f8eb51ea4a35dd1a3b1e002f6638e80fbbbfafc9572716ee03e3bd47d7e64bbf9eead1fe" },
                { "cak", "ee0da529b6a813a79dce59e108c6d294f356d445bb8a0f9c26d185ad6779d10c30bbb61e44e023c2a3ca211951561c98dae4b17aa2afd38d18b26d2d99efbf15" },
                { "cs", "92ef44c2510278e535509988df05cd2e11f1b13e0e6d9273b47d59ba9d0c8e8f31919d3a51d07313f8a63ec3a7dcd2bbcc803aa0ff6cfa8aaca2bf0c7c914628" },
                { "cy", "be3403567b21f6d790882b8a9fc8624a8420210254c4922812efb4fd430925d12a56fe825aa99d25a4cab579638b09b1e5ade81315dbd47f499a2afd1bc4c369" },
                { "da", "515b49f41c83f26edf54cee5dad481be3d0ed9d338ff644251d72f520fecb43449a1d2edf2df961d8a206ff8ac5bab2bb047975c8ab1fb19656d2cb9306ee75a" },
                { "de", "68834ab3e62056ac245541c59cd0657223661d54d296c9944bd2e6d2740e169916d89a0e14b53655e93af0df33d4ad09dcfa6bf6c06a229708f02e89a35579dd" },
                { "dsb", "0d543dc7847bd60613fe064c24a31f1fa0a5cf4638eac4e22b1bad1ce902f6053dc0304e178c007fb6f0e45224be98fe8833c80b0a536e1a703b1f6bac50e93c" },
                { "el", "9e2ee1587cb10c200719a21983204ca41e52b3062c61de6dfb73c176b37a80045f21e93f8b8bccb7cdb5f265ceea0fb39e4d55599596e7e8afa5e9cf6dd8c508" },
                { "en-CA", "069f182b6ae78ca88fa520a1d1f653c40e6e089b0f030b95bf5050c1bae1ad20121efffdf0b9d76fc401213b2e42a50e0e8c21d292fa1a6275ee01c0e03ba3ef" },
                { "en-GB", "aa5a2c9cd86c758eb83cc724d9d02ae76cd47e8c3f5181642fd6cf4b6abc51553a259cc6f1b836b1e23024e29d7c49a9e1073673ce65a8790e3fc73967bc6da5" },
                { "en-US", "d4040662c49f8ff4b4282ef96efa98a4388d91e069918c77b025d25bcf655c646ccb4e8c52fc8ea3ab2fa4cd28f235b97224761a739f066799b2d227bc92579f" },
                { "eo", "1142fa8d4e7e76fa068ac8bdd2c2a4edd75938921be606ea7faad9b92cba43cc5dfc785a50554b48067bdb46201c94f9d19081ebde71803c53eff9a9ac53842d" },
                { "es-AR", "34f060836af10210cef80c0662ff6cdc859e0fc089eb45e3108af63c1d3cb03f87bdce62ed77ef8b13148ba803e4724ff635ed6843db0affa620997a8ceeb828" },
                { "es-CL", "ef5e6ea5dde339d42ef0e5ca0da946dcc4d20d8d6705bee4d35f6d48e403f837dc0ee2fcd70ea34afc6485fd6a8d32b1862ac4f19d112c139710a5eb78a4ad2d" },
                { "es-ES", "cce89e9335d6445c337759e5bc0ada38290ce1a352b2d89aa96ec9b01b1e9d6d1702a163dc50c1b749016e70fb30e9c2a96b9c94979ea1c3ff339b65b576963b" },
                { "es-MX", "67fd60ff5a1f3988a54a368113039174e9cde2619972834f60108c2f1cf289662edda180a8c7cb02c3fa7760614ad35c4f4f0a8a0c0181ddf49d7b8811fe665c" },
                { "et", "59ad864bf00187dfee01d496edd2fe5579013bf3ea97d62d383279d93c780d31bb51f5612e0e2c4968c1cf95e27538a8cd742995014b27a6106904fe9ad29c5a" },
                { "eu", "05c652c0d54de3a926e0de845ee2ca734223c6bddc95547e877d0cdd1a47c6f11371fb5126821198675f50c9010c33edcce80dc1fea1c397856a490044a059b5" },
                { "fa", "a0efd9e6bd60f637af4219f144f398683fa47d83469846017eab142dccaf637d862e4080d3a17c122d24e7817b10bd12efd10ddd2b50ce248c871a1a0897577b" },
                { "ff", "0bfb6b9f52f566f5e98b9b199742dee00e0eec201ccbe38e826a2c38db9a604fc3a3a21beb9482337e38d40c7631e5d4e921779f8186bb29a665a96f94e38a70" },
                { "fi", "d86b4ce96f9cacc2ceefdf36e59976eb47c09c19c1c11ffe3cca5a43896876b0f161630784e236beafd88d12ad80605e8675817c2e5d443bfe9cbe176316651b" },
                { "fr", "acf3a677c01a65c57463d0652bedd46f95a09b984528d6992e3177669d74005de712a0318dc1ea77d20eff98505bc7e86f31c9f7ac79bee5474ab69dcf22257f" },
                { "fur", "c6686d2e470cf45594f8acb797a9481873dd7c406ccef95095b8d0cbebfe7b915975c05d4bf033b9f6b00b59c93c99b1f73303f1ff3261d631c29ca382f7c3c1" },
                { "fy-NL", "ebd7aee33c51e2d1047ed3b10552631dde4f346ca2f9d2b763d49f6699cfc3b99908198fb06e777187185eac1eaf477c66562b47bb853f6b4108db12ce06698f" },
                { "ga-IE", "07048489d750128880311d4eeb2cbe0ccfc07e9a234cc10fc5bf3de43453fee92d972a90ea683656a960fdf556fe8164f2148d5672c5635320f6f93d6f41aad9" },
                { "gd", "6aa29c7ee82b479a1d44ac338eb97eef111523ac08bf5ba116dd0996a767b1eee748af6e133d09a8087088d777a085d1ebe889b8264edd112c81c92b59e75b9c" },
                { "gl", "d67c7d62887818b3822c841d25ecbdd354eb88d2fb1d76d4fc20d54958827e6ccde401103f4b78b9cc0d6e10931c114353fba71498953a8272e140bae860d13f" },
                { "gn", "933024663787fe24d7d3ba318d925b324e388455347c4878c30667efbe5bc9dbefd380609fba2ab283575ecefe572c7055517b877a7b5037f2282cf6365297a6" },
                { "gu-IN", "e13ca93c3c3ceddf8598d99c88c995c2bf798436fde7df921e67059df8666647a63ca12f08c5837340f02b4d11794d4db86b428792531157bd1751021596bd4f" },
                { "he", "4cf016b8d7ffefd3bb310cd777889c026bc2aabc656832c500b9b4e0051fbd9ee6b046aacf5b6aac4222287ab9bf356236eb0156e3513ee6b38fedb1497f8e25" },
                { "hi-IN", "12484629adc28dcd5bf1bdbdea16feb0ac18a6e91dcd362b6baf756ccdbe7eadc67cfc6c2a7779e5415e4cdae60134c5885fdca695e8b641b2e792f56de7b296" },
                { "hr", "dfb93f1d3cae5293f333dc7f834cee10d99a38f282e382b17f7b9ef8b3d1774365cfed9e5f61c115ecd16b8361c2cf86a42284506871417606672c33a908a0e6" },
                { "hsb", "a688e507b2ec4debb105181d6f10dcc533bb888a2546241b9468386d847c367d37a818aa28797d704cd3f4d81bb4bae42d98829304b14f9221f3beea1f8f65ac" },
                { "hu", "c1213c5a4a3d54f0b4fb8cec97b871517c40435e1c2162ee6917e0817cea48724579a9769d6a81e99621e77b6f0c926a987872880fb6fca28226e8c7507b3b42" },
                { "hy-AM", "efcbdc95d5acdebc15f84452363cb9676aaa6f9b281a131bacccac8f2e703124058df25384a0f87f25f38e1bc37be1881f98e2358ae0ca72c428f8b82f758585" },
                { "ia", "bede3bad9092de34a52a657ce62c23290dd3d6277987f6d3ead4972a1b899e8dbc78ebcf2faa10fe860a434d823f1c08291f8e3a62a2dfd0d4087f55b7ef5f6a" },
                { "id", "44f86cc3c92c14fcf80b1aa515ab6f9a30325fb6b97a450993a460ba240618d3efe0964ffa837d54355ff7a8a3d9c15eb5258692718639e3b045a3fded6b9afd" },
                { "is", "195dcd2e91975560102a1e92b76a0834056a0ce41886f9852c5927292822902c9037548de3c217a9224ca6be007c5304947199ff2bdb40a1d7315b4a460cef1d" },
                { "it", "79aa99dd1572c8401e44a52b1169956ace20dde33f79ac9037a1bceff8d16505531219aa0e975600dfc3195f12e0c64153951d06b1b1d7b2624e5ef13e32de60" },
                { "ja", "e64a11b2a69fb3e8d645e88dfa1523f42ae8115f0e08e95ba7a81cdce4fd7b578b9ad95ae83c5ab67dccd9d3d68bff0bcb5289cc59a20ee0e3dc85feea30758c" },
                { "ka", "15a646d1ca794c70f3f528b3e63cb405759bc824f413df49cc9e3b27aa63025fe5d9de8ff40d2af15a75b5c2dc0d3c5abc2098e6e035780b0cd47618bf918da4" },
                { "kab", "b8dd8c32884d20f63a2fcab159be75b0d4eb5f7e4954980dc7497dff129b8e9e8478e29c0eb7bc9ee95559119fdc764a40ddda8a454ac8342e964305bd886d08" },
                { "kk", "c2d9387238f8afc6503731e1d12a4f999b4a7a461f4d95c9c7770083675259917492a59c9b68df5a2b139bc51fdc63592c847941232825625c8643b896f6c70a" },
                { "km", "3740fa9418353ca147142ba87c7384ef82de908d87fa7e34fb6c2954aad4f7d7c1c2d3ef22f7bf30d95c6c1542bb0afc45d63c4e6b25a13561b548712715d86a" },
                { "kn", "a936aea610c0ef089ee6109084abe254c6378713802fec8784ef5111918c91de93c0b384f3764f0771923881f1253183a5b1756488f3b155c15243328d6cbc0f" },
                { "ko", "632024b432243daccf3c9e709747c95956ab56fa7a088004c71765f51b23c9ed717d4935c4d1e7d1d5c2da370b8cef96ef15cd0ff24508a7f39109a36c16060b" },
                { "lij", "02afa75eec70f4803d5e761d25495c1757c867310ffdc357ee01d034b6207c819df47baf24f495a735a439d0572a6b4c80a19c1b2d9381c8fb817a73b3df2dcb" },
                { "lt", "f3aab4e44035b659a1bf5d933379c4a10cb70bd55bff865668f23d3fbd48e49815895d1ea54bd04b4863c9fdb8437774e9aa1f6c0d465cbf9d48146e2e696a8f" },
                { "lv", "cc1e2d24c78374e81a4b2058f45ed5f32a91c965c7be0fc389bd1abc26ed30b65db5abbd66d51595547eb480804dd41aa831cb6103ec1d95e0e80d5604a3e3de" },
                { "mk", "c654b89b8cd9bbac23a7f4d59cf03be4165a0b8e7023d55a4f9005853fbd40489726870d1f4ab42923e2f5e327a64acfc0743529960c6a6790f6a264914dc24f" },
                { "mr", "e3cf8b2ae4af146cd3a09ec75bcb1632bea7491c1b5f0af1d3243f9fdb02bd1f12829140811b518c09f918502033445197a125a7954daf070df0b6aae4704dea" },
                { "ms", "623f07e40999f8152880fc734a4208d2835f2f32ba13c1aac7c216348d72674aa5f20477a5b735a1a9ed5b111aed60bfeadfa6ca860c6928da9dc7b9b8128568" },
                { "my", "04a0a2bc013373ddec624d603b0a0fea97f7f1d9c04e5c0d8e7780e15be663da994a47391ef6e7e58c30ff3fca4e46cc82731ad9cee90284bb5fe002393aa85e" },
                { "nb-NO", "5cb70d6ebbfdb2fa3fd7169de409b795ad3aa7150b8275a316fbcd6ba82c6bbf5a19d5f156d5d66d2aa4f4e02da5ae2b50eebbff2c18e434747821d76a0c96d5" },
                { "ne-NP", "d72f965fc7cf992ee607849e79ba7189c7b2f5ac0744ba1fc4f59a29e123aa8b1ba94b2c343fae3ddc7b87bc953f61d57eff7bca1acf1c2f0c8427c574fd5d93" },
                { "nl", "2df20442e0ed61ef0302c4d135cd48c9ff329be9ea32c8d975a7033091464134921faca3b140bebff8677570330dc5d53e4f2fd2fd4f42a85db9e3c3c3d73ec7" },
                { "nn-NO", "6476a0c513e15081d0f6f5466664ed8d6e5a2b3dcd4ad57eae80ce9d9c79f5559dfc0e7fe4d68b10ff6028073d314f8ac3f1d3e5fec168bf5ff14a5fa4619c85" },
                { "oc", "a23a490a1cb46f15f1664f029090bea2b208457a42f50d3977b1b7f5d72a15e2e6859abfa87e771d078e55117b1bbe53697ec5c13d419b33458adfb9112638b5" },
                { "pa-IN", "093aaeb82a73d826ec6113e2d155a262afe61e91983f2d8df991306492947c23917aff7d1bfdb4c1ad4eb76445c6a21b0c66b930b01b1e8478de2275a98ee768" },
                { "pl", "e7f3bc58f6f73c9c838f21c79c4388efed37fb167baffcf1cb281961529bdb14ceeed8f29fbc2f2b5e6acba017d60c6398a2b924ca46b71241a36d60a8869332" },
                { "pt-BR", "f0bd0ff92063676eeae9a043cb8336d65d18c69d6048c2542a366401290079315f8c086a4ddc5aece2cc9006de4e5fc545866680e1fed15bfcb64be384c5d9aa" },
                { "pt-PT", "759766f9af795eb216f759d151f12565b2644060d45a7b48aed64e501d20a75fb94d370bb441b7118ec825db48f5ab2ba965beda20943cbddcb311be9aed72a2" },
                { "rm", "0daf9f17566b55fafe9e579f2930514c8f0036ab75ec9b82cd06224b1901f66713975c64f9e4de40f166e570e2c8abb9b659eb557e12c0d2d5d78c12a883ec06" },
                { "ro", "13bd5c9ddd9ab87a4b4ac8d7c3157b9888426428e877db721b29290e71664b75d71784de6019086f096011e644f8267e2823979c39760fb245890d51a8057e19" },
                { "ru", "308d1285cedb72cca8f96e45ee3fd5325e62fb5da28a654f5c8d412b984b918a58bffe18786eb6a8f950fe954f3982c0a71ca1ddd60e13cc079f3a546213a3bd" },
                { "sat", "4036b057890a3060e03caeb2f2034799235b1ec2171cd0c1b1e707647f55b8235831ac9c7e8dcde2d224f079968f10177aee076a608d1533cd2e0fdf8e7440f0" },
                { "sc", "e44ee3ba7816a52b73c6f656561c5fb325ae2f3a2bfc469837507c283252ab134ff5bad6ae5e3308923ce01ce09b627814676175a468a90fcfe7ca160eb54807" },
                { "sco", "9e0f96264b3ead4426f448cd31ef3a53a902bff5db24dedcfc4f0410ed5268700840b4e68fcedccd7ce62950cc494425db6a3a88e7b8b89df3c3992f765df46d" },
                { "si", "51976071c398e49e42bd69e3537996eb6d3d4bf2b477f041c2a6a63f5341ebbb98fbfc8e1fff87ef8a4c157cf7e224a15ff00f78ceeb8151b7efb15a8a99a07c" },
                { "sk", "858a658119a5102de5aed2fd6781f228d26f142da56fb3cda7cb6f72aedc6d2affeb0a94414b58001983daaebab21a48625a3b4c400ff00d1548c2b06c435db2" },
                { "sl", "a3a0a758b5fe5913cbaa786ff39563fd09547a90335507ce61622896291148be6c72d758dbffdeb584e853a8575550b61e1b0c78296c9ec661b8474dba43355d" },
                { "son", "ecce4de1d606b464278217bcf6ec883f3a4d7e2e78cb9a098c32a48f06f42b7370e20071eb7ff14fd5344912a548b536b2cd6c150f72f82e220c7ae7eb3dd6c0" },
                { "sq", "91f1be9adf41db077b59d7caf475e17700fdc8396fbcb39fb2575d48bcd7778b112c88645cc53a828e4e87bd16f8f5610322af519ba279fa393e9bf267202cb9" },
                { "sr", "a2f347dc3010569d8c285d9cda44360e9ad7faa0e3f3af517a02003e8858017eb40f1740474b2e61515d680d78f3ed86afd71cbf3255f28cd9fa1f990ded51d1" },
                { "sv-SE", "1ba0744c37d8aaad2b5fdeb98a232ea337a2dd15c99257705bb669c7f53831e4936960645275284c6d068d87fd1472e99d54ef68a926aaea4b351d23af3809ab" },
                { "szl", "37b684e0f2abb81935fc332fd2803f2fae2ca1c0f6ff5f54afebb04c0630532d88f462e3236950e65c113bb0687e098d1530918d614911192c4b16bad338d76e" },
                { "ta", "de137c31086fc1f32062fbe8fa99f71cac25da2a11cb96cbdd9b7a3bbc8710580e4e177a76e880e2be0c2410d97853e06f7370473131e39d71ba2aad698d1e3c" },
                { "te", "9d422d69b43460f81f7987ac125d385d3879fd1a7aa86816c5d9495281413e40cbf18ba9ec3b79926a0a66936357f9904313c46edd5c1ec32574a6f088462db2" },
                { "tg", "ea0672a9286535d5e4557063b00dd27729422b89f8d4264de5d19c5ed5da813354a48af1b3d592e7b69bb4836ca23e971867746f6d593a68b85c221aa5c210e9" },
                { "th", "70e1155be015b5f95b4f0cdf8dc250328833b8b77c60a0bba1f62b292796db12ee345911b2aa677485ce1e7c48abc31a01432b0a676706f7d4785f6b14949e74" },
                { "tl", "59da23d2e1894d2dafffe1071fd1a50e4954c23a0cf85b494f21934fa4a5c2d711d5bb2e9b45a226f57f73e0f48e35bc96881d163f94156f67b793cb86dcfcfc" },
                { "tr", "5a541ac5a7cb0e28d740a932c541956fd2d1f62ceae3a7b9b9a14f7c277a0c72144e5045c1c2e39bcfe833584c7f48117d31b28658881a4210c810a592094329" },
                { "trs", "3a619f39aeb18673bb597c2c825d3965a8a2131e911ab423dbbae8c761bb670429c4d11ce0549668574d70d60f5197fe386216c4f896e870c3b8df93f298604b" },
                { "uk", "8f5bb35220f9d19c29f8973e93ac4b3d17046a6de15107a16a7888f90e6ba5ec11a4e353fbb4fefa29b87ef25cc15d7ed60eb6eb07f04db60342aca445ee633d" },
                { "ur", "7900a6991116be038365a26cb2075ca42c9300dfa5389586a42f689dc635c50d90560c1516ea3e07224cbc141a6140160604b30ff48cfee95ed6728e558e0734" },
                { "uz", "6ca223bd1533761f4380222a803e3dc10f801b19339826e083b7c8abfe739b86a17ea2ee8d3e0702620f10caf52a3fd0146f34b8c707b5bcfdb32cdd51879b33" },
                { "vi", "9e9615516bb6beeafcf1e7db491cc17b25de06cad550d434c15f6fd63e35abd31d54881e9822df494838b0cf10be58309265b2c5584ba729d28aa64cd092a20a" },
                { "xh", "c6211b3517eb2ebcb0db7074d44e1a1ef487409d314fc0dbb09c1ac43861d85a155909d14aab5766c13471f86131b8e5e5156ce40ced4319e9e2170e126fa52e" },
                { "zh-CN", "fbd55e8fcd5b19f9ba83d7ffe857a624b4d6f92fe5fdf189a91760ecb9ea12b439454390c5469669dcb4500990a8d71a8befbf403b5b34e4eb5e45f5b5486303" },
                { "zh-TW", "0060708c97437706780fcb9ddd1f3a54ab32b22f3ba9226f0acb6d4d48da89762af681bc9b2f13f5a9283b790575653fe84b4ac68e7d3c6ca26d125ed5f7bbc0" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/126.0b6/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "eeca7e971eff9d2044216294001901d34d10a301439646436a17b3751657496d13ac88de02fa34f3315033466e2466584c6b16fb1b9b9269a92982f601806835" },
                { "af", "13c35d39a57a57c859da32ca3b8a8d25601250ab105d313eef3e4b002c9b9740eda118740699bf03fc3bb848139f6f6bcd2d3908332c197d7ee1a50a2cd50443" },
                { "an", "f1a7ec0c6cd8006dc3ae9d44d6c943072ff21306ddb49b4349b0b3135fe9791dab0d5263ca7e0f2c24bd65c35555e8c6c15b7f90c780c41255d2fd8c2f71a298" },
                { "ar", "b7d77b71f6f8d09c67fefd1db0d2c39dd8ed0d6fbdb4c773cb836057bb73af2cba265a3be7dafb96b64f00bb6876f95b65d34f4e7cd62c9e22a33a92dad62d99" },
                { "ast", "1652a5f8bf19983e8f7e7757434c82ba27fbe27c9e5ecc6b1ae918b116a726b223b6f6fd0a0d8f0014492d3027230bf07cd64e89b5c5a464dbcfd7f530dbcf70" },
                { "az", "896abbc30f4f2d5aa6529df89a659b4d20232711ee89ee223598a74ed53e2180bed12e5f8a7a593178176a2fc5624f5894147fb2b8fc84e749c0de396bbb8985" },
                { "be", "39bc6418d9e5d1625499420aa7812e77998f41f7cc512d6b8d38b599956d8210e227060712ca17ad5f52d2da0b0be9267f642b123d577929b51c6a8a48759291" },
                { "bg", "c4630fbf8e89c49a433359f337fddcc453ea582c45a918fd643cc23127b34ae42b9e8c8fa1b10de90ee59dd77acccebe566e190988f896547a3206df07dfa7c5" },
                { "bn", "e5da61f18d9a356e07196c75fa4e331e35349bb047ab8b66e0eba62075521012dac157c16c81b9d3a2ec261bd43059a7f2a0634240cdd711ef6240f5eb74448a" },
                { "br", "18410b7b99133269a65dc7408dd0e508c07007423560d88ca7a762dfaeb8794c590b25bcb561dc86615ceba17c6624bba0f0a84132e013a73f15d60b9f6c993e" },
                { "bs", "d716125ff11f30134dde1b14d422e4f2e827091f2523d5b7f201ae8af57c202a4f906a0543a161f123a0689cf88068332b12a5dbf53863c59687a1b64d706881" },
                { "ca", "09310ef9540e8cdb50d754ad6acc82d49f6d13ea1ad5278c43861120ea9f0a6eda61e4041159334cbe54c9aee51d43a83f66e85365f9cdb8cd47ae8e4f045ea3" },
                { "cak", "416cba1abb61dbae4ac333926c760987b94cd90dff4aa12d73f58ce0b3032e2a9c3b894456564fca0bb89a7a3ac52cf8159e98c15cec61e504f25d1c01a48d45" },
                { "cs", "eef3c328ac1bbaf49a19276e548db1e471685651c8e762cb9faa84060d8b63f3e51dcc36f813e3daa79fb3378894eb993fbbc9d882ebe5f9b43bfdfa7cbe27ef" },
                { "cy", "58cc8eeed2c41c4c96d29d7cfbd3d97cfb2add0014206e7ab75ae0fc2c46f297dbb05dd542de257b98e1baa397d3570ab1d878c57868ad28ccb4a049c4cbc772" },
                { "da", "5f3e1164ad7e20831edf1d81a3bdd1898059e7944f0ebfece0e2d3ab1ae37a1446c51f585018ff7d9b105f93dd299969232b58f2ab53a3db27b5df14dea8f7ca" },
                { "de", "410c581252b30855dc9716f71045e278ffeffbdb8a02c365c804a8472d94900aafa8a34985be55848a4860186a6a724404f549e5ef208f263eb4ed7d61b79c7b" },
                { "dsb", "8baf9baaa492b72ad63e9b9a6b74f17feca319a4355bd5e4bbb5c1f314ff53ee5fd0b85a6d11e39892cc844eafaf5f9d01044d7fb9454d75b057b587d0feda61" },
                { "el", "32dffec04575dc90dc5a20e4bfc3e272b102e5b7029090ccd798c78f0c1193fd4ada6508e2b49c93f45e7a38d371ea53aad46bd3928c652e27b29361c253c0cd" },
                { "en-CA", "fa8328ed23ecc0f527db7d8e7267e24878a567456868ac065029b097115ddb574a4a43c719c572daa839f9d9839986c4be2d5cfb11ca2cdd1ab20f37606af3d3" },
                { "en-GB", "dcc6c7b77fa0fb74ed39672c1c5a3419ce5919b2f267c73b881e4645323db9f47ee43b8c4ce6a630e870b4c31f66e726c1144cc475fb25db5f6b887a3fb77733" },
                { "en-US", "8b935921b63b2093321ffe938d3449c6c2122ae14055719960fd5a9b99a78feaab60a92583ce0cdee83fc024391279b847c0272068fc5b1ce998b195737d36a6" },
                { "eo", "db471bf81164c96b41193c59a75894efeaec179e58ae2176db868e1e47461d1178ea40679e6006c97430b5c5dfd2d61bfda0e632faf47054893cc29c3b5c918b" },
                { "es-AR", "bda3d41c32ccb2f08e96fcaaa5ed208f8d6fbbc78753fc2bd2274caf99ee77f397056f8da59e3660d3ad49c58311762f9800932c9cc2733ac7896fe8717242d9" },
                { "es-CL", "ea35c3676aee65014ff5e1d12dbfda7dfb27bf49cc6a2012905d55bd95b78f85c08eae8a2729126e6aec2b903acf70b2c073910b80db2d475245444aa9f1b76b" },
                { "es-ES", "6f655c05d716ac9d49f65705c7616bddb97a728b402d82103decfdeee80482e852da1c7ae88fb1cefcc1038d1036912c5d1f1bbefe7a30c0fa1d94d97f7e60c9" },
                { "es-MX", "ac395612828689060f1a58d1697368ef75ff89bbee2db6bb8ef6e4b36c4de0a60f98838c0d987822301b5d83e6bd549c94545b8184315dfcc27e257b60ac38ee" },
                { "et", "b0aa868a4953b221d5616a15eeb43efb81c5db4bd9f13540ccbceedb93a610bff04d7f84fd9bad718597c4fb9b1ffc086ead261d5f0dbb70547fcc910fa478cb" },
                { "eu", "651c197a4055e75ecbecf10302342561c6e8ae6fdaec3523a9f8d0d000ffeefb60667c8d5c88bbd16d6424d9def5c4510e69af02eeea868db429a9b4da9ff2a3" },
                { "fa", "5eabc56a16c1c09769047981e8ac3b8277858d417a40b81a70152dc3fa307314cae75031c833c15e1bb9a1eaa65956ba304eed1814d93188188576e2ec22669e" },
                { "ff", "b09e12c04a08e130c2057f6328c4e87df5facb1e26adde71c596a22f799ed4534f0187a093909af40b70ef025feabceded21e02fa137e81c815bddac4e65b70f" },
                { "fi", "c02fc0b6657bee56bf07a57bbc7d1ce1a96ffe027be0a1b7c042e31c3f68d137050424bf9bdf42dea2d26f2db6e5acdabc4475d725a880000c5bf22457f6fa6e" },
                { "fr", "203f883e7109591984435798d82922b5d590b638f3adede09a30a5c153de268d5625c9dab28407ec157d2f5ad1daeff676a6dd7396d1982fbe6e52a634d60ca5" },
                { "fur", "f24653a4aca05e9858ad420bdd6e55c25cf9280586c706c1d8aa6888ea4c1b22204a41dee736c24dc8a3ac965408b11ae3c7416641b4684e156fbe652e18f568" },
                { "fy-NL", "729243d139c5dc7ca46b468c574530a6332b05bc989cbbe672ae996655a72a89ccb004be1416e8377cc956de7ad9777dba36ea05ab4e8fc7272bd2394e57648d" },
                { "ga-IE", "60a13ea7ad5e80cf720fc0590ee4170ee7d02b5faba90cf7b30906842ca93f36d4edf4efc4760ce4bb579a5b8845e44e551b8e9892a77496827be2250cf7b4fe" },
                { "gd", "2f8bf959d48db897c1d43f2708369b80c53787657875f4eba4f3939b92976e14111e87902f8a62563c1f0a47e5f0545243bfbf1f8848b5c6e70fcac09ccabb30" },
                { "gl", "0138cb928774da4d932ff5eeb639a25ea158b67fc45604608c6ae5fab17d75534a76d1d05b2fecde10607b7274403844181f665851ee2dd5f30a236d4f8a962c" },
                { "gn", "43ca6078a2c163448edf644bf93532e011be3daa0a4a659047f026c44e42fbb2de0bd40ee5e2b9c9a58b1da2a422924ca4f9832b9a92349c815837b12b40a613" },
                { "gu-IN", "a4b48dc5b15901756dbb71558ec59edf75596efaf8774cf65437f7d01ec57b93cb8973cb30e8e806a002653a4f7855679b53c4039ec7f1eb2fc1709afec630ea" },
                { "he", "b6434777c753d1936a6ec94dc83a6840008a7e30850b7bb52b6200a4ad692ad578e85ea3519d92f92c41910d900da4e90722670ddbf25eb99a90af8820f5fc79" },
                { "hi-IN", "467d1bf31da51c1ef60c82fe7c8c63657ec2bb33414f00a58b1d4b0e445c273edd2b5eeff32193cb4d40caa4292a10abdb56762513ca58cc21a2be357d83abb1" },
                { "hr", "cfee43693eab8b7f0164bc1b025b0ba374f557a8d292145af67996d3144be459264c192c4ae7a9a0ea1516a1e9f3d620fb3444a8286437abcf5ed0c581bd2295" },
                { "hsb", "ea8865cf63d8214f414ed421e8df789593d8ab61d427d719833dd5283cd33a9f35bb382c533b33244a40e42379c692eb52ced959cfda6347846ebb79900b9e2c" },
                { "hu", "e724d14ffecbd720358f3fc29c86ed2c3cc3148af22793087970a35eaf2f9008221a23b579f2189bef8c1a52981c2bc44e956174118a1d2d3816d32315f9f71a" },
                { "hy-AM", "f7384a48a5acdbb8566aca8285cdd9b5b41d651c268c3660c7ab716c36ccc85f9ec0c2c8dbcef2bc0948bac54bbc973bbf520040434efe2044c52a11c3829b1e" },
                { "ia", "36cb1ca407d6c0f06641b9f5da56f9137a09edf26721501ac0358a9e2068f718e759021510c3a99babd244d3bcc148732c0b873d6da17d9be80b27b6c444e232" },
                { "id", "546540ad686ddbaf30da0419670e718f13083a500a3c617b45091b53d78d73a607b5987cf3b9b2f3ba4e7b0a723e365ce4cdec4e1c4350d379587af6dc677540" },
                { "is", "daf287a8c6417ed9c697230e5d6ca3201a6912576060f397963bd449a3517071019fd6766058cd1d3e9de1868fcadce215bc8016aaad06a09fbd509e2f7446bb" },
                { "it", "fe2cbd7dac92eea0c4dd92552ff04ead8b7e54f55dc2621a93d4d73a13a84fcd98a9911471369ff427d9b78bdcbe1b8a86bc837c957ac14b71f6e26a71115ea8" },
                { "ja", "d330a359e0dd14c70092af7fad953b0089aea27cf97465a908845fc743aac8ac76c44244e82a179b546343c845cfc85a31f1391176d8625f160186ac1b6cb87b" },
                { "ka", "44294a1d1a4dbbe13152595eccba624ad47ebc750ed8582dc0cf585a3e8d551aa8f30d00ae3c5172f975f1d8c2b32b20cd4535b52320e23ffa284558b57da488" },
                { "kab", "4b57676be426dc3a4cb5e4afc6354b7dd2d519d12808a013488c1d3f87c28daec9f52f6f1f92289a4cc02a1586266275317bbb6a6e8708eb5341b1dba000e73f" },
                { "kk", "8738794a1f7e4d6e757b5ce7f277bac666239cbd65943889c0f6ea9e235dae68a5cd8b4a934772384923ed20b0d600074e2d02f0790706733f323323dffdb472" },
                { "km", "33f7b89cdd47410bc956e05f1e4fa6ac6be06f2b4a26fdddf7c71cc0f82bcdf7cb6b3c48458d6e46eb5f7abd129e2404654aa0112838aa9fac5c6e59ff8d3f8c" },
                { "kn", "5b58194f8ddc3a53a7473c79d64ab14d4da1e62db1a34bfc313a8a11b132c63667e45e6ff742a309164aafb7da31b4244ed4f59b07eff0b10b056e8da922fb44" },
                { "ko", "56f38e9fd10af680741b7d1f78c75b986981b5c3c41b4e6c011aa17c517b84bce700ee5adcccbcb97c3f94fdb476fb589528c6ec9e52968b4f043b5274a56189" },
                { "lij", "d6403d5bca416412aa1c40f2337dd489bcff411c4a16a5e594a0b7988f746167d72da3376b18a84bf82e979a337cf1046045cb585ee9dfd06b712d443b4a1851" },
                { "lt", "579d3da53beb3edeaf7bf3268b3769534a48501a966cb211bb1c05c8dc79b06e467ea1526c6bd551fadf28e16998ae1c5f5e0be075cc7938f88f46d9cc1b2f38" },
                { "lv", "62f84da0ec306c0b60c5a22bf7c86cf9e2973088c71f052d40d7cc6ca2c7edea5dea98d34d03eda40fa8a47c9c80adbe45648dfc2338b2b7dfacc304c4141547" },
                { "mk", "e072396e2417d2b80c250e7fc422c922b8b4a08d35dae2ab0d9808758565da02d493291ff3436952ab08a94bc60e53eefba54aabfeebe468ba1c65554fced16e" },
                { "mr", "502c9361393fd9e8b44d930929410e30dd5c766c675c60cce0f0b7c3192c7e809ff5462f7ab219fd096974d91d95326c4b813ddd8c63772e428287a6dea443d0" },
                { "ms", "94e5ce972959db81f67b30b33180f31d3eb63748c5432d9c450ca4d6608f390a50948d9a21c15f158b281ccf0613be90761a278996374b63fff3b5d83d118a13" },
                { "my", "8969c6f459a021c84735f2e6003b2eb242437d0adfe2ba598b1c422ef9cc7024490a87ecad76e03e1bd9cf4d0b1c1550164a6dbd294d7da731e387f2a2e74130" },
                { "nb-NO", "efd580a53be6c874c1bdb22d1110e767bb58ef0c83ebdb578b31944383a52e54d6c256c5dd942ff380f7bb4f726e7acdead8cf7d1a2a0c85faaa6d1cbb8b3fee" },
                { "ne-NP", "821c3b6038c2e28ff98faf39fb52dd0a5e1d9bcf7f46782448db4c6d2b72957d1bcb5de09f1840907d805c1372737c8eca1745dfb5032c7b5c96386958444c3a" },
                { "nl", "43276fc795c2c31ce9403e6b7e93cc6d40e1042e5fef3744956c3464a61350eb23d7d1aa6234a9be409d6ff391400472e42e63f0ab0862309369b082b82f2f96" },
                { "nn-NO", "57e5e0b3ab6ed44c241af113f793992ac7c2c1e814def919c7c55decbb81cb785d7f2e6695d8e8ad4fceed1d210078767f0d83e21213bc1e7fe891815f560b6f" },
                { "oc", "729742e4ac8e4321353c3b471062d89bc95bf51a55f9e588600a858da103c552675089604242f77b1fbcaf8fb3f6b47d01d0ee585b9158b6e72dc74648a493b5" },
                { "pa-IN", "981d3815df62f855ceec6cd0a47e53d304b721925a5879b1f0f5f9afa41fcfbfbfaae15cb81e650b86056ed155eb2d24a9b4507312adfbf70cbd144a7c8ffd41" },
                { "pl", "52b9ac1fc2fb9a73cd287970c8637dfbd11ce64322df6c05018f6c82882c1020bae7affdebbc79feaa2fd0a022024edfb0339faec07783554ea2da27c4fe175c" },
                { "pt-BR", "6f44194cedc37af9a6a6ff0767adf8d854b5d6e7697ca3d1375ebf969b45a1b574b928b6b54a3097d2fffc0bdb8ee70ef5b0c66929c6b0fad631685a80ab6711" },
                { "pt-PT", "8d9ad9ce0ce2135b1df16e90c6c58e2036421a8e6513ebbd2d86e1d9751dbd967823a648d610917a6d84526b4212ea1e992d9b7698d6f1a3e50589a493e7dbc3" },
                { "rm", "0f02b93699318a24f3a4970572c5a7df501acf5ee831c853caa35616fe95b0aa1cb12175dde6ec195efa65cd576a1bf258ddbeebb8bf047b900bfb104801652e" },
                { "ro", "da705171052781e4630d8a9bc4cc0eeb12f19536fb7a57e5303de656edcf609c5d46b7af968ba1f8003ba39d244b45807f24ef47e34155cc1cdc110a2ea5d8f4" },
                { "ru", "352ba773b4de01ffdb42641409f420b110e5c23565a4e55388d10e9ab72289df413f1a8bb7b9c1ff4242de2200fa1298576bbde5c91b131dd08f6f20059e30f2" },
                { "sat", "6fb450a5622d1885dc3f8b0f04185a83a58e53b6ffa0039e95e216703b0196b24c986919c676c69165ddc698bd0cb961fe7248f518749c8171b697c17d558645" },
                { "sc", "7def24ee52278532a8715623d2773c27c312f49946fbbce4d4faa61099e5c79d123937280b9055046df2e52e27bc63a8370e17c60f15653089316626b99021fe" },
                { "sco", "5e85e791a24587998bb9c3c14362a34cdc56f138defa0733bad9d8a2120f338256d96daa1b493cc3d0203e835bddab8414738dd4429631ed8f76ccb121316e48" },
                { "si", "50fb6fd61982987fc0b31339be2e005c56b0acd7dfba595cce4d162eba7eafe22c868a9aff2cf8cfdd741de8934bf2c00e1579eb9787e55263c17895c388ba59" },
                { "sk", "3947f006a4a2de11d43ffb8d3f118b2b0e26610f2d755addce0aef70d872083935babe84a549c01e608a765a678c3e84ac9b3bf1317c485ac45b5147c2abf960" },
                { "sl", "10415bbffe73eb0778a3f47cf58ff22a1d62211efd5ce6e3f3280399e4622cdb0fafa1cd551a4b91cef805c45fae37b032ddfc08264854818163464261bdf120" },
                { "son", "944644fefb4a114c6ed46e95457f0947aae84b210265646e22ce72d2fad93df560ebf6dc21edaf39973e501677150a3cfa6f2b6aa6a26c522510cfc163e75844" },
                { "sq", "df3950026c03dfcbf544db0789fd5cd0a1e589aa97920e53cb98aad06d48716ee659aee0b2cd97ea96642a0d8c5eb2adf1e04ca2e6754f8b9a6a44de9e64c618" },
                { "sr", "633058b63feaa5d860d6be30413601604784af13c681f76d379b12d238e3d4c938931f756cbe1b6fb64553551fdf79961e8aa6372af1b6f983be64408a7f4ec3" },
                { "sv-SE", "f65e1effd6a5f42385afbee299e664765a9d207bfcf886d7d797ccbd389bab47cf9a5c954e3b97138af0f606a9071c61b564fb2df1b315c4f922716b4c0c7559" },
                { "szl", "5911644a66b675976d16ff65b799d215df5b25f1b5d8a2d6fd87493cd58218c26213b15c3eb02837b9b2fd214d43fc2234a3f12cefe29cff4e0bbe6ad47030ac" },
                { "ta", "1dcc807f5b392e43195ed656fa8033051655aac70e732139cb45ec2d40b3dab6d44302c87a3a8b1d727f3c87430bde6bf77b659e1974478304f372e95db9c6cd" },
                { "te", "2e2ed5b0e92bfc7b99f9f8ba61839d6a6068e971252d27922cc12562270bcd1a40bf7fd34c675178ee4dcb470c742af443d76d01702c0854645e4ea0f592959c" },
                { "tg", "774a65066c6e1049a67e08fc8821bd811526c1d95fcd11d62eaedc9d3b739eba1af6e183e125be5e360ca31505e8e450e65a363fe7353c0b3f6ed470bfdfdb48" },
                { "th", "a166c4f36a5195941d73e055972d1ce54736e5a4269d37d71beec49f8a2a6794feb95158ed4ed242529c46789ed33dca7f53259b1eda8f227a00edc9dab2a91e" },
                { "tl", "5c76001d20fc198ff7be425f8a2912f00614ebb39f9a7ba53b2280acbcc760464859270b3cffc8b3905cbbf2f07157feeecaa715aab18dc3b84637a2552660ea" },
                { "tr", "ec65fee6a8bc83a15a72310e096bd6751465bd390b5f69a0954cad190a28e8d74cbb3b24b8616e168f150cfa2a34bef6aa2e808b7d87ce9ec96311562b2da1e4" },
                { "trs", "ca6205d5ec83765080b887145d6ae36090d5a0b1c35ab6ba758e52850f8782105e7a8abd4c0fbedcdec1dc94475b7df0c3adff408be9e14cfd4044f02b7452f9" },
                { "uk", "9fa6ff6e60769633bc596485d53d471413cebf2aa899e17d23ca32f8b872b5fb4ff3e311ff5d2651d19ddd0b658f23889a5478e4675a797fae9d6684bfd502ca" },
                { "ur", "4e0156aba0fd2599901cf537b2428d61982ed399e329b1852d7343cdf033fca2867e68031fbc4eef8daa5dd4ed88b60ed2bc1c7d3286589030cc7532dcc59c0b" },
                { "uz", "a793502910848f45f4b22e191c5099019fbfa683aa6a45231f4f0f627d3849bf8bb695b997184e6cf50793986fb5e33cfca8f2177595a6fd7244d509bd884889" },
                { "vi", "3070e8c65d3033c6cfab15b3202bd68870fe210434499f3858c9bf156f93e92966a4890fc455cbf31e14c439aa6606e28aec5b830e7cb5a3a7a7c522ca4b6dca" },
                { "xh", "8eb6f7c00456673617feb1b49c884dde62c6598f92c3f63db05995be8b1ca2723ededf98f23c0fde2c552ff6611a32485b8bb6053d3ef28a04441ea9be971eb7" },
                { "zh-CN", "90130ea6135fc8dd1545cf77b039b52291a3c1afd41ebb5b4eaab6ae564a5916a16cbef9580522084e0eae2efbde2a35e501f2aa7d40ada00607a8460a75d541" },
                { "zh-TW", "dcdeceac63a32bfd2b7ff2868ffcfd12bd6966b44de72fb914fb914e92313f716387e070cd8c4e46e10af8b6672a60acc0f40f49de8d231a6e3994ea5080de42" }
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
