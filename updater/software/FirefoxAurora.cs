﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022, 2023  Dirk Stolle

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
        private const string currentVersion = "112.0b4";

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
            // https://ftp.mozilla.org/pub/devedition/releases/112.0b4/SHA512SUMS
            return new Dictionary<string, string>(99)
            {
                { "ach", "5c81ac766da0d5560fab1a712ed1dbe83a99d1dd02431fc9063948b64f63a54fbabe1b73d15deef0838246394e97e58ad25e4ab9b6ed2bb563c875a39c9bfcd6" },
                { "af", "e5c4ba592696254e806832e90d554e4499cb753d313a624fbc7ddaec0b68fcea7baa3383723629da8a49b1381e4e11c0c819ce573d680734cf2db2b2c9b557d0" },
                { "an", "2997d3d69b809e6f2478ca19815b24aa8623d3a9588917e8b476f9b00e2bfecb58cd4c2e24116388c04fa18a580feae1eed9b190e8d61af11eafe4d1b9296688" },
                { "ar", "08f08f87d8d7964feb27deda78d12eb844bf48cb864e71e29632745ea1391a8d160aca41185df679fc2f4fce7d216b73db54f68a771b8bd9c88aa4d1e3156306" },
                { "ast", "676d60538485203ee13faee70df5bc7854d79b8bc6f6633c26477c9b11ed8246aa0f03205abce0443b4e9a8e26b8eb4359a5e02a8cf81b243597e6b2da45853d" },
                { "az", "d42ad6b88ce6110c9356ca1066a9ac266585f8581cf2135bfd23e4a98e1c2735d06fa44d8922ab8e6517c9028bd14ce6f6feb9f8f63ae9522e63da413e7c807a" },
                { "be", "065cc9a8c9da2113363e5fd0e2e4d6c256aa9ae94f051895aa364429c6aea1a336810b938303453832d6f733f3c4b3c5bb6d70d110133e64048a1cb3eb79e61e" },
                { "bg", "42df85740f253d5472da041a21df022ecfbffda057c1f5b59621923ee03fea51aaf58da295cf02df90dc9009a331f00b6074ac1c4955bc9aa87079724f03e436" },
                { "bn", "5bff78493d2eb09e4bbbfdc0f6eb95174284f78189b4d4c63f2e0152b3700c1ae0b093feeb70a6fa2891cb612104e738e9c31bc475a6475c79db57d6b4dac8d2" },
                { "br", "d728bbd8e305ecfc9ae26cc729a952d39c1f146630ca8bf588583d2450612816b55fef2f4bc8777d2c44aa869036732d13e674e0275d0c8e5220bc78bd6c96a0" },
                { "bs", "84eec19fda320b936b92fe336cb774841a74f0c5844c843bc6d3d71bd3ba65177cd66300ba2248393026dd2aed1fc2feda3eb220a1cad0b1c839e729add265aa" },
                { "ca", "b3e06e9d68adcef5dc5aa4f7186ca7db86c62b79c55c808ff547e5c4cf02e8d622df19fa79070b2592fdc15ab37b45f7a2f0d04fc20591adaca7f660d3d86e5c" },
                { "cak", "d88f7afb49ade119e0bbdb8d9b5548e771e3149918c1c6f47701090ef3b28d5552295c715e6a3b5266ce9135fbb50536bced4d160d18558411e24dae421a5e5e" },
                { "cs", "4740bc441f63609f17f053dce7c81e039ec89324b676c4e08622730372c8edb8dc6b335d6102f483f4e8f791877f46b85eee4a31413b648c99cba82be1d2af54" },
                { "cy", "9ca063fc239431691ada33a05ff8d755179d62eb112b0e184713811868b944f309c8d277a3ec27e907201ef36bcb645b3efa5d9f01b4ff9050de3c523b7c65b1" },
                { "da", "cb73b9ded4d25b6b9691f3febfbbc2e8f1b1c3363c13401997a097a40918618c35ae74c870b7b2296ea063cec76208e8254665383bda501980da20300621e4f1" },
                { "de", "caeea26e0f7c06605c2268b3eeb1ef6ad19cdb3426666cb1c29a90ec743c69fb5366d618d5a38dd6eb6da376b6400e2954de2596b7bc43bc91e5d0e48a43b1f8" },
                { "dsb", "0404dfbd6e25c14f26eb284f504a3207ad095c114b6e03a862f689e7ac598317783c9ac6e0f31ac65b89f7521d6a8d07612e739014167983e54c844db7196d80" },
                { "el", "19d1c9c0f688824b9024d411cb5e0d627a320543781f2156ca99b33dc058e1286dc8a65128666d4374984f55a3ffc4372a2f3f3c67fc33a7239ceb4b5b27e9af" },
                { "en-CA", "b8826b36b4cdc9e11eced985cc135c80f4cd0789a5cc7e67cd39a9855049cb2050f32e0af73300d8c98f2821e8915a73ca37c6c4ef08b287a0ec6fbc06ea2dd3" },
                { "en-GB", "ee621bc57b819980a974b12a501eb0014ac4c0ebce10e066f4c2a5f03bc12481af96d500e6b4fe678e1200da4241230cb27ed45b5ee3532c41b2a65fe34c35fc" },
                { "en-US", "da4b6e25e46cea2157fc851b91a23526b1d162cecf5156db180d2b16ccde645cd7ea8f9f9364be8c14d1ba8b69363b06b178306d80fb4da249f32b526ae22064" },
                { "eo", "53f0ecb0b3bbc3062ecc395148501f529600d45918ddf777330a63d8350207638efc90f83d2ad51c7b3523d2324368ae32caad29f0b2055166976384d3741c95" },
                { "es-AR", "c899a7adeeee58ca6b4ca0b2d65d15771f1a1bc2bf15c00d2f216c82ec7886caeb7f2e9f799e8a1efeabd80a5cae8549f106b25c0386c86b657f60d98ccc7fb7" },
                { "es-CL", "81e4ccd4d277d4306328fac6016e22e58630df890cde31b31fedd06c71b2f35ecb8dd3ac0d5322ac3571fec7914c60a45e93c9de23d5386821c78f07eb0a6c88" },
                { "es-ES", "764446d5ffbb8c5065918c900019a088ebd9376fd107b1073e10101675bdc7c8524f4e8ed07ea3922e7302281a2ad254373d3048d8a71c1c42457ecbeee52470" },
                { "es-MX", "5d55c576ec3666b983e34104c92e0b546185e71a6fa7afd013834548ff1e319f59696daca4f1f36f4e550299a3e37e7710b4c28921ea1e2d55e0eb0992d57997" },
                { "et", "b3e673eb96890cd15446855b3757fa8de286541c9fa74a6274668b75f74e42e17dec8e13a352be7fa974a729ccbdb1358f162f8b83963ea4c306220b96deece9" },
                { "eu", "56112fe1bc7708452dcb50a69938dc26df7916f01e581cc32b6eec28c57413fac4f838545c9565b96d0be72bada2a92542a3a069302795b462fa979a96ef51ba" },
                { "fa", "12d4fb3fd924d8cc1baec419e04d0a589c5a48aa79644af4eec311580685457efe868af06c786f9c71cc6e1c3cbfd9ea527db48500d7eb2e23266061e6184b4a" },
                { "ff", "15c57bc68aae635c9ab8a287f48ec26db88b2fd4ae6b017d77162b280a4023ab97d34172484db529948fd1717ea2377768bb475b90fe007121362aa276dfebb1" },
                { "fi", "92dbe190e28e18e40b45ecb5452aa5670a8f22e613b475fbc32a8309efdfd6f2c3fe9aafc16aae256a10471cd5fed568aa2fd58bc91faea35c7359cba98b8b9b" },
                { "fr", "a99b2e73946a470a38ada7b999cf43915b10994fe8194fac05e75607f81fa1fb472b498ccb5b34562605b5655e6a423d55672d5bdad7ec364e7b84540508a47f" },
                { "fur", "7468ff891b5f3f73106a09f5e8853e688ea5f5e4487bebd681e1b1ce18d2e94be547b5296c29dd8afb4b1e25021042030dfadb0dcb9a9d143432d5cb66a98d75" },
                { "fy-NL", "75db9bb5914cd544d37d59314c126018c8c141255b726fa292395cc76057979b83a5516ef8b00734d94de83b6dbbe644a21b845c2592807f804bf46057ed6460" },
                { "ga-IE", "d9eb9e6ac2ff6f00cf9a4c7e7a1e49bd093d643155951a56e5c0a00553e6f087de70a91f5415205755adefa3ffdf01e59358848aa376b6f91c4f84bee9d70ead" },
                { "gd", "7f784156bcd48c89301a79e55f04815a01119306e159b9f74afbd1b024bba1313896f6af8ffbdfe71c0ae9650d16cdc6792124b0c64a333b9cc3175d4c005486" },
                { "gl", "cfabd94077420e03e4fffc8139bfbbd99776d6c26f3e95c64f67219b9709f98ff75588e061dfa106c0eb011da10f4e778f121e11eb8a99736a80c6ac68b854c7" },
                { "gn", "2b9e2ffec27812ca5a49b1f80ba6586d5a5c7b5c4961bd6e822259c878d50e267c1a3058767fdc7bc9c41e9ea4d3837ac6da883511f2f07e7133a4fe4e5f5501" },
                { "gu-IN", "e44489e2a6532f193f642c4f8de7a24343cbe6df1571dd84c0a8ad52b1bc0a6d3215743d89afaa7fcfdce0ef8f5c5e7cdcb6a0d4d2c8b27c2d488f268437df85" },
                { "he", "a8da32af6e1331cda6146f3bead05ba37d0bfa8a92b71bd037231bf9e2685c2e41a28e8ef475fc56d26c762aedb989b47885562f6b4c90d0115207473dc15a45" },
                { "hi-IN", "81f7a8c84eedccecff03774eb5557c67b327f70378fbe5a421eec58107854583c1359d6be3b2a1fee3917eb86cd05e9b2666b68d569521a4055f9a1b186fb018" },
                { "hr", "44dfca624e4108c6e6d4fe89a836823a0eea3cf6bf41264af436f7ba0be68aee96ee38aa4b1a0e7e46028ff9d1a1e271e1530aa8f7a88d7ee5c4904b649721fb" },
                { "hsb", "0466a6722d861c11b007157185ae685bf46e37aa97b46a2f2ad74b029cb713b6bb5f151847f4014678e533776b8fe7e46490300bacd8ee661431b3f258bc7006" },
                { "hu", "8ecf1b3a7f6c31c182d22e80fd9981f2d2f09a6ff5f6714ff2ce5c57b0eaeda317b7ec33267ab394292048bde1d4b357987973999dc7d897a64779f576068489" },
                { "hy-AM", "6def6dacc4d0fa0e7041946fdad3d94ca5e4e4d1101dc4fe0da68cfe8883facd1cfebee8043ac36d569847b58248cdb7aec01d26423a918e6d370c753b1120de" },
                { "ia", "e4a9bf81ddae4fe46b5de40ee3e9079ed002b3372c4ad8fae0df86c7f4b68a4fa280944619b91513e724ae238df818ffe66d3a670e2fb470bb6dbc2cac2d0523" },
                { "id", "b0802df4512045a24a59ffddfa398634494e351877ba0fd5cddc97ea31ba63c10bdb1766d71380b96fa553a6bbdbe78e8552dfa43f50527514dd2b5673c8138f" },
                { "is", "8a0bf77fa72ce75d2dce7e8976a6b7ad7a6c4cbcc2174c2483376da071431cc51d7d11b9700518904c0181f6f18b38a7fb54a702a80278b6c7dc263044e4cf48" },
                { "it", "bedd7b2f94260fe812d5bd957247600a853ab18ade2c7d3feb8b8adca4acd22a48a71277bf13b2f64771e14f3b245e8a5074eb2fba76cbd0b8ef1885d5a7dd25" },
                { "ja", "8e6a2ffa25a1c747054deb0b9798e2046b618f007b292f75724834b89e1e865083ec588ecba104778e3a2178568412346d7ba39c60866402aab0232e46db366d" },
                { "ka", "0848942df86ff779c1a0414fb4875521a6ea99e3b3864104cb9fbd3092deb00d27584c06c954dba7a9bde0425997c98592e46804bb76c809d7e5e292b2c689db" },
                { "kab", "28b075afeb9de43b266ad7b6405192a7bc2bfd8b09210209e26c6a3bebd67b0d8fc8cbcfe61052f3257943515639fb4126a429313e270fe26a5b5129c179f38d" },
                { "kk", "a145adcead32b826f05c509a0421bf562674cee8c44e4d3db87a639a73b76515d77ae3e18f5c2fc36341ebaaab77bcb4ae946ef836623cd88c81a3eba15bc823" },
                { "km", "8a785a63df2028679a343c0fb67e721e9d0a2f10ecc4d10f0339017e10fdd2daced032bb9e80409d41a556d5749d0c71fef899ac6686a6c095e4361ba6f86007" },
                { "kn", "afeb5d76b7a3810e7b7f34bd635b060d928d04c5faedf1b4120297917219d962ef36bdf6ea5bdd8a39a2b0d8af047c63513d057cb9e6e994d440f786b20b4bd2" },
                { "ko", "04540ca19b176ed964411021d35ac8b8ff388693bce5f33ada3b486f467eddef74557abfdc6bf10963954e18b45939729206de0408061a9fcb2d8cbf0686d92b" },
                { "lij", "08b6089c8b37f6bbc8468d36545a5cd5ed07179df22341a2a388f0a88c82a9b6b5a1ee9880bb001d79f730e97605ceff0ad2bbee3bcd20109f3cbfca0c8d3cbf" },
                { "lt", "d97e2b4549b28ceb18cdec07d69b48baae75f957cf06423a746d9de480a1ab3803be3e692032cee43181d2e1000a1969c6b8473098c853be5184343f80562c9b" },
                { "lv", "443016e5bba1de1247d398165b9817957792daf2d58922c1c7f9c2f4239349d0806c8b1ebc18da43b8b55037018472ddffbc0d685093f10d9ad12728c5c8413b" },
                { "mk", "264af02800067610db61f58b34b8fbee4dde61fa282e9e15155fce6714e1fca3dc713258a386d2fd1bcc785aec167ad364dc22ffc63dbf757f3b7e7ee69fc934" },
                { "mr", "abdddf2b46a80be06b4b1dfb485eb3161088a9e7778a3ba2f3d41c5b23c4b1854f4e9efcfffac1471369f3b39db460f1bcd8e36ccdbca68a5121d5284b3995ea" },
                { "ms", "df91f8937a140ca4a2dedaab788457cc0a71e2be83745e6400157d7c14e7b6c0bf3d3d1c7e91402aeb06140ef092c91696baa178ca17b0a69e7a141d63ed2f74" },
                { "my", "e66f1548f10bbef5bb1bbd2a674e007cc18ff145f51c225f738d4260d2a7f5a0c92748010a45d1f50a64bb7577a2ed5946aee6f17241c362138d5dc2c064f768" },
                { "nb-NO", "67827d522cb82b70466651590c014a35be02199b075bde42810e2294b633c4a80ee1ef9081bbfddf0a595f6e4192ea5122ef1b3cdc4acaccaf78252657832679" },
                { "ne-NP", "76833494e4a15e83d05260ffecbce0c93327e9d0b1478af044f91e34bcb4a1b047bec86107977bed463516a2b6b35a8b85ae0124d85d3f6939658051a8ea3e1d" },
                { "nl", "80dd56ecfce5b886e9ed2051d213a65f2515a2dac3e0898fd002e43fa8c6f3260e7bc2accb945b0deeeeb237374493fdc9d6b5535e03178bafbab9b9cfd24f3f" },
                { "nn-NO", "8674b2cb139cc6c7e15af8858396bc3102e884074d25b53c64b0bd2e2db50b5d0edbe52019d073664b987fb75cf0f2af64a59181e95a2bfbf332dfc292ff73b8" },
                { "oc", "04d66e8f1b93df43df7f3eb13600b655ed96fa8cd7c00f092019c2798bb15d05c01f6a35014006484ed6ce4b2c7423eac40727a8a4ccc887240c1fa032754201" },
                { "pa-IN", "79255c3d0e073c345fce8534ac7a57578c7c3967a215fd394929fd50b0e54def4db7c21aecbd111b7560c8fe01d4f471c55e20680e50d0435ac44f439deee841" },
                { "pl", "db01d52c33396d8ff93ad6e7da50d2aebb9b54fd2185b5f1d367bcd77932d6deaec7a4dea4cd02990d7e74e34e41d1ccfb8b535354c48dfadd05f9f7bd3bbf2f" },
                { "pt-BR", "1f04ca2646d0fa5f434c90918368e0d14684d6519879778d491be3f14f1fa391e22de4d2559fd5b8d9b92913c7924d97594733b8c03d630082f0b660b8761e14" },
                { "pt-PT", "4e23c1af5e1bf8fc4771028de8c1d8d768e910f36ea0f1f09670494fb24501618ea221b442049a96da7a1662ce03bd2f8cf77e6190ddf96913dcb33850726843" },
                { "rm", "997b532ea8b20c8e81fc51037de94b04602ed44b5e7d04e385b57b1cb349293aaa104f14eca3339d120206f5b8118073db07449106481c54e0254fb63719857c" },
                { "ro", "c0e234e11ba6ee479d90f06d9b207d694e1e9705d0c8b454ec85e2b782f38d1fb893fe13ae564356a1ac388e5ae29d3023f66830c11014f2ee708e01c24b6c0b" },
                { "ru", "ef8f375c6e1aaa522e0505bdbbb75af96300d3aeb5935154075b9344155eae04defd9fbf1a871d29f996b1707160d587b27a75e68ca60848b28c1c4670efb0a0" },
                { "sc", "a80bad6071a945fc9114426122d91184ea4b6162290cf6f6428c6ace29bc83fc61d5d67ad0a7f2dcb0efe3772c0a1d23e586f6d399a3b2247f5457afda6893d2" },
                { "sco", "06d7f87163d593443c051957ca5be08492fd6b81eb36f880a3a668626e103d39e299a25ed61c4407a4b891777c4da1c3257e699932b4fc8ccc567ac8fef48d94" },
                { "si", "92097ee9f115276120e75ea13eef4b9d8c70a5fc424a221f9c93161738ed406d73405f9cdab77086bfac185c9abd6169956ef8ef30ecdc1deebc8bd5bb87e342" },
                { "sk", "436edee9994a198e6dcadecdb57ef03e1031535cd88962a5c9cac8fcc97e81feb40457a529d914d77e0ef0a5385b55e8d2d69dfc01f220f566239357238efdf9" },
                { "sl", "fd59dcee8e1dd939e8b7b4008570b1355251c41c0bd59b945af401a3ee367cf75e423f002c21273dc87ebc6ce23d171f81213773284896cbef50637912cc62f9" },
                { "son", "11d86cb94036ea4b0539070a2424df830b41fc2769fc400ce29c50a37c4d10e23c3c6b434fb4b2bf1bf8bde9ef86f11e907d6ee97e1d2a21f6a0d0412fd52e21" },
                { "sq", "46cb36a7e4633654876f52063f69dd01f7ee0ab017636ad1b0af2037ecb6ccdc781096c78250e729bca58fc17b895467bceea18dcc20e11ca4235b38b1e08762" },
                { "sr", "d9d75646a7b4306a36909ecb02ad0a40127fc45a95d6ccd94ce8da1a0a4a18f0741bd2396d9d7aa9154c14c68b8a5cbf344284eaccbf0ddf26f48302169f0fd6" },
                { "sv-SE", "1cceaab7c38cf43a279175b467f9b91e00160925a709527b7d988033245c9551ee55b7e1a0c3fc5523680e5c148d6d23ecf52ef50b608dbd327efeeefe055ea3" },
                { "szl", "c8411e14de56538a9c2c1b0c06095aede770e6ce11724c70f1608085281d053725614c7e284d58eed45817924fc62442f5fc291b3fa8150b47954f0f74d7be5d" },
                { "ta", "578160467c1bcf1c38f92fc94bf9ec8cd674c6ab4af26e32947b908323388a92188c0aaac06cfaf9b94a3c365b9ad506438b3dc4a58882bf1c1f8dc04fce03ab" },
                { "te", "c7a3da49273a49f047ad099c8f95986d6abe6589bbfa502f2b66639582fa89a93194d864c5e73a74b7902d8a2199147df4d24f5e209f510b4a8a7d5e870e8af6" },
                { "th", "0c1855ab193c11f4c532ae692200b5250ad1d842d651c0c54795da28515e3d246152827bdd0d82145a664b56ec481b9e7cded25016f21b301aeb529cf31655de" },
                { "tl", "802ab2c4685cf92f679c16bbf492347a863ce7ec9f47b288bdeba8c22fbc2fac8a43e8544aad8b80c50d95041de9bc6a9d73acbd7a587e0598858b2e3100a9fe" },
                { "tr", "c07f3f175d126498c8cea1cc39e80484a65528afe17fc5027d4db1b880fb4a84cf07e3eb78a84519a366cd2cea6696c553b15fbb7b9edab3ed54c9373cc2fd8a" },
                { "trs", "aaebc743981b0480182491c6d164cdfa48b32cae256a8e7d79e9b31883991553763d63bdbfaed44d66731bed8434b7ef579739bf4ddbb95ddc72141d99e47981" },
                { "uk", "568f47bc9eebee8522e6efd7d9fb5381fedfa308aff1588719874926448a9e2287e98fc2d911e2280371c99cc9063deb5dbd29344bf87368ba86c663cab7b5a6" },
                { "ur", "ed74c6c45d49b1a8cfca0bb630ef890924ab13e18ff70df99895f025247bf4b8a99ccac7594579b8f0772dd38429eec62689a545e7601bb493d8ac1a73423fcf" },
                { "uz", "2e5932a11a8d17f4d9bab062691a279e88e255a1b94189cf750875d10909f5b26c9060d1f0fd224e263bb03cfdd383acd15f6683843522cd1dcd9393ab3b7165" },
                { "vi", "b0d8106740c9374f977214caa3654a0540b9a1b6349953642b02f1db6a849e210a879d41d1e33e580be10dfcf7869b7fe0f7bd24f163986f34225d8fd6df5970" },
                { "xh", "85ebb5477effbc6f540ef313c4287b77b9b824329b71e2b7db3a676ab7758bc3348afe492e0d535aa5c5744825f769eb561435ff35cd818e9ea802873446e467" },
                { "zh-CN", "9fbaac019bdfc415bd8faae14900c55bddd78effbcfca2fe6b06db516585822e40aeb7672c466916fef8f1425f60142cca962e047cd56289bf6827240c4382c7" },
                { "zh-TW", "4a8232c8a92592d3764e0b3221dc237cd191db93d373bf7211638567432acf3c486cdd42a25505a0b355f158aafb1d74b20906641411b9cec201b6d4f574157e" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/112.0b4/SHA512SUMS
            return new Dictionary<string, string>(99)
            {
                { "ach", "f79a1ee3e9484792530dc3c128da08a5a16b575a47906c9df1e1d79a53d2c89d36396dd5497a3672a95010c948c6470155dafa6402fb668d0cc2f6aba0a4c1ff" },
                { "af", "7009c6cf781122725af363485545395ad21830dba4dd801883e739e9de79ef6bbf268424b7f7178b7070a5f4e13db8040109ce2f475b5507006c4ab4e5d194a2" },
                { "an", "a2beca3ba809f10221a74e816e5537259720d99b008b7e6efab5e4731f196dd8bc0bc163cffb47173c7a45097052e14278758e1742286223e791256aadadb008" },
                { "ar", "a6f1c2f77272ddeb5d6714325423b7756a89759301d93df17a75d31728f30377968fbfa36bf4b00df5ebd20fee5e645a46a482582eccd4d411a8f4363fa028d3" },
                { "ast", "480eac2b07ca7bb4abe883a828ba8c7c90a726afc53c6c058d1ae8f0a5dc800dba91badcf017944c2ffe4786ad14294f3f2d8b42a75c1e37d09593a217d25ca5" },
                { "az", "7550d239e35ba3207429a1fb28832a7c4370ec7a455ec72d01abf3cbdfe3a870a54bfd34262183508c9ed329e1ca616b8e1d9724920313f1e9887e95d71aefa2" },
                { "be", "f69260a5fb641d6d4c67a7d8fd1e9fde4a1755ea3ad09887f6304ae7a8b28f0a7086189470f6c1b1b0895690573d8e16a5efb8379ead3f9acd94f35855173587" },
                { "bg", "e1d3583895cb6851ffc68548e2b9d7fafb664748aac871ca5de2bf6892c72908e1e49d8d0d9329e4c73a6af75b6875f5976c5972e67b4adf2e0895e7d4bdc349" },
                { "bn", "492088257f251a0a33669286a6285f91e0b6188c9a257aaa1d9a935386bd4bb516888a4187b92987d7acfb2790278cec54b7ce07f8a85b8ee230168742bb050e" },
                { "br", "688326e0aa0b1c57514159c1adc74b9f4cc02c4c07a7d0946a23a12099fad77089cd5bccd7b066166bb1c86f9a05cf1f45e29a2b7d49d7a85ea4067074c25777" },
                { "bs", "1e6a26723180f9d8644b86988b908671b116b3ff85b72595d5194df7f6e28c51934e3da3226b57b9d9ee3e96b6231f22a5e0d6e79c5c098f64bf0fb302505650" },
                { "ca", "49eeca14be7ec09872f106623409415a5ba8c12bcbe81bbee0111712c90a42a699d270838b6fc1fdf267321e7d0d91d457dec19774434b28b118aa3b3df22812" },
                { "cak", "39e577dcef7b0f15a19cfaf77cf1adfce70f70a45589289824c73f9fde57e1b16bc7093282c4840aafd1672d17d50740a92beaee4ae997a980124e754bf60e07" },
                { "cs", "c7096a3ca7e1b991353b280d9e934cf145ad5f8f4c673942d34517bc207400e8052996124229a5e382e5370808b0ee0d685818cabcd51f7691b69b51ce3b1cc8" },
                { "cy", "3690460158cf5982ce6030c06f1acf37cd2c35b867d2f7512b71b2b0e64406583c83e28c081c2f4c31be3c2dedf91f2a814c757096a1baac3ec38e35df5600b3" },
                { "da", "72ce449b811806db29bc5f6b1e0026bbb2ccaced7227d225d142b3e527868a2b863914ae65ba52905d771ab738d93091b8286241b6fbc93633fde64e87a6d682" },
                { "de", "8d8abf22b33499ece320e625aa7e137e25c3033cd1a0da8962b58d6edc62ccf1e9657911e8819c11b04b7f6c1ebb47cbabd1aea70a883f56bcd3855318fa616c" },
                { "dsb", "e9a8ae51bcbe816c0378493746f6be20d85e47c17038b4ee9fbdceea161d09ffc5338fffeb7ff5adc04bf83f8812e899cb6867f5e7d2be7a874ff635321107c5" },
                { "el", "f263c4504f980588d35addc59094a04cb71754cd6921399684efd3960c4bec4cae2d430af5902cdbef482d9f8f503aa7bbb52cc79784316ca7bfa48fa0155ca8" },
                { "en-CA", "8f757e9aaf777de5d888ee07855bfeaaa1b0c60705d791270eaa8f6140ed843aaae8656214853db1f0a400b38225f1b6d26a2d88ac4dcb20839781dddf3a11ac" },
                { "en-GB", "126a81c1291c8fc8cc87349d392e88b166ff64cfc1816c6ba4aac47c0f43356ace5084b66ede26a283e3444ed069289d1af86de4a85806c19806f4cefe7f2db2" },
                { "en-US", "afc6551df0271e7d8d5766c63addf06c28f6a83d6480ec893665f487c63ba4ac16c3fdd624a41efda2f74515895a05f57afd410cd580f7eb106651058fd0235a" },
                { "eo", "a2882609d37ccc8146fe60a49abf67524f138c195add9057229d2421350dbe3651469049c9fd41925ca22cf12ced5d369c09bc6a8830d8a4d400681d177e5ce6" },
                { "es-AR", "fabddefb6205ee0c759b38d6e6cb39f6d9d24ded954a8237516bb9873e0d1833eab5c9c9a04577b4d9685b3eb887167f9f437d1bdc5b0d418d75912067bf3891" },
                { "es-CL", "0bb1e4eb1d4662bcaa95b6efc61658c61ada66dc415b3af0927e4ffdcfc5b310676e9f2990afdacdc9f0b8c46eef7bca72bd07ac98748a16ada0d7d90432002f" },
                { "es-ES", "e078af0b8cfcab085e910e7effeff2f1cfdb7941cc8d77f5bab9e9268de5ff1f6d04ed0cbaee0730c522fd0bb3bc9cfaf862bda3052375d9af726fd6ef8f047c" },
                { "es-MX", "ccc359d569251455db75b81c731595b486cbc448efa196f69ce33a44aa53922ede28899373e9f6ce49be8397783742823b675e9e332db4152df7cf63a9d9ac07" },
                { "et", "9207b92746a0d787cbefa0aff5b83a5536cb56ad2e58f6e5887780d51e497bb852e4e22283f6fee9425945cda3b422b819e682ff0f46465aa874ad3dfe42c041" },
                { "eu", "ae67afefbc4c94263fb9d08af237141c78489b30fd0b476dd0bdb057a6c6447562bf9ba33ba57cbca32fb946481b3b480875c39e55cc190452165289704b5163" },
                { "fa", "2c932b0e1b7580ade6961204dfd1e546ff35597894fb5c66c7e87e2b4aa1c55341dfbf719cb0e7b3828a5bc81264c41ccb9944b2328a85380871d30fee8ef25b" },
                { "ff", "9652c8fc091c4b932668cf081ef48cf5495008e655ba5011c3a6f8326a7e5cdecd4d076dcfcb481bcf8e2d942ddaeea18fe0232f185e5bd34fb603f786e98021" },
                { "fi", "becd668fc6e3f5df6622b904264e51931291117d6405923ee3c9f259fd0265c49e605b0635aed66748b51def4b14f255f71f9042e28993408f22887331cf7f02" },
                { "fr", "d4e15d43ebf4f7d348380d84ea221f3f6c72ba228c1f4f2b307e91eea37e8663531bcaf01159cb6b2bee7499709fbd43a93dd57a5b0e63756bf25a1c36819c66" },
                { "fur", "2c91d1bb6ccc11d3a001c88be86b6f8e2db18fec905699f4639e88e3f2adcdb2847c1a15ce579fef4e435a532ee4946916beed6251414100511fcaed58838b6c" },
                { "fy-NL", "5e109d07338fe29e1249da6f2abc060bd92e6a946384e00f77d9040350c54ba685ff9842873e6fc1f7314cc19861ae7ea286d7c7e30ba5b4beabf2cdd54db89f" },
                { "ga-IE", "8b6f278e6cd137e7c17d3f89bb15de5b20d9957f968ab76bb66ca7e27ff5f9b7c1e4ab78ffa507a9df1de730494f73adea5a31d6d9c6dfd0d98efa2b3481669c" },
                { "gd", "1a64e3951d54ef0ec8b65e14558a45f6fa62121204b509d9fc68593ecaa61f1638d61012f0c1594dac885d780493a99d116cf2dd3fc3f2ed1c8dbc3817d1ad77" },
                { "gl", "f2e327a37bb24eab98d2f2364ebe880a69558f6f0a8c6aa8c91caf39aeb4f58379143018f3d3accfc1da4ce291b5a14e60c19ef54a3bb0fc436d8f7d58b4bd38" },
                { "gn", "92d0c03e58859869f648910b9ca9cbfbc7ac802784831e29dd084dbeda64b6a18627da8c34aec2256e1ce06de907eb3bc4eafe234c824892cdee8039bac2551a" },
                { "gu-IN", "2139a9962504fc187e4c230beafc52ce6e465847d8419732a8b9a7483c0bf8be47a8e465a9d8cc5a4347e0b5f1e359ab2347a1c48afa8b95efd3c87b21a70a06" },
                { "he", "4c6b8188d760b65ad3ea6988599ce744456bea2e4df62b8cf0d2e68a59216758134ca1e670626e6b79ef93f3eda3689403f3671629bde812e5c6f4d0a3ac1799" },
                { "hi-IN", "0a565013c769e3eaaa71f560fb06c48c55337a9380cb59ef9769cc2a1e6e3e0d5b5628a59a30668155c090007e363050df6571b131e7937e5916e0ad0d7b51f5" },
                { "hr", "85fe4e5897fa5604766fc4900b1571d89016d402aaa9bd1e97eaeed0aefd8cd86c83853ba70792d45a754a50ac197e755a4492d4fdbc1fc61861c579a2536439" },
                { "hsb", "0aeaeba861118b39d9ec4625c1a4647cbdf151fdb5e40327fc3b7c93f09c3e1c76b5e0e0ae7672e649ea6b8040a3f2057d6874ac1e156edbec2b3cde445598a2" },
                { "hu", "fcd9beb73b3e96652dee4b7c1bc753044cd1308a99aa5d87c6fb34f276677b77812652efcd3cb735b93ec3acfc16fc7eb0cd870dd23972fb3ef4e9089369a83e" },
                { "hy-AM", "92b5088b13fd6f6bc8b2313a4b37e08e41c77bfa635fea30be387f4a3c59978c5b2d4276dda632c18cc89db5d7225c4b5922f645a31f3801296e3fa40c43e367" },
                { "ia", "83b1465488f50c08f413db2540fa432e6df8f2356b31d350d3c57a635dacc8ccdbc906ca4a5804730773cf89cf19c4e2a90899e2d1ad5f9bc65e6431399cda36" },
                { "id", "b84f66c5e00c1fbbd211d75aca21bc975a06ee2736d97f9af02a62d61d9dad3062c1f025f725576c1f8e4bec831a5b30053d6b83f7e5f68370bd96708e0767a2" },
                { "is", "865237cf8c47abbb5d09993d731a84c5e97c827ee7f49ce71eb83e1e55eb26b932cef911604a35b4e8388078a818621e88b10faabda18f3518ba658c337bdfe0" },
                { "it", "94ef3ace5f45af37537af8efedb528e9784e8d861b982a1c0195b7108982baae74e983365b4091128078638324258c6d8a1054cc0e804d98ac5374c44d2fb20e" },
                { "ja", "17fc1cf72c8209c27a18c05e44604b239898147cef147efb47ff99f7bba2223651f08772ebe5df11efa845d4d95a06de22c40bca700e68b0ef4698385132bec4" },
                { "ka", "f6a946b10d0fc4ad2d5e6193685504ab294407cf16e823d0b1324236a3471db5c89ffebf8a33507d547f8e3625f6e78cc7ef00ce5a704c671794506e17612498" },
                { "kab", "8c6172bd96d6df231940e5fb49714f7053a7a45684e13aab091fbd5e77dbb9d0e37ba8e46eed7db18782c219b3f97fb531c307e69c1c406e12a4ab132c0a7411" },
                { "kk", "ac6c7d0345ff3e2f2cef66ae0d7713017f923c34faacf4bfcd665fd662bb210dd0065fda7399ee762042836faac6059df7d65406b4496c1732b11989b04fb9d2" },
                { "km", "4810eb5faad8d397c7d70479e8381cba8b7e1e5c489df92e8ebed3ec3cbc4c1ca5a30bab401b466d7a176372aaa746837a59da8b5004b289343e5f308d43d89e" },
                { "kn", "7efa6df71658f39d3c42e6f89e2b9c78c396e518a9e7b550dad55fa3c5ef34b3b349662aa0b5e3a39f58bfa36a8c2e092007f581ce6ba18fa30b414410346e42" },
                { "ko", "572206a16cee0ce30ffccd48b2e63bceb7938c5dd9b394353d7999ea8d4387157da4aabedb115d21a02ac3fdbc27cad49dcf5168225ddb2afe40d651c16bca9e" },
                { "lij", "482ee68b406bd133b583386adaa3738a63334e659b715dd179f85da29ba4907b070444366d346069c638b3e5a599c8be0972e08534e527e3f399a2f0cd46a9bf" },
                { "lt", "2569a6ce8bb437ab3f3d274dc3bc8ffc7f2d373dc4822a0daff510908da6e6009cf81f9e8ebc89f23c658d4baa50a0d5334ff588af0ac521574643fa0aabb4c4" },
                { "lv", "9c188ce5c1ddd8d42d5d82957b5d0f8846d3797c03861e56164d55d7551f2e1fe5c81e0550ac04a06ee48ba6aafd543d28b1969b9b43fe3a1e5350a4f3899d30" },
                { "mk", "e9a4f91e2d6f799caa81898265983760e6a00c591956d3ca6891bdf88091b1546e256b5536d8c75fbe208fe91e94bc9234c2d82a1d4af0347bc7a3e97295465e" },
                { "mr", "b8f3ef390599704175a72404fd7bbce6412eddcfcc002c336b4b64601493b4573ca6670ac1e6d2aade729c784dc3d833fc985c6684bcc0a76f0910aa037348f4" },
                { "ms", "97fb715f74a299bf94664edfa0aa4ae6124922cee8ab3acf0c787cace29c9b9a768656a2159c84d7f550e09b5818aea32c4d91ae79f28ea0e4373484a136e628" },
                { "my", "764dd633fa477cbe5d3e06d0c57040bc1772022e9f8af9ad4be2220d1eb8b7d84872e8514dd45ef6ef58ed7fc3f2911d5ee768ae5f2f8d7715804d4efb2eb081" },
                { "nb-NO", "7df065d5dde0f035666bbd58d8a4ccafd45d1a315f214230f2f282a7b0331babafe4446032057ad18ca987984a4bfcbf5dbb9a20b3c170f90c31f348af0d97a3" },
                { "ne-NP", "213d5da3038dc0b473bf9785bffe74453a00c8c8d0d612b86a669d4676756b5b0080c72af5f0b1c579ede8780076e9276b955632cfefb4c283e4978fcf806abd" },
                { "nl", "cf77718fc6bc92f7ae99396ab32c120b0b8ed86485321cdabcc05dcb2eadcd831a2147ffcf3a5fccfbd8b2651451f41f2dc0bd9fec24a44e22cda1e028bab8bc" },
                { "nn-NO", "f876afa4fde36d4b9c7118befd53647b1900383e35d5e5f84d3a886d84cafce88f68d899a5d0c9c29e32c032bd856ec685afc32bcfcbe578b9eeff4aca37bb53" },
                { "oc", "e0240e0acec63db2b93e66d83692e3ee5b730733b9edcb37ed9283fc6974d2a55a927e18be46df7a8970d7fb1cb0edf91e20f4373c3fb5fbd735ed872b09e989" },
                { "pa-IN", "54de7819c06e7b869014f672b3e06d1d5e2b8326c0a3de8017b1230a85a6af91adfb0dc69727ea3854bf69f83787f6fbf68c453ce3a7ebb841018d2ec9aa826b" },
                { "pl", "f4a704dc97cd9a5b0131a14f3f3c4f340292ea0d50f8a14af9b2f9a9cbe7b09423c4799ad04e4ad8ee85eae5aa5544ecddac1daf8de36ce6638a40545cc53424" },
                { "pt-BR", "79301f5630c0090f128e6f6255fc9d0e539be5bbccfdad0c950707655d303af4d55164374cfd7d89cae2ff99008762834a45bdf7ccff3a5b55c62e677b73bb18" },
                { "pt-PT", "146cab558e42a0a272d0a12a94f607e521fa56a2138a40362d7919c8ff2cfc093824ba014003f04736f4c6f436872c4c2241176db8ae5cbf57ccbb0ddf43bc66" },
                { "rm", "63ff177930299df986e24855060482cef1f9333c22102cfc4a122065620228c14a6c7e16bebbc592006c04b667c24ecf0bd7a0bd70ca324af92a7dac2d3712f1" },
                { "ro", "98acd9f98d9aca44c4ce0e407be6f6f9442a254d32bf97b408998153685044479c7df8a650da994f13e6244dafae4e93173c835ef464ce981b94a71680ee91b7" },
                { "ru", "9287c0a1940e29c42852250f9c1f94024c57cd8904c5c62436bc18ac6422ec126b5ec3ac23d7271b9e0064106ac8735794e716e84631a4bf0dc17e4a6ff720f6" },
                { "sc", "c8c0e463b10521dc247a9259574b3110e7c9f0eb0acdcb560152676c97b83981334094c0debec3950d933bd511666c625d43ef41fe58fa7457a2618f53cfa323" },
                { "sco", "0d22e6308095424504cd890604b48a41abd0f60f7688525988deef12ae42640f036a78c031d8b40d6b2f720f724ae6a549e83ed1834dedd1be5049ddf82ea341" },
                { "si", "b06b90157f325e71d61eb82bbe058e45739119d88aea14f727547cacf6db2207284035bc6baad7eb0162b97a6cf90c651d6edb2fcf54594917b1c4461a865d15" },
                { "sk", "97c6b5f93f6538e700c552866e1b5fed5b0a5c6afcbf0e26a7d5d1456e947dade6d87cde441aede3c533964a2f1b43f2271a0671886d68b985cbaa2445d868b9" },
                { "sl", "d55847fe4bcf5964a22682f2f804b59a48b0c03b6cd4ca3fc7e4c293c8c3ec9a9344e741a9e4f2ae2972d6b41b902c853e1e178b64557352c1c129e4f8eb3064" },
                { "son", "f9c47f02389167b3ca0a955843c67eb7ea067d4870f3b8420a36e9ae34a4556ebed12724769808416d9ebb478ae435d704edf9b96c15c3df85f966aa5b709799" },
                { "sq", "9f6b1d2f29ea72c5c394dc40f54c323c15621b150d75f2d9476b0a1600273ace1b39286a4c6df8bc287e741140a3fa052cc05ef0c0a0f86979270aa9e9a8e0ac" },
                { "sr", "2465f1d08cf1561d123fbf82cc98699d6f1787e3287a7591723e4dcf4cba0d2abdc283ea0bf57ce633f4e2a61ef4ec6fee0058413a5e0a9ffa1e0b1de7070015" },
                { "sv-SE", "55629102519cee84d9718334fbd70a6635fc6e43f1809f417c7a7893f8e24b449fab2fad9f2ab372f66b4e50d1fd6a12fb46e8a91b81217f0f9b0024a0a4daa4" },
                { "szl", "0847d41196f0acf84c116fec6c59e8f3ab4f80752f998341bc55763e24dde21e7a6f50306d07a5e445aa1ea7bd721a5570c75877d89d7abe75a9b93327a6de91" },
                { "ta", "6a92fd4f25d7b74c0d8fb85b42342a53cdbae63a885ffea652c24c5666a178b20efa4d9fc98c286eb4bbee89d31f312139eeb0713d1f533fc2aead2831635b3a" },
                { "te", "b08679cadcbfbca558bcbcbf14793a9f1b971f5d54eccf44b8dfa7cece4fe606abb4f190f210f26407da75e6c3f67e668f31ce711191252abf94e608d90e70dc" },
                { "th", "78d01ce37f8ead1d8ef42bca405ec49263e90f5c5ffde8d4260f146a8bc32cbe6915ef1f97661a775d2982260ee98e20d5d4486a900c2654e483d8c82fd99a0b" },
                { "tl", "cd17cb3587ceffd5951ae3902771e1c1e6bd65cf4d55aba297e2f138229ade33be2f09f9fa040129bd7969be3acba27f3e3b5febc9c53e77d9a340eea3c3e09c" },
                { "tr", "08401457b13f3f239a549f81ec915e8584d03d7b327168f58247d8e5c0220936eb9b898d118ad75b7ecb6ccc1b8214e5719d46a1770d56ff6ceffc3cc87a148a" },
                { "trs", "3fb60b4eb6dd4cdda14bffe07acb9265feb73cff6a83845b779641147643cd4735695b61d1e028a3fc9f2dcac18d55db6b41cec0b86154e3421f3ec95899e63a" },
                { "uk", "e519a7fd909edda5984af0bb0aef78a2204ab648e96d22f067ba7cc37017c4da2c2ea16ef00c3d40fab8038f87490c55df109b0c5218dbcb13bf0e0ed143d766" },
                { "ur", "c338c25e63628ccca97207503804ea0709c9b254776ea22fcb1046dbbb049317bb2149ab78d415ccd0d3a88e3d878cb70817cd00b94a1dbe797942517e5e2099" },
                { "uz", "d4dc0d6fb0f0465dddde8447e9966ea59be898ca2a5f8eaf8cfe17927e4a6545f9fc250cb343cb6a4c1677ef732525b5f83c9246a82f4af1e6eb30ce48bfc76d" },
                { "vi", "9dc3e504687aa662314d4d2851acab1a7622c5f00ba2b656674d5517c269a362e0616beeaa77fbfffb7fda9a04a712f4b6919374d8eaae290c2d730543050b1a" },
                { "xh", "e83cc86e633daf7c3c556b649b43b0706011c6b0aa8f9b6d41ba36fefb2f9d3dcf0ac55d8b8e9575fbcc02007910e587703d5be33a009648b83c5f96df97bc02" },
                { "zh-CN", "390363dd461c7e48de220510107f3e3d05c4cbbe49d0ea55f1f58ec37c73c06c24cc8c8d7616352179ec11c8a92b82581537170584a514c5112f750f10268660" },
                { "zh-TW", "0840c909c8c6b3f095952ac137cf61a94c68448ce2eed0ec32022a7669ac7f4da625b1c081df53a199dd0d30b6540c7bd2e88dceb5b1cb30efece69544c4cb71" }
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
