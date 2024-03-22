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
        private const string currentVersion = "125.0b3";

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
            // https://ftp.mozilla.org/pub/devedition/releases/125.0b3/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "c9cd9c7bab86d85a0aa12b7aca885ad46de47373cc4ffae517cadeb8e4e0289de97ab473634c51f540fbe3ff049881a981a890fe53831f45ee00379f8e568bc6" },
                { "af", "b4dff72de7bafb91c9a1158ed12a7ed47f783efa80087d15405cf764d7755cf77950c2840abbacbbff17b230549d61bc537083286773adc40e55b68cb24e4a5e" },
                { "an", "2f1d099fdc449bfdac69e5483b7cb93846d21b38b769e5f14f1fb93d9b9e8ba1fa1f3b1e0bf6ff9ad9bfd2ecbeed09daa5690a84d17fb072cec2de5e9ea6470b" },
                { "ar", "26ff25f28447ad809be4e93d2e4a5e6be5e2026691ead0015cce2f864416d8b143ef6cde7ee15a06c884f68a0c0892e2705d08470ff5a4c915a54d89698605a4" },
                { "ast", "60e63a6c8f47fba4804f83dec7cd8f7aa25e2d4546d27ecb5b0c5aafe431b37dff05c31bfee942ac6f3c3b63dad01bad0b4fc070e4cf5ea8ef6b9692738bb113" },
                { "az", "883ace17741b5487a4417a5584710abc1d514a56d2f695535c0bd68ff86e382fb32579c6dd3a8f9c9791b43bd9644a1177d0c2d4ed85850547ed19f833104949" },
                { "be", "7a22fb962668b867163f3fda9aedbd46132f05d6fd475e27ed8cf87aa3cc507465c969613899fedfee3e3515f17755eae36b633afea6e879470b1508f5f022b3" },
                { "bg", "4b38d3abf58c301c743b71a79a76a0aa36232da0e41dcbd42da03dd9f39ccce62af9800d4e9c4c0840e2f3d06b496918fed877015983adf9c3efed909eedebea" },
                { "bn", "a95bd5cad797c526cd723794212cf889a0a990c00acd2a7477a3a03aa0d57ba7664bfe341dfb5c9110611e7db2492fcdea4a2b12cc2ca7ce9be32d5e15946dee" },
                { "br", "d603ff1b7119db64a1e25d53c6e015669628d1e81e38d134a470d353df8326bddf79887d39b476596c6c8ddb67e8c6595b7848a8070f39be329e16a27a97a415" },
                { "bs", "e4e4319ce2d8d0fd416d0e88179c86139381993c40b0b1532227df62fec0121f3c3558ff951592ab7a5ffd5ea3f853b3803cba3202b3aeb923db6b802ae28093" },
                { "ca", "914b0f0a136f594ff3da5c8f0c2cc4a4dd05ae96abe51bad6cd2e327e262279ed9fd1a76874fad0c276e90611bdb59167c193d1dbbbbb8e3563ec12f2a74f262" },
                { "cak", "f0d37275e952c9db820de8d8203a129fb09693def6bc1aa73c6456f4157e269f6e182fcc175fff8a5242220fbba7c2d1a38ef30b870d6e08867b57d24990d066" },
                { "cs", "345034c05a174a7f5d2ea889d28e4dcf3d3fb7262e029729ccaa3b06fd06e648ab70df581915dec096826d7cc4d4e8531a5a8660624a3af06f876345b8d42942" },
                { "cy", "407598d8a000689c61d2456780b6734ec1860599b03f04328e524825ae137b742fabe25c99d8da49a196756881384b9a893cc9ad82015fb2ed4a2e5fc647ba9b" },
                { "da", "060690d36d7d0f9c9d9eaedb4c836d07f28be240de561af2f9985120585761f8bafa1dc9e1f18534dd079b9544f05e54e9736add07fd07ce4b5ce3956fc7669b" },
                { "de", "02485a79b4f8098b36bb2a7025a0303f8e3c2d4d59e05ce9b2e628c18d0337f7b5e6d6324053fde2f38e03121318b669c67cd1461b26b4ee6ba346fd17598e52" },
                { "dsb", "d9cc7e128317bc34238bee652eb5aad30e37d7555ff0f7e210bc7f7d8d1f70300f77f2fa14bee1c881dca87b339cfc9f431e53d63bb8573666bac655913081fc" },
                { "el", "2b99a0e1d774a7aa868de9032e57bbbe0f37681a2d10793a81aed195c2f4ba816a91db6f21bc13553b08d6c29f2d8342acacbcd75f17ba3c4a2914360f1a45d2" },
                { "en-CA", "b920e0c0304ccae369e4556d061d6c7a475a356f3d27e843cbcfd3ee97bfd8a795ed2efe051935939d802de8ea6fb950e8e1cc125d8ea16eb11491bb03daba42" },
                { "en-GB", "817381f894f70092353b2be62c484b59bd64c94f829ee7bb70ddbdb8c0c51246abd1a28324442d16bd4d348c5724baf5d1cbdd2f7077757ba845242cbdca5734" },
                { "en-US", "c29062a8b4052584830de99d5ad8d07dcfb70ee9c9395a1573b81ca060c3caeadf14c2e18f3868c73676bf929b45a61aed83f854afc5731cedac774d59ddb065" },
                { "eo", "b55c1bbc671a3f37a76c42dc79705cd97ff58c2afc50dd47ea683fe91cbea04f6d666ca6d46c30c8c454a5dc503bcfcfb7af31eeb5873c880e1ca16f7263ee2c" },
                { "es-AR", "cae9111d848238c20759fad55f69a0ac640e48b3367349711a71ecb112119ef5855034660f3c9f0502a3cafd3d3be639eee88551b22804f75fb553b715db3fd1" },
                { "es-CL", "73cf4bc54030f7f9b10b79a250d1f02841aa6c0fe7ced8f549e7c18b34f3cc25f8c9958f7eddbb943829b16e7806326dbc620e80adf4b61899509dcc7f7d2b7c" },
                { "es-ES", "e49fbc5c0ba193c10f771f7925a659fd901179997db3eeac5a25aa7834c26568266b9ff91e59b165d587318795349d63871556cce200bc462aa1c86a2a2abc1f" },
                { "es-MX", "a395a721f1ecae61b2a4d9adc8877130f774955aa1f50317cda7eff1bfb18d76898e01b163b0f5663d7808b0e7bbc3f5274026123fd80d8dcfe7da1e811fa2d0" },
                { "et", "f4c855e50b015d74c6265b18c5bf629e84f877366c0c57871ed860c09ced6fe5d1ec7de452d886e9365e322d36146cb4385a3a3e259e980d850f64e361169cc8" },
                { "eu", "38f6f06383900b92720cdc68ef7ed96c2fe50db0cbc2292d2e68eb7902f29aedeaa8c33594d200e93c662828ae125464920cae7d5b07ac0ebd39cd743bda3e2b" },
                { "fa", "bc08f0a2c52a27b2d037b4b7a1c97e8224b55cfde53758325aa93b71ff7449505672517be5b5dd41848970782244e0185f53fcf84aca222a0179821558054380" },
                { "ff", "f0884a9f26d9f340d770e298ba796d58f80cb7abb16aa552afa25bdb96d6730610aa6ed0ba855ca7bd569ce4a84a5109106927a7e001e39d1f1a02a621563b11" },
                { "fi", "2beaed5a9b054c619a3cb734004a814e168cbf44a8f43effa420c4d710cb961f8482a93353c80543b3edb5f5b6f2e7a9bae4cc25fb7a6fc4dc7bf01b22f001e7" },
                { "fr", "3cc200ae62cd7a832aea86eac9a4f51ee5c563156c01958718adcdd334a2a8ec0740f4ede80295da3ac5b9fadf68ae3c66846fcae610d72cb96cc1bdc1ccca6a" },
                { "fur", "878c086cdd72b05707d71b27c0fcd3f2fd910729d15a74f179de3fc8df9ff60d207544c787b80689041024494159e632068d3ee9b302f8dd204cf115d4c3c330" },
                { "fy-NL", "f4716f10af2500fe5c2e50ed3aabd59343b52af399d6c02676be528cb7eefd97256f94917c62d83e8da41bdf8b9074d6834763badf6a23dace0406d227b759b5" },
                { "ga-IE", "d2837e07f1ee0d6f49d6d69b2e409da667c0a18b5e1c2990ac06f3c62ef0156d7a95237cae987f987880b1756235a2f14bbe96d29207c77ff6ed642024cdd530" },
                { "gd", "f3cf96816eaec1a8822217c4b94563008bb19fb0d51de54ffe50624661fea597845abad79ef433844fcbb936ea5d50d7356af87fcb1065af4e81fb6403031e07" },
                { "gl", "d6c106cd45d6b0e134462f87486e30729758f029c4ef95739d4913e3e3bc42c4ac25c37b5249a5b0934309deadc67602fd5254c95911771e19030849679b02fb" },
                { "gn", "148a99aac80b05cde1897465a27f78c74efabb67401725a7a5042938d3420474d1456e91f7961c7a39a38663f8239e7aabf45a423c7a7b2ecd412162e469a8be" },
                { "gu-IN", "e5ac60a87f9a3504d95f1b1cf77593801e01aab260522b4bff8027b864e99a81c1506592ab6f4024493d03b920419bf2636d6b4e4ab260d9035ff03598f2f5b5" },
                { "he", "a35b08dbf6327d03cecc537ba337bcc40f0229fe0163ebf4dd8e6751078ca26bd80e5e823cf6602759174361f5e4d7e861187a6ec7dd00c35817a474568f35fe" },
                { "hi-IN", "d5ba5af9439c0e2b697d19b156242457789e4593fa2630811d6faf2269777fe75187dbec84be798ffa06aba965c447bae08c903145540723148910896bdcbb9e" },
                { "hr", "a79e02de976d15c986ef5ba8d4c013196b1a937b35531b56ef286c7beb6843286917d75d6bc74bd0bb3540c0a478ee8535729e2e9dde0b5f6ec387556a968424" },
                { "hsb", "7fae0cbc51dfc465c32a63bcaec10b1cd59ab24dd3fba62e7bde32786ba890177dbc2b0118e71056606503115259fbbb7420761a1540da60033d2ba939c7bf38" },
                { "hu", "7a5f6cdc17cd966bf4533d6fd5bb0e5d18c2fe9efc96b1f6adaa075624c83b8ab593ab78b8f3e99f97342cacf8f749d697a9dc86a9d523b5c31a0bf034175e60" },
                { "hy-AM", "a00c0dd6a0260579762b8ce54a9ffed5ceec15bd3be61814cc5ecb28561c431b39042dcc814a3f83288220cd577ee0fd7494e3a3625e43ca3a85959781a1ee9f" },
                { "ia", "3f3f517edfdb19ccc5a1ae6089feca5f2048d08b9a3fd450d31cd3d2244a8045d14f04f9eccdfdf9ca6a0321774c64d53946872e7c333dd2bc2c98d40c742a08" },
                { "id", "02d93a80e79a2d4a8f6ef59e834f489cc4c9c250b49493a8d1d5917f8856d8623c408f99faebd18b095c1a90ed5298ba099131b2c8ce60d1f749d4b4a2aee1c0" },
                { "is", "ccc2a6e5f484fe2b3627a3e89029aebba6955a42aa82c519b69f6785496356a6fda8d8a43cc8b3d9eb26725de88913cd14c0f38ab839c8f7cbc169eeeb9e629a" },
                { "it", "b9508f2f3679b57d59628441ebed1f1b25de31c81eeb3ac9dcbe46226027a5589386b8dbd19a3aa4a3f378e6bfe56a53fd267bc344c7c6187c69710e0bda1e28" },
                { "ja", "fa5f1006606db97955c56e7756e09561144c9579cf763bf0b3145fbf93e1dbe9a6ec5ece2c1f363536ee22b45c5f9dfc652aa04af1cfa5a12b260ac8d775499d" },
                { "ka", "e7243e4f1a07b497c9d07bded1f0f09c798665e4bf6783fdb1c287e6aeca4ae58ef658eb320094a14b8859e6e3e2d9704c33b7b622dada0ae679aa79ef678634" },
                { "kab", "ca307f4b2a1f1003eee3ff3303ce6a76ea485b519d24053ae4c84b0873699bce64a400fa19dfb4d94066440290590e4c72a09ec4cd76680eec6193ce5cbfef27" },
                { "kk", "d52b8f19960cee6dec23febb4040b3e9cac7fd1d34eede0619082bde4bcf4898aaeb2da686339d5fcd35b6e7229ce43b6908a2ffe9983d8586b1d666927b5ac8" },
                { "km", "4b2cc2f2a009895deac3622ff1ce8ef67c99c87461fde8cccf5a9531784c08abcaeab1bd589b0d4224045b1e1083fd2c46fb13acfa09cdf75c3b67e3d911d05c" },
                { "kn", "13e2c2dd34c796ed3a394c68b3b58aa2562b9bad3eb2bff114aaf2c02647f24de39559db86b7b88d6c5d2a8a865887bf60a4b1329f0604d9a7c3066e21160818" },
                { "ko", "fc608ddbe2727dc6fcdc6736543874df4971298054194e58edc82850aa6ffd849a9c13a66d0a098bc76641fe19184effa81204d14bc6a5cbeb834d800d7ab462" },
                { "lij", "cfa83b66725629ff3731029c0d25b98ec91a8e8230e3d7b2bd818848600d808dcd021937c50f5fb1baa2d89b4a4fe448d14f7ab138783c359314a3f6b30c4081" },
                { "lt", "7496e86f21ba065686a1ba8a9980e9624969bae6ad618da658aff3846eff520f7e27662b575067d01350c1f9b63b9851eac096b0a274c2eaf528f5b0f9b1be66" },
                { "lv", "fb3522f1a53c30f1eaf4460fbbfa38c0a7d669990e1b5ffde07e85b6ca2bdba8e1acd29a6d9b763fa203872f9054618cf578673c318fb12f67502c0289bc677f" },
                { "mk", "b7aeaa71558c06760d7555eb6d54833f0fe8c4c0e6a3ad516ef4a967964b490c278b2a636c4df66f7747fbe5ff1011651bafee938bea52f72c6986f9c94903bc" },
                { "mr", "e0408f1964a4b489060dd5e0057a52d7cd1909ff826304960068a968899ea6890fc6df16d6fe546b2445fd8ac71967f851535f75c2835862799bedb87d24a68d" },
                { "ms", "26e814c16edaf18d05c8949bb0d8d792550befa9129f4bec90e5594ec82f9564b9e15d4e0bef56077908983a97d6c154ec9987d1724ee1fa3a725339690dbe3c" },
                { "my", "7b317ba046f1ab0f0b0cdb4840ba598c807dbddb9b6005f7fdc139fb22ef8b7067d2a4a56597a1dc587094706dbbabbb7c243530874ff45d261bd6198bec2238" },
                { "nb-NO", "57063e80602c0b67b96de65898cf3be896292380e20bf1847281cbd75f4deaa3be89241d26ce5bd84c0306ede923aa6d140630a1b43facff72fff4be42722748" },
                { "ne-NP", "a3d0699aa41a83561597668265ea0865b6974bafa0b881a67299f489d610457e4b7502391de29f81550b7422ed77692b862f2963f85e3eccdac87d989d53ad96" },
                { "nl", "aa075ebaf41154f413bb258d51a88e361ccde3c2588a1b2274d19442c162df276fdde490b87703f7207cdf4d9813612509af72b97fea6e01ab98f930466c4dd1" },
                { "nn-NO", "4a8a6a961b0efd0296d6feb471dec39ab5dbecdaedddc7d6f2f3f0b10a351c19abccb5b0cd207d46182f6f66cb582474fadb7be9b84adcbd4d86afd7bb3178a3" },
                { "oc", "e886f72edf5f31342bb0e0af2421bfd569faf90a746bfa6bdd14b8aff3ea499422d0b19541324d9cd6910adb606d958ca686bfa5a01b84755851872daaa9186b" },
                { "pa-IN", "0c62614ba5ff938cf197c6c6790e21a9aada40d721dc197a29c54a31ce8d8671ee82d2de40da1668cb8775ae6cb30618247836f94440ec1abdd047f4c159e309" },
                { "pl", "9ee790bf1181e4048ecf6c8b58fb32d3225e5722ae3a5258997edac3c1889d6bf20db51cbb7561533a939f60c2a328d0fcfee7fa946456f92c27870b2e0bd72b" },
                { "pt-BR", "69d7e25750688b8a35574d4aff4da9ebfb3f32f7946677e3f854fa45b6a3035dcdd1635ad534fab9a023dc6a024db4cd8881130f421a4ad7cebb16db76305c86" },
                { "pt-PT", "88ceb86beb6e599767c8e8f1aba27d29ae1ac766ec487809ac7a4d363b46068a30cc6388ef934263fd76a9a1cdf417894f0c4fe62fb86def49961a75058898ad" },
                { "rm", "83fcb180a4df726906a9b3c615e31740357818c58f8907bb779260d2ed792b30306fe1ea78db1b5259fb8418286526d9e8e2dbe4f1198ed156be0a14b165b76c" },
                { "ro", "ac590f718ac7dffc04c5aa4dce8ecfba4877354636926e8b2fe326f1c7a0e7f4a676609e0a917b0e0e7c9d2474612bafee7bbe599c3b26fb9ce65c9a1c821c5a" },
                { "ru", "bc553934028fa08bcac5e4dd7c8cf5a2b85658d527626ee3a3c3a7445c266b057fc27eafc0f42d4964263a23755892b7f5da1803149645be0d8782299c29881d" },
                { "sat", "d72a18d7e9ebf22483303e5af599287938c0101a4f71a24f5bcbee5e207a87287d5703a37af606337c7bd8d7cdef1abbdfa722753501a23a465fe9492e5cbcc0" },
                { "sc", "9cb73f6106eb52194d59d31be4955b6cee8fcbd95ab7647b6ec2ecc966628ac2a9af6d2e0d5c5fd59497980b481cd7519683a293436abb670d426a2711982733" },
                { "sco", "df2f61496f316775eb70d1facc3d1561445b15ec44800d5a4cacd4e329b0dc79b1dfafccd777cbd4585c2fbe26684897695eff75bbdf385445d1bbe6cb4c6f7a" },
                { "si", "26c19771a2e46d520fb4a085950accbbdb682923562481b11050526be98cb7dec5481ce8be459b9ef4f0dde2474869b771a02e148c749a88d7284f1a5b74580c" },
                { "sk", "586a967fec61acd5d4a37d8786fb3076f943c4bf77b07f3335fb9217f9b48531dec05c0ca80ad29dd549d9019a6bdf82e01f372008733c796635cd36c96577c4" },
                { "sl", "784e3db58d70845771eede207ba5cb11f27d21226aa0da6db5fca5930e3901f5c5ebc975159126f3268fd3f5efd640be49664009baaceb726ea51ef4ee1db6a1" },
                { "son", "6127d16741e4d967784316e92fbc0579d2c6eebfe3103946426cc499b2515228c110a73536fe4db5caf95bfa5f3d4213258a7a2c3a0f3007f32a11989e475f29" },
                { "sq", "8fe4f09f748d725a7b7b3d19e0ca2509893f6fb0ec83838abb2b09a6874ed286823ffa61b919da910cfed2a303fdb2ef29e3f4ee240df8920798bb861a79969a" },
                { "sr", "49cdf7421b05a7bf1f773153a08f849421d5c20650c9e6e17488acc9b461961476cc10ab7639a55d55f7996182497ab86c76f0a6ef22ad04092879c829458e1c" },
                { "sv-SE", "898415eed6c4a38afda61031c478fc65231efc846062230df96cc28ea3031888e6dd76be3fce47164679009b0508fb2474fcd5df2d8f77a74f6b896838a556c8" },
                { "szl", "4549f9a302f02516a8b1a5157bd2af4b18595b6ee2321cab94eb2ac4a841dccdb2181efbf600d0c7c6a95e281b26bcfb6afddf0a4a3647bc6d06095d715acb05" },
                { "ta", "ef327b5fc3ca301e5736e1427d18e54cdaec472ee20c4ab46cbd153286f23cc54a4d712101ecc580c82c9d56406902c7e7cb37acffd45a0d03ecf6b9b48ebb30" },
                { "te", "f384a45fc61c64c7d598d88b539d6b84ffcc21c0b1cc2f2c7611b30f38929c3abff751b5595e757d89da221de1b985608c886d85a3fa85314f94368a180ab313" },
                { "tg", "cb361e19126415eb55fd2b689866988496059bcf9e9710bccb9d25af58d16ec16504e39bb6a06cf9eec4563822b8e8598a1e877392acc7d2b44528507d71ef4b" },
                { "th", "b6792f1e6d9256253b6682bc3a9c26dd3de84e184278b32cda3478f72b61241f5b164d21f75e278d162f1b33255f47dc634145432eed1a6f7d15f28d75f8a912" },
                { "tl", "eabf47a5754944fa36ce868614d45d5c92d228a23f9f999c40fe2e31afbac97a47441d6b063108e2c5533243334a278de00dcb3ca0e7f3ba8c24e7e1c3c3a6a9" },
                { "tr", "7d025323e4153eb1bbbb9918d0191a81991c825106da296e093c33602395331f758f10426c00a79bc4439b85b3d4f0575e4a35f4153389ec84b1c39d0c40c8a1" },
                { "trs", "e664c9620cbb705e84d906aec784964972fa41d98437b45b4265de8635c554eb03d0783489c7e4afbb54c6f67dd3d8dac740db4de9c0c2b3335870839d098248" },
                { "uk", "08ef31b3bca743233142c4b0eaccb29161e05e4dc04a8e547228eee90b697ee0cfbd2579423ccf6116cf22808ab2d34795c7fdd7bd4edfcba4ce764535b36634" },
                { "ur", "816dd395575eb157b7896b249a061f9cc397da13029e22c750b2383c8e65cba2faa5c63ac404ffacca7b82d9d4f11ff7748c684587d1f0ae262f10bbc3cf366e" },
                { "uz", "be0a320a3b6f643a349fa9fdf02f3335c4966e87276a7794e1c0b86bc3d0e89f6fe72fb9c6ee4daea7ca8ede8efd5eeb6ddc8b0ea0da8878c6d854dc2d667f4c" },
                { "vi", "81c2f0897237bf1d98c4469159c38dc13a3bc6ce228b7941b4a804e5e96605020faa5cef475f2ac9fcf8a1473a7c618f064c810c61aa93d0d24e06a35607ab86" },
                { "xh", "86361895864aebabe86045820fbb5a984cec97d3e5c032c1ec52c9f33d8b6cb396efcf18322f6044626679fa4e403d18925e9e516874d9c6aeb7f3d9b749c4d3" },
                { "zh-CN", "9222f11cdd940a54e8ee5c6d82d532b66bf9430d3b132d9bcc2bc5cd68d7352874dd48d254554b6ceeb82759a359775b7e1e99da5ef21d0662f6d05a7c0a4483" },
                { "zh-TW", "8caf6b8f3095160110380a99c4a2625c5f70051342de034675765e8efeae9ac7d69a0b1f339e633745959640ad8b33d475a0453848593eb00a1ddad10177b392" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/125.0b3/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "d67b69d6b8090804fc769f7f67248bab7cfe40d606b02eb817f2fa5e9d6c442f40c4466b3f3a185ddc3c300dede85f63db7378302b549c709e20da5ebfb00fe4" },
                { "af", "a0617fc07f8a8498a7757678443574911d95d2589e89c3947efbc6a8577adfefcc25057e63d53a6a07e1bc224247e2f8c8dfc49ecfa7866ddfc035ff3487760f" },
                { "an", "9c9c15dea12e420adcec9e9af7525d78078082d42e820ec70173b0405b44c2b985a30658517f20ed908ba70a51f7bed452c34230e4fccc5319483b88ed1cfd10" },
                { "ar", "5a9ee2042f646602fa3cfab6c9c95add4bfce679a85fbd511caa87b6801ec1b53d124c1e9e88a98725f0e5d3281a55cd822cd32af66e0668db16c236288140b6" },
                { "ast", "8064401a64c8637b1365c72872c9a4ef4d049d3f7d4e1c39e522b63a1220a80344f041b9c181ea17b42925448338e34ece9c4a7f1f55b253cf94cd45e853bd09" },
                { "az", "d6de5a7797ded7831a9ef269febaf16f0f3ec7592adbf5b9e186c73557276225252c3ddfdc5795a8d6bff89a103916ae54e08e5116dd66e8d74972b12f92968e" },
                { "be", "eb703b08a8a18492657f22189c59856c5c5889fc397287067bbd3a7acb6ac163bf3965c66cc36dbefbddd71ec34f4e30b23d72cdafaa828aff0ef7ae3566b7a1" },
                { "bg", "7e32bb9aba0bbde8572693d5e538d307da8f05fa1530267b0f1e7042e8699c024e4cb8f5c4e70de6e3253aa262b1120dff91c9082f338aa83bf3ccaa361da81a" },
                { "bn", "3dcf3d7bd14fe230ef40a6983f4d6d229b524bb52979684395de9658f395c76e46bbf317f58050a28a0acbe5571fa4e2cb29b7b62be53ee9e1e6bfe26233202f" },
                { "br", "ddef3ce8de68b0f8fec35900580c457042a01b881efce9b0081a743c43214e24be158768705bf0869d06149dfbee3f6690b20ee39e8c054e488879a6d89e9dc1" },
                { "bs", "c06cc7793483baa5922e7db708981efc5208c1a704eb03a3a4c0b2940dd7e328212af9963c852668244b160a2e2101d62a8fb12ca30ef7413d84f94f6fc54711" },
                { "ca", "701fc62edd6ff2bed7ce8405de8d115995dfa849ffa1b0c7159de2e2e0501e015ce52394a9bb7f974aa811da25ce3b3ccf2471e026cfd7946fa91a7f46b8981f" },
                { "cak", "91301698e45950ce8028c626fa70054be5d0e52ab8dacd5180ec67dba429369d3223df51fd5063c0ba0393099f26739689abec7b2e2ca15867d2eb11f8241c8a" },
                { "cs", "8cc324ae7a10e6aac8d9ca639540407404f8fc1e4df0f2a5d2841bf85c597c6f7c91845b58ac53dfbe402e7111f1308b558e3836c45af8c39cd176961a0c2c66" },
                { "cy", "7cc9ca06ead5ab14b3d7458749282de53e8227267db087a931027efa15d4cd2488774fb48e922a723cc0b6f701d622c4e0c8abe9b90a9da89a29b1e7e043ed41" },
                { "da", "5e7932f99b9321f6428209c8cef9fbb60eb30664a4b4b5fe43cf2478fc61dc9e84fb4c414a16d4c38f1403ce96017e59f2d63c6fdd9d926f3694f1e50d11b7bb" },
                { "de", "a1c6f44659f044524568c137933e2e89f60f0eb3d2b8d60c73a2169e36eda2843f4b24d070c8b1baa950f9a694a2fe0471ee35f5d27de2f4b7e9ed82711a0585" },
                { "dsb", "a4de350acfaec7610a0b26cbb4f9b3810cf7f23cf7848f4066be0b1763ad60af0aa3450ef97df0839bc56d8c68d3d4e56fe33e0f83cf60ce50c8ab4954d549a0" },
                { "el", "50ae44ff098f3c9cadce44480e95b46b1da4081e5702f3bb206d16e408767df7f1b4b6f4b99844b27f02d28b1b4293e945e177ad5c4c95cea17e6d9d90d2350b" },
                { "en-CA", "58a730f7051e3128d72c38ec1a5f3b54337038d412929424ce9136a2b03078a1e035eb31caf48cd49c2c2ed8d8e7d5851278cbb9f4bfa50906f50842590ab30e" },
                { "en-GB", "aca090b8d2ddb468e07214ae876f50a6f35a6e9ec58f396eb29ed40645d00537e41fa6c7f7915f7e59d0f453f601a4c93f13458b115eb35fca1f4f5f73cf1b54" },
                { "en-US", "420551fef8ee7169f76093bf653f33e5cf45b6ae71b34f183a753adef7382a63e656ebab42d9d04b85eaedd0aedf864816599154214b882fd0e8a09194c849ce" },
                { "eo", "490417a534cd5ea8b5d37c642e49bedbf84a7084140efd0488a550e1cc812fb1b7823329f83f7e1b1d0af57d554148bac5f17df77f34b480f67207507a234cb7" },
                { "es-AR", "490a4a58c785afe09b9f01e7421de7079bd38409b10233407cb7917de348ec09d7347ecaea4642150fb61e1b6fb05616c78c571b9875b92979610a8b1712c984" },
                { "es-CL", "83c6e7d955a1d48689d9d76069c0d51e3cbe2aea69a55b5d58b111391df59aba06d83bd6b3a2e4804020ad5271571054f9828fb471167157c78a61f527957be9" },
                { "es-ES", "a044be9d11282e04338b85d71bab233b64e9e24dc31e7788272254b755f026ef409df9af4c3cc590a86d1a90cac4b05588df687707966bd540e39aff5f980099" },
                { "es-MX", "8998c4c3af4e8634d17cd65435e789d51b6a23a9357c75eebb02807e1d0b406c0c982abfcbcad426601640226f072b5b62b42c48417663753a28014be088a4f4" },
                { "et", "1f95f0ce41738835644ffed2f6a90facd90d59178cea18d1217a67fd278992e039f1dc1947a8f3f59487a592e1d273ed1da433365d68e7e08741b28a4b7dd7f9" },
                { "eu", "77826c1569c0ecd6952833036b4a42d77232b7cf0f395e7ff0bcaa7c6540f28c39f6e7382da051e78f8c1eaf8f2b55f2e4be4962d66846bdcceadc98c5e03fce" },
                { "fa", "bb23cab5f1bb082ae5020b648ed8d6aeb31d4506eb2ab304e21cf31206018dc77f2ff7b4bc167fdcc4a19a68d27528ac32506c61b53cbf6889ed57141bd4e67a" },
                { "ff", "3dc56b355802654eab1dcd73778e1b4bfc2d1b726da9d95058fc31b7ed925147f19ac3d865517aed6db5d22da67d6275ba2f154c42bd53f26cc72f01708a5bbb" },
                { "fi", "394fc04569fae1ddfa70fd245f960f8d79e0b0ad521101f876b2693db9fe546d8cd18989eaca8eaf698ec15f3d5af242c86848d617ae361235148cbab5efa245" },
                { "fr", "23ee75fc2b359388bdf4f9eb4416789cbf570c98f289a61614f0b7ea71469842a30307a6f9e8f0492e9714f79669f9eaecf6ccba1eaa45baf9922e0d03ced066" },
                { "fur", "185aabe901458c6960bab4e919b2d3371fa0ff2c10be381444aae5f87e14fa6dacc8d901ca0e0213f4ed12d4ebdb1664f46c982941976887841928e8b2624100" },
                { "fy-NL", "edf32852d56324d91baf78f528bed267e3a782d94685928306818526b6427f17873e368b9c08a94db2aaca5f082c37f54ada043d7c261c465bb69a8b1c20f661" },
                { "ga-IE", "deb66b0b65a1a47073fe4e95c4a52ef27373edc8cbd6a07e2094af722352a7261c6d82625b6a0162f8c7c68bf0b2f2dffce0a458ce76f6c731de785e457cd44a" },
                { "gd", "606922ce6598f4259835d746caf84ec1ac6204330d14288ca2ed8e5bef4b0d5b987ea91a4b22a362b8ff5e1ceae6cce9f063014e0d21fbd6494744a390f00470" },
                { "gl", "154ebb0990d7dd1074b8915d15c99c0bb724cf4f75a48784bc64fe2ebe2f879605be2e9acbd858072dafb3d6b3b10b9817575accf4388a1eabfcc62a36bfa11c" },
                { "gn", "2111e0dddf1496b5d7045ab50db1e16e1ad9a24dc55324f2e5975265169254ce4986f6c64a76e54b856bb5a1fa17000d4c1965cfd8c9d498cb61b4bb72e47015" },
                { "gu-IN", "d736c6baf1bff518be258cf29b00cbc6c92bb4f1f43c90c4716071c9f9f95b282590b78cd49a29b450f30c0b9137b27025422ad9eaad98ec511f37e8d1e3c95c" },
                { "he", "e07ea8597a70f558dd06a0e4bb9783ff921326ce71266b4c681d34d6f35826694c083522255b3272a6a0cf926ffe693a60233ac11f3cbcc6065f6c921fa90ac9" },
                { "hi-IN", "4794cfc2ffb301b3401f2e959495d255a93dc9ff94f208c436ebf0f5b0fdf0ef32f780f7b98364333e39431ee84304d56254e99a63c278107838a4bf059a3e48" },
                { "hr", "1503fbe0e537e103daee5fbce8df77b4a7f27309b2f8dd29ed5285371c09e6ba0a9958c740a3c54811691f02d8c9c490ff927ba9e955d5a9d838cf36cc56634d" },
                { "hsb", "548b4776446efc0ce1c12561e2bd00ab5c11014beadbafa208bba97d217b72fbd415e4aab248651ec98f6bb31bcd65319029dab5a70e9cc22d8bcd96532e931d" },
                { "hu", "9f6bbe14f7441d27b16bf0d8b4700a72169c303c705641e41821f5ac57f83bc070fecebfdca8c897020240ff5e4b4199becc84c168df8d2984a6a3e2f71f8f6c" },
                { "hy-AM", "4cda735e1bdf80be6f62f8819984068d025eae169e7b0f97cf90aeaa083a81db367796f79dc696fffacce2ba6e6907adf366a6a348406ce96879056395ead2d3" },
                { "ia", "b4b83c9bfa40999648e9ad3693cc995732d57fafe9e243ed16f54d99a5b43678d8af414de34dfb7baac130ba4cf0c6c4021018e0bf2acd4370c96ec5e2f480b3" },
                { "id", "7c9dbe05ec79d06583456147b42fa081c2e375df322dab600dc281ee45b9043a715b67457227c3cb1b4b6418458245f9a9438a61a480404427d8f062c3f81ea0" },
                { "is", "62781214bad2b899213992d6cd432e2d4fe77e906749b9595554f35dda34cd6bae70117d99ed1637b627b63b20a6ab73a744e6f7a560ab11d773568da621b394" },
                { "it", "b73fe2ec5318ea45b0f6343f8afa6fa4569f5b11550fabe2bd0cec6d832879eb4a5703d6ef9437dc1f3dac1e744a2b7287c532cd4eb503ab530cb86c61dc335c" },
                { "ja", "0c89e9818bbf17ede781e31f43da537132e3abf3c507eda324d24b74ffc84e295f7e4e7969ee787b28fc7c1c91ddb292b64c188e4498b82292cf3ef8345b4439" },
                { "ka", "b945befbc77fc0520b53c90d731fd363ed49f6a47ac44ceff9a15e4b347bd5ae06fc6166ab289722c80c927bea8c23d8031389584dcf1b024aa313ff5abfb68f" },
                { "kab", "fd9ce2e1c4c39f34e1a8fd145f1356461fc4ec171df5c528a05b683400a872b30c2b79b343154f7f154e2fad9e5dc45183bdac6b49faf68d5b664d1c98547aa4" },
                { "kk", "738524e5e9d034b25835c737c6f574a3cd39c94b74fd69d7a4f2a0a4aafa9c3d830df274ac4e92d32fb76e2efe5c08c6e852c614d0ee3b48a42380dc7f958001" },
                { "km", "26ce6ffa4d0be1626488957e367734090073b06e211fdb8c3d810beec0e787ee85c8a0b1a52328ffb0a009aeb37018c618142b7ce4ceea6ce48f036964bffdbb" },
                { "kn", "e5fa602ed5ba76f7ec2bc14d406e7930ea6a81f85d42c0e24150e78becbdcc4157d2ed912c98c5c460221350b052fa63223db82cf8ab78b34ac080cf5fdaddf4" },
                { "ko", "fb2aac9d058c4ccbf27954079f1efbbd789b9008f20eed541a1326d283fc5c54d4ebe2633a62a4b4bfd7de0aaa6f4da19fbeee28eff324f85419f7b9bc5f933f" },
                { "lij", "083fd82ab58e0527750b3e38a64e996150fded1364f616765013e220e0903fdd8783bc9bcc6186c138e44898b70d5c2be7ec2515c5e42018571af4c76f6dd223" },
                { "lt", "05f36b142f30577b8441a7e46f9182ee7f8cbda73059edd44d890f469fb99cdc6e9c8089e10fce75b3e71ff2698102b10bf98b0a00118d2835643c27153e326f" },
                { "lv", "d539c8031a85b16cf990dcf6c456f76df2a7396b5c9db743c02049ad6288edebf65cdc325ab244c3dccdba596b7c613435ab6fb5884e541e5f76730ee9d5c77f" },
                { "mk", "91959422f71d21bacd3e79be496e3e01dea76d677c7b03063b984c1a02a7d67e75cb0c58c937228c538124306037c001c84f659a7426d61b46edce3c8af06ba1" },
                { "mr", "fda6b75e87c92d12a538857746b1b8f28fd0ec8638dcb5299e0c245776cb6796b254698074bfcfe885ac423bf0518d3c6c48f7a9c11fffd0b0fe228fd32c3783" },
                { "ms", "bf31c3a2b23a87fcdec4dca1b65c2d31826507ece89ea22ec73aef2e8b05d9f2988be4415aee86da5954b4755d089de52d7b8d724e1a480a2e15a0b0b7d1bc6e" },
                { "my", "cca08b7e621244eb6f3388d97aa5ff1fe883f1b3f6d7a58c4c5885901a6028e2cc52273256e577521953523f6e7ca96cabaae2d8d19c3335b56d90b7a42e2a6e" },
                { "nb-NO", "ba62d1e0cc5e53056838ac587f159028a35bfc50cedf76471e3ec1d2b99fcde30921d4499de702ef53e974b7be45a28b19485f36c64e2ed9d3be1005e2037d69" },
                { "ne-NP", "b9609926e34bb8ab7cf446b8df6ce0666a107f85b8a84e8d03cf3e3792c390c8a6a73a8b81173b4ecf202f78f2a9132b3d5dffbf072140a5de037637184fbdae" },
                { "nl", "6095b909753a1b1063185fee7b0b94105c4cf020af1d37a24ae4f4188f65745a02a7a3c75509cffad0b62815fb9c4c2840e4fb78056810129f4ee1217da2f130" },
                { "nn-NO", "298aff1e9615d759da2580cad85e426d784475ed273bbb9f5f6b8f96eef6436eeeb2b124d2abda805305cc9c729b8489777fb414affa5f54aa0b550841281aa3" },
                { "oc", "83566671fc8909e30727400a16471c90e9400f74fd692cf09d24a2a902ae647e1046797e4d606edc9d304aac5b11f253200ad791488acfaff0e6130163069257" },
                { "pa-IN", "94cdcfe6e1307022711511b210bdc0daf2274fdfce605b45630cecaad84ccc5ff8a537916fe3d2e72b1f1b2ba10c91e82db4b4bdc40fd12110c692d0fec09162" },
                { "pl", "6db5a3ef1349582c94c9684c9e9febfed9bbd29b6a48ff07d4d65d45e5831f7ea16091bd7f2738a6bf494e2d19fc8cba77ecef6a173316f4c3e84054121d8f06" },
                { "pt-BR", "bec515ac67cdc2df0fff818e47a250a4ef38671396b1477e00d97cabae61670f071be77027039afd7f9252b9d87b66c51bab54e4e6d77c98aae0c67dfc406076" },
                { "pt-PT", "7c46828a8a3c6049b06f9a2f9d50d1e0a20eb2556ddbc6a88eea5205a2cd6ef50e010c58fc814f84205f400a4206aeeb5f41e91562902ed452c617ca3d303b8e" },
                { "rm", "0f8a571cecb9025417d43f50098ff467210576257cc75fb0870c5f376bd992bb093573ec14484eb9c4b55889f4ee3feba836896f6896de701a0d2287da76c374" },
                { "ro", "5202ee8926c7e3d6232ee63d8fd5c737945aaa6989b108ed62b3e078112bf8e496957e7c09309e80ff0d99de72d1a3596408a352a14a73115afe929d7d942069" },
                { "ru", "ac46bd949c55939e4608752d249fa6b28a6bf86dc158ec7abd0fed3cc7151b9139fd58fa3f07e0ea2cf98589da884ea2032978dbb4755a5005426647a13ad1f3" },
                { "sat", "bc885c95ae53d3f4f3927f393b0dec580cb2788c5fe476c13f928981f20ee7e73991bfa52ecb047554c79dbb253dc43de9c9eb47a5c7d0dadf868e783fb2d40c" },
                { "sc", "4b39ed063fa540ac919ad8a6310aace529e0d88b1e52e19b60096ca56dcdb6821b9824fd7b2e81b1664707af8070cf629032013bff4a309353635d74dcf1576f" },
                { "sco", "42fa292c3248cc5b3ac67e277500a9d0d858119d04ccda191e0a38754447d8feeee405b4d05b9e5ef817119736677acad113ef5721aef088f076a99575a271bb" },
                { "si", "419d02ee5764b962c80998902bd2e802246a94367d1d2066d65945aa6ea789a75bec8a21c3c525d6d07e4ac317b05e0ac08dd5cd682555bbb766f366c6463da6" },
                { "sk", "3f9ea20ad91ad2fb806451bd7fb16b988ac48bdddd53ea27d085b01745d168ec576ab55111cbf333a61c1d85e214c8ed63110622ee9bc21540b9e10527cb0197" },
                { "sl", "7f75b6f21b2522625774e9250d6671a171bc838ff0858534a22786247e3f95fd3e48123f6cecfb444a99c2bad8c18fcaa9c0e87f3b858fa03a88afcbfb69cc5a" },
                { "son", "0c7950e3b614a03f92167cabf5a03db53e91b4f94b0819ec7327accdd987df173b74806c59008736cdf5bf17f36a93df3f739c60503d41b523774f8ed172c9fe" },
                { "sq", "8fa6dfa1b17fee814bdeb15aeca6354d1074b4b86e515465cac98fe459fca275bee4d5e143b0d70242abf12871573828a0453a2626f01ef3bfa51ef2d0785bbc" },
                { "sr", "b12513eab1a5936ad9d310cb3293ef6ecd701b43f1569a7c00eb76081d546af959761a7aaeebcb45df6246062596cf968e17102bc78bac802aa6b299d0b867ca" },
                { "sv-SE", "aeb07d4b14b43a794bc2229a6971357ea2ad4d63ccd6778938ae942e6274dc0b2944cc35d9a28fb5ee0be3473d951c47885e9f7590a3eba7229fcac3c49011e5" },
                { "szl", "7a395b09f85ec885a717d621a677e5cdca44ec7e55e802f0f4e0e7ef5b1da0db2a1f9e0ee5b10c06e0d711407eac685fcd20906fb28d9a0ed7ba859ca46b0d83" },
                { "ta", "74edd4fcb2d37c2d5f8e2f8cfb6e02230651a6ec4e48fcbc2dd30cc5f466f9d0b52c10e49f6a3c0086a9fc78ada8ad204ccf9e5782f347d4c6cf5a3a5d5329fe" },
                { "te", "2657a71b6d9d25db6f315e90f9f5a8d5624bc9a4854c108f7ee78e44f931b021ca8c172d285e0dc09c6742169374f637c89b69ac0fa91425d3fe968fdfd7e188" },
                { "tg", "af6c5316fbcfacacea0f92b649fa6270f6886bff1ebf6e0622a329fcee4d36ac74e14593648fa9a359142d20c4bb9471ebb4de56da473e0503d74791ce07b0a3" },
                { "th", "a453ba529a548986e60f7d1810fca4f3c628e7b509a8a793129fcf2c8f68ff5e3a6012ba1c1cfa44d3f70691a922206576c7b1d3d6014283a503473a2de1573c" },
                { "tl", "998c513a43e521b25c7c7f1544a210e1e9b0b306a2e9c7fa77a6b9a5b1d65d6e0847bfdbf3314b100c47a0420e4380a981acf26dbc7c29eba1eaa5c70ece33fa" },
                { "tr", "ad076d0f6371c213d0f2d20337cd95a3c3c99d2edbf4b412b65259f2e258ff5591eb3ced384cafbab8f70f428bc02a095ae82152326ca86bc97d54818cccc204" },
                { "trs", "96c061890fac9ea24edf449ddc0957e3399f9e272456062fe2fab6a21b7890300fafe93bf286fef789656ca049ad524ef0ef9e3fd263996daec7c1797e610aed" },
                { "uk", "d703c555d4cdaf80257d2f9ba0be8ab24341b13bc3a928886cdf003c1e004c601272251298713aa2e2c3881e691fe0dc3b90c78034455e8f5fcf92f78bbbf9fc" },
                { "ur", "ee145b1c5fd310fbc9f254ab6c237c8a5d29f595ab566ecd34569156c088550e06f54cbc9e8297a3f37b1f704dfb38dc7b8e7fc8bfa632d4f6219cad99166d23" },
                { "uz", "70376ea7cd46d1be0d885c6fc2a74d87f6f8edc20ed974e4bae3ab358e632529dcea0257056c37edd8201280d77a0a078598075fc6bd891363075cf4b9ebf92a" },
                { "vi", "2e907341a50e622a1def87585d21c1aebca1ab27932e805e8bd261375d7aff65cdffb487087aef111b7c173b5ec84252c2e186163fc3448279588f9e8a1a4246" },
                { "xh", "94d9af529e211bb431df6fde028a30c9ad5722041bd8f7d57d2e2c3a4052f371a9bee290945f1267da960b6e59bf8bd706ab318c80b088e79d77626082685b42" },
                { "zh-CN", "cb34d5ef54a13a4cdda8c01823db78cea168903f5102becff070c7c149078356b89445a622795949004c0c195386edaa88a8378dee4260ba987dc9864c249a7d" },
                { "zh-TW", "d52e05b31160058f73f25265852f9c88a993a0f4ed8dd7f2dc4c970beb5577d484c0ef02364999cd8714e988a562bd414dcb4283420ef4944c90a7b0b162d0fb" }
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
