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
        private const string currentVersion = "113.0b4";

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
            // https://ftp.mozilla.org/pub/devedition/releases/113.0b4/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "7cb71213111c842e90d4b3af18d63a3649981f611f6c94bcd62d0116e3c072ba363691ed18fd1bb8bb8f3488d2639c5b59382cb405e72850d93aba9582ac8dbe" },
                { "af", "52f2fd2442e5211a11f8c700cb24b1d4e8fc7115f6af27132eea297839a12204718ae7856975af7ffea99e83bc39be4827ff2f9c3b351e9903f0a114eee36be8" },
                { "an", "77d0991eec12640fa710d29321ed2317fd8df90a576421e26ee5e226474030e97adc8b0720a4347b7aa89858767b6687399d573b7e54f4ac0a3344725ef5a946" },
                { "ar", "b46a63d89ed176ecdc1b5f6f7347202df853583871ad6b646d96c866297846ebd42c864c3630599d53c3a4bbcaa555b1c4409687aa9fc74c91b27d9768b6326f" },
                { "ast", "52ab56144c1932b2be10da59ddcb1f24d7b9cf7cd075973331c9c18b84c729220c423bd07b012cfd5c0f9c2b75a840f8e17a6355684b4780ddbe3d429ee1a7e1" },
                { "az", "4a955df0937a88c1780f24c124c3264373be47b72d8213777d1d144c2029619ec74a8ae5e001af663bb15df0d4bf718a707b5c9459f60cd852646cc7e3d251e2" },
                { "be", "b46b172aced61638c7b602987128b60981aff51c411e3e5d7d01cf51573f6192db4de912c279763719cf08cdb82a194e81253c5167f7f0be73fc31e0ead76bf8" },
                { "bg", "495dec098c533110ddf1cf1281aa684a2ea6756d4c63e280759556a1caa9bbcf98f1ca2d2dc24cccbe52f0e51a2e7f2cc974d794a347404eb7e910bbe7ef90f8" },
                { "bn", "b5337fb930f829a16a9575bfe8b66a45776fda0cbc53c40c28366e2957e5f4bcc981eb836ada769ef459e7c3ab9eae4e6f0b5f1069f19e3c4c944ccec0dca5af" },
                { "br", "cc1adea574ccbb2f4f46cb795c196c790af95c4b41ff839f497fa77b1097d65adf7ac404ca6c4b30d6a67d1fd62cd2fa7dba3c784a8f34b24cc53d3c30021205" },
                { "bs", "31114a1d73ee41a36ab42e4a903406b1a849790648e870ccf64aa9526365b0a8a6559a7c00d63070580bb86c74b7a334a731762c649e961b518378b698b0fc58" },
                { "ca", "9af44328a74542f038f3f0320b8b18d954d8c33cdc0c799824601ddfd9b3ec2f91292a7c29b8e32515a2898f2ce2568c688224c282c91b7a0c4e656ff258f1dc" },
                { "cak", "8264ccbd65d45e70bf1c779f79a5a916c6dc8a0df64e2c6e697df273ceb833c99be92b0602c42f7c160fee4c2a89a7934720cc46414b624d8350ed75c9e37ae6" },
                { "cs", "7cccfece01a9e4b7a5d3a04a9fd04276bb8f3925861146e27a784b6b84b10c4746be7bb64069fd71f28b3b019d8dfff681f7248eaabf8596874d29b94a50c788" },
                { "cy", "cd72b3d7b64b777e6285b434f2db6c4287919ee1831ddadc1d543d979e9555a71dbdc95162053804046e2683a601019066add785c4177e6e8c32594c7f678ac3" },
                { "da", "ea0690e4277fea01458e813bcb63e3a7210a20f4316ef674c86ff8f04382f9145b7728ad3ee69d3ebe2a41beadeef3326b5deb6c6268060b43b758e5ca06d691" },
                { "de", "5703824e301f722155607cc8f915cc0f2528e32968f6788850e92eaa2bb082021326151f02e68a74b44cc682044bf6abb291f3489f901788914d20eeafd4f6a6" },
                { "dsb", "8937aa4b7830277437347024c6b65995f30d333427b7fb6b632cd4636ad1bdbea23a8589562da7a30af68becf0570bd0ff63721eaeebb294d17483cffe127a38" },
                { "el", "e14d7add0baea15df1ac4320625c073b852e096b7150dab6e226286153cdd1c5e0495244210694be7c4e39a37ef7b3e2cb204382d5386511189b0d115974d493" },
                { "en-CA", "6ecb9ceba17f99e5f49d8d78ccac3452387b77c7b2659ef787e31a8e4c5f8282abd1a790935272823fa6a0790be2c6d775f1b85c44a5632067d33dd39045b13d" },
                { "en-GB", "39aafd52371193151069eab09bf4e6ecaf6f3c53a9a13954d77f0eee766ff3152e10f1f08d5f6a9d930adb1ec3c8be3a2d1eb8f6d9effad6ec854e8a054d2e86" },
                { "en-US", "8e1917c965857e58fa3419f8cca7500f8da1a8fba6dc14941a3cf0a969e82c1fbe426dd9bbca38be92bde7472d78c60234b149906d81d19475a29a4b28b91e83" },
                { "eo", "8e73841ffc8ebfb2b67cc07f0d4017c825c1584b46e6a6be8d1004e660467df7ac6e85d00c598b8410dab02205a602e8ab244d1afe1c747bcd20b0b42fa06c60" },
                { "es-AR", "e1798254b1d9c66aecf31449c19b4bf40f325466d082c12bca7ed46de08fc59d566af3d012239a3fa94f6c824bdcbdac7fa34182c223c1380a6bbb5acc9a7465" },
                { "es-CL", "0b3312664e4c3f419ec090e868f17df3380132a25df35b1ba6d2924bc3b497997b0181d14f8e83d2d11a4901f69b66f8231571aac6bdbdde9635fe48c2ba1a4d" },
                { "es-ES", "0f74a29a175f3a0056db0ca04562b7a4688bc9aa18c2d07d6250f3e61c7791ee0fb7d048adb305785d73c9faef2c4b5b2e4b4e25c1de91aea13da1351a8bf4b1" },
                { "es-MX", "53e131b7b34610cea88def2f1d61c85e8167bb6d4114b2abbb989de53736d767ea580509ef73820bf371098cebb79e91cf94e858147e98c38964d2d99468ad48" },
                { "et", "da76d349aa5e3576acfd42f62a83d5415894e7be1508154d14bdc32e6a0f4316a2a2a86a44bfc3a2aca9722771bafbcc025302a8cf0bf874da574ca720315152" },
                { "eu", "07774e760c0360bcdf1622b0e0aa8572b00446b59f10e41dd341c30c5f6080f35fe1a355d7bde32835f882e2aa93e1ed9e5610f200e207c50d31b0c41d559d91" },
                { "fa", "5dc9b7c689807e862c1a1f6ce5c4f4efec664a9a8593b2c0f1e09e35880975056148499d5eba5006af4366f2b8adb851f43d24a790ef857353c258b0b036b2bd" },
                { "ff", "8adf7ed0273c0c823e8943f2036569506a4e8c6c3c3efde1ea1e65dfc7ff76ced9eea48515290d4bdb30517b18a064dbb6fd3722b1248007bebf597be6bbcbe2" },
                { "fi", "967f982721c051d8e194244304c92ffd8038c9b6a01c9769961d5bfc00e9d8da57aba6224e8f99f644c8a5d81c9b49cfd00e63e65efb692578dd270dafaf7bc6" },
                { "fr", "d02de724ac324fdfffeac276538b43e84cb21471db5ac3452da119df73760aeab807355381eca0c0051e77408d63bb64c7697b957336c8551641b2e1351d85a3" },
                { "fur", "fb2fdcc87f60713c471ed85681114e081cdcd49db124150a1f33a99e42a03eef1a8c1c83827f0d7664c84914f0995f523470a78d842d92c2fe37dfeb508196fa" },
                { "fy-NL", "416754213f5d513113f7b2f9b34a0531f2ca6fdd9e42ab51cf8e1b876b0a8195581bd80642ad218567ea0134babf05f6c6e562dc0ec8e1b164e82fec1e768ba3" },
                { "ga-IE", "f4c2306151acb064a52c97a9143d833489b533dafc025a008e854d0d88e1d7cf52ec2c04d4b0b8356bf1961366867c9093edab8a7e02b385ae0e6a63a1ecf006" },
                { "gd", "59d55b3aa26c870a44cc67f838d8a8121935a3ed5c7e4ef02cceadebff70eef37dd0a56f7d2119be677087dd0f0b85bf024654fe2b14cb8c0352e8b593b9eebe" },
                { "gl", "181675ca1c64369ace9ecb0555b4376f7bc5476b83be074fe67aef87d667a29037ef1de94fbcd2ea037b998732a595036428fdade1d6e4b4870224d3b8e3fbc8" },
                { "gn", "5afee1e485f5ab924c01c612043c0e931b71362aad052613e61d460dd56d04b349950c3ed031126c52b5d2da4afe714d520b5a53d0ea8432a483742272c16347" },
                { "gu-IN", "b19268c487f41ae17d5aa31ba636ba0c0b4a12e1179fc32a3ca7e98ab99d5a1597924f5349b1dadf2f913832b3694115150e3e8cdd27825b25605b4013bf7b65" },
                { "he", "8eeafff4a4a1cdeebfb0ff718d6a4ac6f094e274811ee36280523061079cc826e1388e6aa134ad44e6df0158bdc453eda9712b04e2910cf97cd8d2d227c9d418" },
                { "hi-IN", "fe28db3ef4d3993a8002969f0421b301d25c23261ec6c6f3e148bdd4e7141f7acfda90a121414532cfc8fd2c015618d98a9b5688bbfcaa19a82f4b6653a2cc15" },
                { "hr", "d8b36aad6b5994bd6dbb9c8af246f8cd9c48fef4c637ac7c64f779ae9ded8cb9bd2c3b8a30ed5115d589cc248cfb76fac74bcb9188ee2d96e4cb6f0feabdc675" },
                { "hsb", "deab2d1e8304eba08768e03417843c9f1577a7d88426a816bf676d00741f8319763f8a1a843fb44738ff736fb63cfa2052987deb176a8a051116de2426da220c" },
                { "hu", "650293e58d9a679eb0bc74c515c5e8d428fdb9621e96052205f8cadf3d5f7ff0eec40cc6cc6192ce94e6653e01ee23d0a87ee838ed5110b4a66f78e44437cbb0" },
                { "hy-AM", "1bd612b2bebca4c453de0d356e3f6a63b968f72e65b65738d0c302c2ada0f2fb237ff8056a585a27fd45fa988ccc1c48235129596beda355b92683842aaf8c25" },
                { "ia", "8f67c8bbab776941c911194f38facf5cbced421d815e6b0e09e12fb761f6dd3689a3c6d066bbfab06496e3510fe69f91953d255da60a5bfc533106c98ed10a92" },
                { "id", "c7ac74626308c4738e2cc937b3ddaefaeae257726aa1ebde202de8b64d9f87d08db7c0ef0988e680d2f1ea97a15a74bb82547d08738581309f2ed56bfcd0702b" },
                { "is", "e63bf06ffe0bd10f199fde2904b1716e83018bd0e2da6a52fb637f53686d082f5324159ef8b7ccc32ed27e2ebb71ba5ace60ed1cd824662895c6aed58d09c27f" },
                { "it", "85591083176f312ea6dea618f807ab449d832884485ab4e11c5d4bc7b7607487de9c63a922c1d8e85ef26ad2893d675b813c965c0e68a8726127e6eeda06cd54" },
                { "ja", "d7e83ae14f98b03e04d413f4df7bd37d84a0d8dba80d50ed42099f368e059a7f8e65ab1764aa2db5fad2260d215de19e4153419da4f82536bcc2686971da59b8" },
                { "ka", "f1d82b7089402144d2ffe40dc61e0b435876573092f9c09970fb8fe9c2dfef48cf54142060e80c717e225528ac711ed48ef9ea034ebc509660e3ea4a76543b44" },
                { "kab", "68bad9a7095a37f75ae6a7ebf7532ebffaa02e2051b1689c88c4f9adc10c54c2cf23d4ecdad458572db6d4686d04d949af65ca2ce35fa3a48dcb0dd04bc43ad6" },
                { "kk", "f1e1b021e7600a15f11c5f0fe3ca79c577d37f14432e359a984c18c7a6295dc667f9cfb753fb695cf775bdcf104e90b3e6a79090791058b97c2528984d8d8c00" },
                { "km", "be3db5f28d90c182b762c6e94e611409f9ab8f502bda29673abddd38f132793e400e7c644f120560bd97d47b01efe5d8a6c8461555d2006a6cd6bc24abe3aa10" },
                { "kn", "6c5c88154cd4ebcd0c2b50d89268c45ee2e3ad9e16940f23a71748d105ba181ea79af1752da354245f087812afee989260e48a88c1461bba795658decefa92d0" },
                { "ko", "44099f6218cb1a8ed894b257b962bcedcfd5a4dc433d149b59834230bfc1df6747593e2e0cf28264d56e99327158fd90ec27e8c81fd087f0ff39068ab2db8c5d" },
                { "lij", "fce470450927d2298112536a9e8bc24728eca5319c462f6d08e5ffb31a5355e59220c658451e9637ab274134191d292cadd713a6d06287430e8bda90a43d9e95" },
                { "lt", "3ed469de78074b2e1cead0225acb02683de9f2733af6653120789bda704657396e94b8bb6f779dc6c88ce11331c7197f26a2125216bc6232b4f8eb2c00b1f5b9" },
                { "lv", "d05156301726fe2777291866a520741c334884bc3bd0265e462c9ec265258a1cd32e442c8dbc0c9df8e7cd4f6ff5b62af28145b1a9a0c872a617f84ed3c150d5" },
                { "mk", "e167b80a18a848164e456f6e2ecf7ad4fd04ab8c259359f3672116f7a995988770ba23dc898e17f6187cfe00be8fc5785461f295c0c7f187e6994d944614f5d2" },
                { "mr", "ab78311e2320e1919920faba5ab71000e2163636e7d7fe4147e3622fe3ae3ae5681450dfa7463b83a28fa0a6f424c89686805bec3a2cdb3e2dbfd882cf7231b0" },
                { "ms", "5d758e0bca65c901573bf63e6d8df86bcb034e57bb60cbf5fa0a35e6ef7cd5a6a5550b6264e942508236f19f784578def164889e6a77e398c9856d113d357e5a" },
                { "my", "3a1699c6bc4442a7842f8689602109fbe3f177dcf4673d17b16963eb34b0254d50d2329126ba30113cd3613db80c8e195d3f83529a746f8e4381acad33574f86" },
                { "nb-NO", "4f9516ae3ff15fe52b422cc79f43c673a23d36c7b0b9370d1d956ddaccaea5c917c7c7a7311df404bdfdf88fd2500cc3f676a1371a7bfccafd6667b5be6d7db0" },
                { "ne-NP", "bd802f7455b94bf369276a720eb16282df9174450dcfca8d4f427ca509fdd62b7f707c063e7b38207bec8a99ca3e73315896fbf37e1ea72616d3ecd4796b0799" },
                { "nl", "c68939acb3cfaf7090c4c1902c4446286c1c5d6e8f28499434abbdde19b6b60624434e05156a57de2475ceb116f111eb20a15bde81e8407ef7b68a5350c4e87d" },
                { "nn-NO", "b9f75ecf6a07140e4105f96521245091039edd0326bc2808d5b11fb710061245daae58640ed9e63147f83f8b297b39cf08940368b71ddcb712b0dd89d7a2fc03" },
                { "oc", "fc4d30606b3ea55218f1d15b2b9df6f832eaa49c9a23d4b4d96526f9d75f7ac932ed4951b3e55d1244897c39a5f508a78c24bf9172677deb854b4e0dda201564" },
                { "pa-IN", "2dbe95358f7e924b8736e5405ec283d39944701f84374e9c809ec4bce1631e7013e188c2000bcdc4efaa8613b7e45f7540ebfa89d3e238365ed006d33bb6d7c0" },
                { "pl", "661579f318834fb631aac9379ab58e3cb306bf708bb42573b8601e0be0ebc2ee7ec7551541b45b39e97b91980eb71369cda826bd9dc6c9abd0c804f63b98f89a" },
                { "pt-BR", "421f2687498d97132ae5f61c57bdcdce95a20a8687a696aab6c1e46bfd2ca964e824bdf7cf34fe8b71764515b92d91dc3bf5dbe23a1b409eb65a81e5399c13a7" },
                { "pt-PT", "660eb51fdd1c400371d1a5f926e4512ad86e8111dde88b15235d4b117fd8ffb2ddef885e913c0761a20fe3948bd10dceb08ee3437cd47c8db76f22c59f1296f4" },
                { "rm", "6d31d79a310d3152c37dfe0fbd2dfe5b3b039cbd2fd5e8a85c7173d78df282897dfa4aa6f41cae3e23eaa61822dd630880f2333a6c4eabdf92d26c6c7201c313" },
                { "ro", "d69d4ab88b29e72fdfe66f388b031f79a5d40e3164d9cf6734d8d0a9648ea82c6182f1e702bf748b903a7d383442bfb19f08464d50331f2e25ca84c2c864a760" },
                { "ru", "80be08a19de762e4974823f25b5f454571572a30228a069fbbb2210492ef483f5caa9ffd3debd3681356b6596731c7eb31e28fa8055a08674dbbf3589169ef4c" },
                { "sc", "0d54d6fd93706c34d9effcbba89894a1e61817e54a49bdc2162c5017c29f8f3f7b4aee2f5afdd3b024556b67e1a18da9ceb18cd57a65c379453e919cdcf1768b" },
                { "sco", "f208758d5dd199c329871aa842c7099dced35d6f7ae9476a114f55835aca40abebf096be994dee180f6421cefffeb45be56253b6ed7cb94873de9d419557220f" },
                { "si", "25c515833dd9a0537f7a8fba91745666e77e6f885abfe3ec6a542d9b5b377b19f6a5a4e08000f8113e20b807bff795a931d8d8641fc55b730cecf6a97f30b693" },
                { "sk", "9b4a15f35ff3d3fd4f67779a1b6c08b6dde0219b8e0cb0c74a137a450e185a5f2ca4a00069d67c015bb6170df28e5546b4d1bcf99c06806367acf416f052ac28" },
                { "sl", "20285d9e32ec1ef685226d1e6362e71e26a9a24da23bf4147295ebf9554cdb2338c7f2e4c1324497282c2e238eb466a7e269825c63733fa53447fbcfcf41a425" },
                { "son", "918e83620b78128443f59fba244ff2672cf04c6c48cc6a2d2c04574e4581b8edd03808ba3da64eee1055bbe261e6dc58baa7f73ecb943c027aa5e4d95c7ebeb3" },
                { "sq", "7bc41f75309fdf024157c502738616dae98563e1aff6b4ae3fff8898fa3b4c5d28a7eb87c1bdb01125c515ef9f8114eec392718a5dbfc084c877258a9f87f9e8" },
                { "sr", "0fd155969c6869dee104aa8407e7e45fdd33b73751dab5a6a37310aeb5aa6af1514e2362bc988733878036878359c45613c9f0508f049e4777b1b05ab11d4a10" },
                { "sv-SE", "c216c01a54570199dd868c847b9f3862d894a61a75c2d9861d3c159801e456b6abcc706ad7be1e9dcbe8b2d400e8f2d3bf5fab30994729ec06bc81b46dbeed70" },
                { "szl", "4ea3bf6130a1c54228ef35c099d83f13644d4ef04d4def8be662b6708f4785cbce5eebdd6b42228da28d5ab5787792f8a7fdedb630680d5c592359fcd693587f" },
                { "ta", "689cd27d2ed6c225809028e6a5b6e23be6092c7937375458897863abe7a343a328b343c6ad89f0153acd80849ab4393423a6492c19726956f2f99bd32b52bea1" },
                { "te", "6c19a4600992f28c0c44d5a22ec7827045812de6464d4d0e2c5b8bb5ce2b217330eb06c74c2f01df3228e975f421b896c7a93f5c28107bb840ffcb2ea2dfa488" },
                { "tg", "ad49be0b9a5e1f903adedb1431148b7808c1d5fb3cea47b3279aa5d4c87265e55c845c5c6d2fa0c8ec3760203e14ab15d3e1105e7f10b1823f155a04c2a2ca4a" },
                { "th", "4381c1bfb724af25f09581ab67dd2097e9e02faca0ce3ee69935ea8641be20750342291a3e07f969b76de295fdeba00dbda21d236aa83aedb3d8f907950ebe24" },
                { "tl", "4af3ff574fda2ed5e2ab603b2f2b77bf563e3c360dffbb8b57c4d9f315778b3e80ab90d0af10cc26ad8141e3668ddbe7774bdbc3fb6fafc0ca2ee18cb78b4e77" },
                { "tr", "224bd0540f78159472ef0dfba23c8a26214a73ddabb10518fa33458def003b347a3f72386e5218e7a34b59f405064e2bc40d6d7f7bf053ad36237a7f8eae6590" },
                { "trs", "d789fd0ac8a8ba4c7588b393d85e1f6ebf4caf0d6d6cb3f1014dcd07fecec627c1d733e00dfea5393910b2f1f5601e9514295dacf915fed43b8b937434d3d2c5" },
                { "uk", "635d2b087feee24c2a2b4923ad2c74bbf93c9e9fc8ea6d7eba6dfd3ab682c40b147a974c75c8aac6485429587c39d6421c4b1dc54db7857265e27689945236c4" },
                { "ur", "66ad92867b4ed6226787fc120f0bc660974625b762701948c4068dcae77b8ed5a27b128cbd6f64266da8d8385307f74c530a7a35d8087053875844adc3b7a5c9" },
                { "uz", "27ceabfba54978167cd215f0f09fd8e0dd94ba0c2ee41150d88c6a7af3b62aefaa16ef78f6d6666de0fed147e1bbedf1f8427cda398d4313350628b4ce4c45ec" },
                { "vi", "bc7a287406767d0fe12072b5a4096f588c4feef40fe208a9ffbf2ab45cda93ee05c38e0fdb299a9075110830067b251c34cf093ed44ceeeb78f83300688e4f85" },
                { "xh", "7e267b039edf8e5e3183223c16113176fe101607e3fef4e77580faf87a31e030486d3f7283f879f6d16f1f44c0129e69cc1239158a617a816d5b791315bf8f74" },
                { "zh-CN", "5e8b42478d0c2975da0416801a927d94cd1b348ea0a96b18f0800f4ed9a7b239b5967be95f26a83bd964a3c3ba6ceb13d4eeba94381adf7e1570d0cd5258a1c5" },
                { "zh-TW", "66819d052b0726f6cba32c0ff344d9d38b570157e4f59cb3812456d88258a5e55b5a508453c87063812db5c401a82664dc46e97df7db10c25e5a4441d2268ce4" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/113.0b4/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "1c9deba87b1d2ae019fe81f5ce7f9c476f28484ca255b9411dacf9987b6c7848fc7db7e3a9f0ee4c2818a33f16860288a0c388afba428b0be31a1fa736088e17" },
                { "af", "540eddbc38df4b6c1506c8ce0dfd1fb3ca166beaefea806f4fffe09528c19fff9da0ab81d9832d535658c836acdfd622fd9e8cab0e2ce5aca37c6513cfa21fb6" },
                { "an", "821eabf31c1db969cfa2f583ee3fe2661468b1b1383bb308e14d7727216a2ed08b11b4dced72e8d474087705419f60dd9d563fe91c94de9825cc8b57f1c4b657" },
                { "ar", "d517874a77e9d62e1cc5cc4b993483cf788e059d9831e21257bc5dee3e1ab1c824766b00a02010007af536347f5dca725eecd13c131fee1a0a39a065966b9c2d" },
                { "ast", "e08c8429d55e739fd5fe72761be803990ee3f42f7dff642844cf2493d4b44dc79999291496e827701b42945e139ad8c38a119b6c643e85699b1a04b6ba31e911" },
                { "az", "036594ac12dface11754e663cfb0f268967153beae9bbfb6433b5e3aeb0e3c966a8804062af5490f32b3594b5449beae5086d28883a818585e3f5ecfa781452b" },
                { "be", "1b5e5a3eaea87125a7929b2e7a5015663e3850aff80d8ebeeeed31f8b01fc2861f7d7fdf0f3fde5014c19241ab1c3e5992e30c087af2a576f43469f9bd7117e9" },
                { "bg", "1101c1e517490f5add3beb12203b76434019b309a0832d9f9ee996fe254c5d3ff5d09e0bf7d431c2c9dbc3e0f433de3f8698f82d9a78e50747c4f4d2cb26ee1a" },
                { "bn", "250274ab401b7dbe1cc90aad5c7062328333410123ca976c5a3ef0184567f47c4ed2cfee004d0608db01480c3303332c9cda6ef3c0fad5ed4a88324e4be90287" },
                { "br", "bd463e554e9c8c3a35335d6ac3c57748c3ebba4cc04e4ea8f252d1648180e79c7a74e6608bd282d4ef86a5ff0917946646ba44699ff7243e02fdd574d40b8f4d" },
                { "bs", "e9ae4155be31912f8077ce079bf16156c1152514f4b23ed0f40eebd4b346460d9286ecd1cdffd337fa4f2ee3b22397de1ca7f0af62b5944f608a06d457f51953" },
                { "ca", "11719f60a41ae3ed14c6728564fa61af28b26df7f06e322a4fe8a08c17ae1858f835ea56db962b8b568b42e83a436049616cd030797551dad1143c4ef0e100db" },
                { "cak", "68997a2bc5501ba251bbaabb7b934b97c17146de26d9d44092b44a76b2a0f1473074ebb44f5fb2926eaf18e36d320a0b54cff5bb0db9ebb158ed3cf2f351c055" },
                { "cs", "dec10fd332be5a922443577c8d8917b6a11073b1174ca4922ec6fac222764b2f2e71737a5e730da453629d8d4eb865add8cd530bfe9045316abece0aa89d70d4" },
                { "cy", "2999bc74e9a5d97c08a2783ed866015d461c55c1df85f321ca425c4dd6237cf02a7beb5aafffcc0844e06ef07d997341c45a2967d18e32c611e0b79e33e5efbb" },
                { "da", "1f1386cf07b6c9edbbfe476b7911bfb88728494f6949daa2228e92425d7d90c75141cac03fab2b4ac06ec34f6c2da68a6473febedc5eddb7bd25101cc7946218" },
                { "de", "2954347dadf21857a2725682414740f28815b8561d853cde2dc77388a8afeaf5a4da79f9eef0da0061f4ac235342b99a76d78b6c6f728e29d27b1efa4ec7e766" },
                { "dsb", "32a3c4ca9adf1b775c7424c79de85c61c16f1338cb8cd4de59f666d64dd2581dd23b720abd00019d8f61b9fd676067bf3f041da5282db523e45ea6bc6e380e8e" },
                { "el", "33f19cb1b6dbd6a976e3cbd53414bd2863d1a57582959920b6198f7b94dff8bd52e50d6f55dba141554ff4472451f514f9710f301fe11e25daee88e3b67c8256" },
                { "en-CA", "9095844c85c6e093a7d1b0c113731b23d54d24e66c9b285df4a58c8788fe2a50bacd72ddc7036ec0649f9c09680388fea2002d8e909a88d278f577b3a1ffb970" },
                { "en-GB", "8157d196a44c63d6e74ebf1aeed5a80dcf51f4015c3ca8d9ba9bcc96fdb7e100eb853e5f0c764261e33ec220f3ffb8d50bbf0d9d2c5205e52de48cba1be342c0" },
                { "en-US", "ad6ef2f59577c632165ac6c10206b56c5bbee08b9de03f6dda401fe764fb4c55c48b7080c7d5c47c57a7d65b5ead6820cc190968687c3665d1c6d79988d9ef2e" },
                { "eo", "f4b65c41abd1e177f166bd61e18d0e4731049b7215b6929d165ca56d6c08b4b1c4efdcd0d57f289817efee124763159a81b15abfcac8be05b1a2536d57b42d93" },
                { "es-AR", "3bc29fcd592f81ec34bd731a5bdb5f1b641e02f7c3471c051d467ae75063ce67ceee19c0eb19a1ba43b6043c6ef750e356939301aab54ee491d07aa021263547" },
                { "es-CL", "4847fdb2a909e273f244fb3add94d3440bc3a71892f11eca780a4bfb2d9bb83b4929593370bd2052204d8a067fc4dba193734144656764662d94ed9dfef17d5c" },
                { "es-ES", "255dce403a787cd52da51ca73c2bf1d6a690f44d1385cdac364fa50b3aea1bf8108b836b96a88bad27dece9d17e3b16f32e3e9b3e976b0ec994e6820793090ba" },
                { "es-MX", "f694894253bc9370c8088b090fcbea1a0072cd0738756997d4efffe344341e33c59079e7cf40ba66779f0dda452ce6c98165d9935b2ba45de54c9127a3b22007" },
                { "et", "8fab8ea80252e19fd393de68cadcf83dfbf04a409cf127e26c88ab7e3cacf4b6610abf00b7c5e4fdc370cdd61de85d5f8f7c9414f18d8e3cf79b7f0ec3c63fa0" },
                { "eu", "1446d156c05e52487c262abd5bc616d962edf910ff0d541868dd36bea72625096857d518467dd1de42baebc78a12f638170fa2c8925ce3a97427c0770429ee38" },
                { "fa", "4d78c4b2eab622eb88f5886206266dbe6072c26edbf91899e5014f727cbfe5bbf191b5db1853847de11b1227f84b817387fdab3203236cbef5a0f0bb7a604597" },
                { "ff", "f3a3c18657feb587339d063885bf4cbe17715f745a6b9e911e3802602fbf2e5f83ffc6189b3125a5ebae830c7c8a23d1df0c28425514a80342c0b040bd2a76a2" },
                { "fi", "0f8f9bb5fb23d1791d5415e4e7a4130a994480ac7f00e04c50e259305014c21395f939717e656490a89c8bfc146b6d9825fc5632ac170678c0d003ce3f920539" },
                { "fr", "b1a415abef11bd29708e8de5c4bed18cd19324d6fb49e73d22fe97c9191fc988527a1454689442d8168c7bafb23002d6052f3cd4fad1e809501790c2af49ba1d" },
                { "fur", "80194465000878718c917b780925059c5be540d6cadc1624434f62dfbc7c1ecc6de082a73930567bc67e94f8e7a8933345762e83989dc89330ff746fe10f74ee" },
                { "fy-NL", "04f35fd8697bb62ee14bd4d9b8911015da42cf902ba193641dc27fe5b5b4ed9e5cf57f1f447b37e0def6c31cf61d1cce3e52d49114eed5c2ef5034fb0ed916b8" },
                { "ga-IE", "382012a7522b713c820d438d83a0dd366f5072c881ed31aaf6de4384022b9c181bd4b0d2c2df126533f9a6c1fa561bddbb42f2d6c8004024607e274e3a394710" },
                { "gd", "143fa0fb0dc9d77fa79b95667c019cf2f3c547dd03b6b554348d3af66ae517eb5e8911e68cded39fb96f90d29d2493042f37cae1cfb45fac180f5c36a9370f9a" },
                { "gl", "274ed7aba0422a1cb6d8aacbdd6025bd97b7cef521831be71e803b921d6db39b469b6d847bfb83a01f483162b53a2849f9bffbe3ecc6e1810c2b216f15d62b94" },
                { "gn", "f346fd0c4fbc8f9a58c3a6d65c2422cb677b8d4739b073509170087789deea0a3409728aeabb19a7721af9d23c076fe373319a35aa9abbb2cd255f373c6c8a73" },
                { "gu-IN", "7ef92dd126e6dc0d9c56d4c6789dead079a64fdbda2d25b84dd2d8578611238f4162812cc553a6611a4dae9fac85f041d921a751aef5be503a064ca555c3ccec" },
                { "he", "6ebff78dd3aa5f3bd37be26d9459d354f6fe6bb4f264f1ccfd41cbefd6d21576f2b35d30d522779a32661f74b4023bc0023de7e7f43d77e615cb7e9658b1016a" },
                { "hi-IN", "154382a69892ca06cef204afd98f91fcc18c9a56d2413d0a34f2aa71c687087d79de668ed712f672a0cbd5fdc6d66ff7dcb777bace358911b7fbdb4715452a6f" },
                { "hr", "910dd9dbef4487bc6874ac789a3be046ab79f28f4701154c5c921371f1d258ba8f27d70d36190c6dc1a2db1fd220210250a8094f529be0a2366268f1637f33ff" },
                { "hsb", "b0c8252e858c0107ecef06fb9e7a1c47b566e27f1422affe5ec5fe0b65e63378785c5218b7299a2a5564e2855699c8b09acd13e54667f29cf69af2245116f996" },
                { "hu", "999ab9b10576eb73c59042340f205db9a624f8cd1cde7de907447eebc89a87f77a6a0a0a2142e5ebd21968d4d0b7d9dc66cc3690994b681b218154376483dad9" },
                { "hy-AM", "bfbdba745512b1757560acb0db4a8e4d651b0a8c74638065734cb7629410f4f030be6449d35bee2da00482e5213642a4a4ff97cfe11c16493c8b8d2de2db1fb8" },
                { "ia", "d50a85e9a14bebcf3e3413d6466d5638e78ce05b32bff89ffd8fd089c5a0fe9ac14f8c7fddfaa13c48378ab9433da5a7b4c34b56f4cca4f8617a7f612aea2e5c" },
                { "id", "5f7425a538b98707f77517f5485a600610a392aa988ddfcc51fb138e11296df5a128ee083d55af938da895e2ac12b406ee0c62e4fc42bb14acf1577f45cb3c92" },
                { "is", "4ace7dc4776c3296c2a1185d5c1651a14d36fb02d214e80e1bae24cc01d37f263460deb58ba7f760d59a22315f0a2465c0d9468e9dff35975174c3c08231950e" },
                { "it", "cb83fe9b0d81012a664daac59edb77917017fa21a87859f37a3ab07019b8fc63343aae07c0bb39484ec011315a0123166cd0f7c5dc8a045463c5783e58986590" },
                { "ja", "6548b079c79989ac4e17c2ca7aafa5083c63abe0c3e72af992874faea296bac915884043bcca747438a790553fb3a6ae439e81f5ec9c34a2735fabb54f4026af" },
                { "ka", "2b10f57ce7b5b52190b07dc0e20a38514b501502979e018e9f3c0cbdc43655e2bf5276b63f6b9af7bb3f6d1691dddb5b8f4cf77da3df060b99f071e76696d4fe" },
                { "kab", "f478e2a7548819881384a57b99ba637d589346fcfaca9d820cde39ee818b036593bd8e1e6fdfc0b4c406abd16082d832a5a23f7b1dab26dc221b4537312250f4" },
                { "kk", "2a9e5f7e353756c370e9b14e94ebb395c138a22e45345c0779e66f5b7303341e1924dcdc9763f9ff44eb0bb3cd3a1cf61d23ab2c8602e10489a1903c81383fd4" },
                { "km", "7b91c4653d439a94cb07c17ea525d00c8623bc8f264b645bffaa82be6f158117c57bb61ee684e424e75c3205ac59ba2269dce9c897942efd270c9227e036ae81" },
                { "kn", "201dfe6ce0aa104abbabcd391997afcb7ff2a49c9c9086b44546a1042eff9687a7f096994d220a3460fbe9205249ad7169bbe880e3c4277bd5c95c7fbceedd4a" },
                { "ko", "95380971498a0b0bb44f96dfe6a76f61dce9dde88f31c4f3e857122e51c124573d9d70ee0110988e29429ef2cdb6850b1f31e4a974e1154e0e267c9bf7f5bb02" },
                { "lij", "1e0acb1edcadb74a4b08c9c7b977de8e9935be62f7001719a316470fc35a335029179873846aabcc3d6f259e8eca15222f0162322cc24034e9ff6f11a38c57d8" },
                { "lt", "c190842d21a08fe71d4e251a1fb2127fee4f0e88d32b29bd6b9cbe1c09831cbdd3168dec06100100577a653ff912d7e5d871a68546e9dd713d6dae8198234843" },
                { "lv", "5c6070aafe9f16b9e225c36ba98203164998c112be65b2c902d0a1a2fa7cd60369f149192e1a14aa429a26eea1638eb8efad3f5bb3e1c5163f081283b59d007d" },
                { "mk", "e490f237d85fda5662f58b3fcfaf3eeb1ae847f60b5f51d5c94296c758ca72bc093a87b1c8e5c1f2f10dcb8f80c3ad4019d9fe0a1ee52762b746716eaf9a2449" },
                { "mr", "57500bf9e74c9901620369090405228a9555a77d2d8f901b643fd0ecc83ce58451d6f0b4c5e0bb07c86dfff7b925a8ba43f11ca6bfda1128a9c4ed266f5f1178" },
                { "ms", "01027dfda8a28163320c535b51deac8d7c004aacd0d0a6b13b3afcca4a042ce76f38028bce4f9175ab333f546a2e6395dd1c908f324ac6721999b2b818bba91b" },
                { "my", "52ea9eb92473d857762dd0863cf43b320b116900021774ca387b904230d11205f24045bce4f631e93bdad9969674e89ab93f09a447c581dc5ae6f9a93bd5ca04" },
                { "nb-NO", "19bcdd3ceedb209e65f8c4b72168fd088aa1d6ac796624f994e7f77f77525571d1d1bd973634f8bdfb03d6a5ed88280584dd6b19ec319c10ffa07f6e819984a1" },
                { "ne-NP", "439e6b91e6180c50282c60c35ea3a6ae5edcfab9370a9d0f614abd9d6c5d58d9ec573a1481da00be5d142d78122c9e607b08b37fb8a6a4887a8b48841fd9d9aa" },
                { "nl", "4aa29fa2be1e1c2f91ccbc111418b118d71b7cd8e035fb0b573aed5e2c8226968455be5195b997b2a092a81eba018c3d9855810f8afbe21d0982c0b40344854c" },
                { "nn-NO", "409df406de0a355a0e605a59536a75f938086c0dc46373c234ed9155f18942080b7974822fb14a195e30c8bb18ba43fc163a0e705c1a9edb1a9b763e534124ac" },
                { "oc", "fd70d6d048a4210ff1c55ff027d6fa7f97554b3b5bcba87dd6fa8e99df709f75fa25940b213d5395e790bf4393e8b986caadad9f744788f93cd686ebe442279f" },
                { "pa-IN", "fad8d9229561751608dcde4290f6f9483bd9fc999eaafc461dd1e84ef3f49e147adcf52bfee9ee1d82b3d0873e7a3090fc557e956f6dd06364808879cf8c93dc" },
                { "pl", "fb30828e9142a8d6789efc0a900ba78a04623922bf44080fa7c8d2a05934a787568ff28f6f6dcef6be68082632688e368ffbcb347d973ad1a8a631a8ec75b9c6" },
                { "pt-BR", "a48856091b0036a90505df40341dab41626341bc66217f14f67c8917ffd7ede991aa4d20361d66cc0f543bc459e090d30c93a0b4d18926b92067b6243ad2ded7" },
                { "pt-PT", "f279dc45304a68aaa3e0a2d8823902b5072a7be0d254b50877007ecbd7e5b027dd9d133095489be081eb243a20ef0a21dc2211d4cc3249de2f92ac0ced7587ee" },
                { "rm", "0f61a8c4544b73f0771fcc549d6225f1f6d05fe6821d87a6f0678ae446633a4752b3f4e372d1f12f02ced9f7afa2f5a0d136ea729fbc092a41234df6f0377912" },
                { "ro", "62e64744aa8dafc4aad2882e7c8938fb5a1bd33b99ce97d63f645e6cb9f841a20495a060ce60374811feb497884d2df0795c5e4120988529d2929fd7ef4c270d" },
                { "ru", "1c5efaf01c2ccdb39c33433ea9390ced0be73ad3a0612480efb452b171de0fa2b19a68fbb9f604659a4186feb61ff95d7c7e1fdbce724197515c72fd02f9e46b" },
                { "sc", "556c6f2ccd11ed44d387160106c49f2f8760232b54ddbee1b75d2919e5b7938f5ef858dbd1492cd142ab9782be6b4b741471b460bdd489a6a6e5992a63cb6c03" },
                { "sco", "baf8ff8362eb4db930e006ffee5f2512d037964968732166bb2ace95f5dce973323bc76c46af4a4157d4252afe518490d5efa942ce84c226dec077802a5a68e7" },
                { "si", "ad7c422487ddb3c16fd8d197b835fd79c6bde35fdca78fc2f78bcbb38f07ad805a54d9cbf3afc4bdfe5d07b8640755a670aac9594ed008a532de37be1d70e333" },
                { "sk", "a9b016f7c14b82e6313db9226e43ea772501633799efc6ca9d5ee892c95a429d8294dc93fe4f1a8c0d01b98938eaa3de74982971233ca8c1e53be717b051e3a0" },
                { "sl", "cb16ce2d4918eb9220629b189ddf88a419b05a2fc6ca93163d1a7c22adc1e07a481a32e2e8814247d6873e2b6d70ce982d5bb67046accb47cdf69cfaad0c2a57" },
                { "son", "0536a8c8f6e1c739bf45dc7a4ec8f1c407f99ada48bdccff585f8f480d2955afa5afa8867551fca653d359527bc5e4542d56f68c88bb7d4720ec187cae4cf69a" },
                { "sq", "f5ae5d0e2116fc5f47776ceade37fc998bf35a427f6770a64dc07b1153fa6b4d8deb4036f0419d09eff633330d2120b675fb1fc5dba439650b47dca3da6ad575" },
                { "sr", "dc57722ad7235ca448ac16ece4d452304dc5263d734fdd1b23956bfb311ed897871586282b47d75063a64d9f894ec489ab1163dc58fbaed56dc8146afea888d6" },
                { "sv-SE", "62151165f7afd6d57e5a1f6d4119d1064ad558167eff895ed6f3ac787c40d1f07ed7fcc2caa880b19756136a70c94b9442df8425cb6393a9f2a20a427d887912" },
                { "szl", "1fb28507d6731bfcaca76c48cec0ed1a77a07c1ba80eaa02c6114ed71e8952b642d70355ec7e94cb6e7087c5f27a84811a07510a0dc3fee9791d4de1e3beba48" },
                { "ta", "b3afb44453f25cb7aa0cc9d7a8193a556c3525898ef24d91056136992cd4e03f1bf9f451e9c543bc6f13ff2df8914ccda0698aff99bbddeb525507bbc4d1855b" },
                { "te", "198a84234dfaa1f570e400e8dcf162f59f4f1dde7621507fec47fc582431bb028c350311bd2924903eac3c791a17ad01ecfb12c869739304543076a276068c38" },
                { "tg", "eb6eaae4ea257be50982915cc04afeb0f1a23f31c7ced40b4dbe758e2a863aad3408ff9bce39d193a83f8b5cf3a50d7424854e2cbab7b10e21db82c598a5729f" },
                { "th", "5ea896f491ab0e5c43f92ab9b872f270ba1a34a77b3ea9f2facab282035cede67826363f3a78c699ea68589b2fd2578959ead80c284105c8fe1b64af06675768" },
                { "tl", "37991072cc110f3edc02df81df8e271645db0ea130619be725ddbc66949cbbbabbccd7af7665c4240518d4026b09250c9393649c423bc8528f76dec11edd04eb" },
                { "tr", "ddf65f94dd5840793452e6bb0126b4988bbbdae8f7886f903967d3d43c420946e97b22306160e61f9646651d6ec6f8560ee037a08bfef58120dda2e0b01c6d8b" },
                { "trs", "4dcf82d71d08de116182f8cac0d3a01fc8081115067954ee6ff84acf8eb96589bf4dc7d96d297a9bda6c17b66814c1a419e01c76650db5caa1ac0721e3edfe5e" },
                { "uk", "55b437ba583d4c3e31067f622fb856df167fedf1ccaee74b6ffb3d529df4e115ed39c6fd2dcbe396e737dbf851357b97e31baf558aceb0460f023184d9ca13df" },
                { "ur", "297c8179e27ccfa26548459e50d6fa2a37bb71a90c39ac3383f71ab8166ad46a6d069807ce27d67df68832de9eb17132c19d365218ed5b65f41181218e5f6564" },
                { "uz", "0d67e0dd07c16b7100f509ef0852988719781ca2dab474967a02ff214f115242d75f4cddf9d6585ff76090c95d265ed2d2e55df9e87c293d9a3598c8a2294774" },
                { "vi", "b696f0d6d107b798c3479b0e3f480e334df052c196122b494f5a521e7cb78376745fe530801976264fc84ac161d12b17156e20ae29ad7f53bdbbebd97fbe5283" },
                { "xh", "540c229713236fed65209a77c4371da9f5915dcd62656c1234b8fa921e1273e9371f84a672c9dbdc6accdbea5451c4b76de1adc3a3a034412412f587a0a4dc11" },
                { "zh-CN", "48d9846d4e4ce989a3b625ffb0425c0da372baf704be0ecf836a87ad2ee935589dedf0ef25a61b7b62e8d92668a2f45826cd4b318a0326dda306b3d977a2b415" },
                { "zh-TW", "241f32e314be9c5a70a87e74d0e67401b43f7fe177f94286bb23862b3cf55c1202cf0c14aa2fe87418ef87552e002afd18f540ec9351ddab0539ad2148846ed2" }
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
