﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2020, 2021  Dirk Stolle

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
        private static readonly DateTime certificateExpiration = new DateTime(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


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
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/89.0/SHA512SUMS
            return new Dictionary<string, string>(96)
            {
                { "ach", "2bfeb89057a67753268268d6bca2bc12f9cee7fbd9e56bf9d4e7720864cd6bde539ad7551408fba0db1c0005c46de5f2b4dc014d48619a228dcf7758488932ee" },
                { "af", "cb30f92a2c5f2da50a80315f9df90354bd02d55912fadeda02250526edc8198062379db17a3fddcc12c16c895fdc70c374644232fba04e09d79f9c2a4b228db7" },
                { "an", "44067bc68faa36107f634c351ca467226b86b6461a6582c26c96642232add2294aa3ed89b301f792a471e9ec0432eeffbedd6d198fbee05f5f7136c9e37536ac" },
                { "ar", "c1ae5c0a8bcf7c0b52508a58d72bc843ae45a19e14b5bd7e7548a52d066bf436b89108032c292aaf38c7a6a909b0a79a4085ca266cbfdaf4d3eda1f130f6c0e2" },
                { "ast", "d9c8ca21d7eed03c0e29f026cfd59eed52fab7041bf9394f5a9316fd7fe9e34e4cdb01cf1d3424fb2a716c0c011218f50ef4bc6d76caaa92c15265a989f52952" },
                { "az", "72d3f6ef275d2fbd7126ccd5e702a5e67b4033326ab378e2b2a56ae8dc015614368a0d54c7970b25bd910cd19a908e674cd801e9447b6b4775e110a963d68592" },
                { "be", "95b60b8322aa3dafd5df26530bd1ba005f24ec3ab01662fbb4862b1d6e7d966b2e984e1cf520a3d6c2a4da7efd9431557318b0cdc36dda262f01fa1185180ca5" },
                { "bg", "8c8070917a144eaf0b85056d8d0001faf169d4309ef077bb0f5d1a8a5d506ab4e07c7db52a9bd539dd19ac87096e3a87365c24650ff6e9a95607250020ef823f" },
                { "bn", "c2aca6f31b1f2c8f399122e435366139800fa90c1a5735a406ec101e841225f676e4bf054da9de81843b21de66f3d0edbe6a65927fac3792db09885e0647d2cb" },
                { "br", "d084bc929ce6a3c22c49bd5cd475b63444ccada4929b3f711477e6a1db14784c77c4a465efb44dab775e6ba1a1ac1b93f4ab04529b35cb61562427fc21fb79a6" },
                { "bs", "bb4a549066be80ceeb095ab1441a9d1dd7865bb100d1cf245f098e128a8b93a57b0f63257df17e607589956966eee4f38b8aba1cfd6810f5897fab79ad2ff516" },
                { "ca", "1aaabe10f63ebf7e2ce8c9a136d3a0b6bbd2624e22c87af1e2b4120ebac1d6758e305434d1b8de882d07f843ef89d9de4fe490e6989745faa9707c1a704e82c9" },
                { "cak", "a0c94fa99fbf9b8455468dcda0cf3e7b35bc1c90c7620f560d4a60af4686c9e4421266efa56bce588be4ff7e4d83373408584574bf4d5f062202dbdf13850757" },
                { "cs", "61655cbe6b7487cbb970d81c056197da2d5aec64c4888fcb831bf150ec3f26ca18be851b48b8ff8097cc304792031a8935592f2423a3145453b78d6b2a876044" },
                { "cy", "991d6b336807537fcd088883aea4b4209dbf9d231b66ccb58adbfcb406ce0b29d6200e5cd416a1e92d00e9c766a38bbe592bf109c915446ed7abd7afe537af75" },
                { "da", "2c074e94ab66db0eaff7b260ad0a492556c9a2dc0ff54996d823bdea2a23010d358d9a89b56160ea50049f8849dabd88c6593c7b31f6c798e13f1742f1d14f07" },
                { "de", "c48cf4b5ef0c627226827309e0704a374155f618f6134fb976f5e4e3658ef460186e07172de364ec1adfd6c0bc693a60ab9c652d8983a150baf2924872f3675f" },
                { "dsb", "48ae16f70eb451b8f8f3ee65efbccd234e879fd13e15b4eeba11b9b004975992a34349a7070bfdb8ee4aee624895d9dbf734476e2479e0ec259b530cddcd087f" },
                { "el", "fe8a4949f1a1293723ba1199553efaa8aa91c79e395d60f2610b4680ffb7a68bfc07a85b19e78f880ceb93d53a972bc50e5dd82c13cbecc305f766f0f8b1c734" },
                { "en-CA", "c2f5ef389a42768fb1cdbdd773073bf72b94178726918763515bba35895f650e7cf385a5df6a20152413347e3c693038aebccd88598fdc55c20c6c2134b91198" },
                { "en-GB", "b167a67f19f466284dc4b7784bb8279575d3a1315a758ab96050ab600cefcef0a1a2e414f769638e8ce5482f606694c3890f1871128bd1475b44510a1b66717f" },
                { "en-US", "b84c813d2c4c9ce65383640374cc66af1b1c837c3639e5e84140eb7e9aea9798ef006e08fcd340b594dfa3acdfe71d38912a5052f29cec8517858e47bd4a83e6" },
                { "eo", "e27d21fb83a47019d014cac9f75bf2448d0069f9ce24ad3db74a9ca7344ac11948f8cb54ff8de72232187ff8e3873e8c27c615c0d3cb7c25fdd0688919a4d0c1" },
                { "es-AR", "b018d730fc24b72669cd43532ad039d8b3031398b66c35e8b440f4da8a80fbf8f533dc3078bf8b7806a459f46e9707dad8089d549645e0b6fa72e75bc0c3c0d7" },
                { "es-CL", "15937060c78c13b65f1423065837e35c744134b4ec2189d130c518f542701d67e68f8aa127938f273654028f89f59efa59580560f43c4cbaecdcaeb80d19d82b" },
                { "es-ES", "b0431bae6b8ac5a8a19c421ce6deb15e487c7d3dce6e3c2038a44ab740d787ca2fbc572215f7869150af94e37d0e473c98b5b8235b98b1471a2ff04da7e95038" },
                { "es-MX", "bd514ed587a418c7b27f6b05f09d0939b3ee523c7df779df36428afb0bfb9cc598fbc0693fef97744bbcbfb83af2d93a01f4f1bfdfc778d5751ff42d0c43fe4c" },
                { "et", "7cfb878017e26320c35b171a22238e4722a96bbdfa325d4b18a977a5746088dc02f41426f8831b394f4e3a22583395c8fa33e128ecc5f1d34dd55e9a06c77120" },
                { "eu", "86eb4e5e0fd1d9e479ba818c3608527ef55301e5107a5f8f8e0d562c88263e9260b41f813b448ce568ffc41026355828e240096809cddf89d750ad499cf2f729" },
                { "fa", "960784eef835643ec35e84cb606b4f39bdf5ef07dc3e78d5c414a14e1cd391d6c3be0983d5f87ebb21bc3c2f4e1ec2b526afcd66701a30c69acd5364f7f63de4" },
                { "ff", "2ef8e712b4f3fc089a0967434c7f55b8552a541704b1b36885904a9ca017c81e4ce2b5dc427424620432283653b7fc790180ac19a450604ebcdd997e78c51bab" },
                { "fi", "3da452e2f2cc2875cee7e37127bdf6027000c08087c0fbd580fa873b4e191e2507cb121a789750e46100a8622eb1df3545bd9a35cf05056294bd6f7f69c917a4" },
                { "fr", "0c2aaf21a93c79060c8321fe4e2d70693f1e490fb588bcbb10d2fe5d9bef43ef0ed56168099214f428f9ac51758dc308366982ba56ce72fd690b2bb57729c4df" },
                { "fy-NL", "8485c234c3ded0dcc286b253102d048967abba670829dceee1272ac7752cdd0ae29ffb557445a8382aa2e7ca383468532890b31f4e497e91064ffb6ea43f1cff" },
                { "ga-IE", "fa20a5fea985f9b6491746f8911875eeba4667b497cfefd7c1c26e38ee2620ce644f6540a04f5a8b5e81c6c87748b1be8802deba3b5e9557f5f40c8d52ae98c8" },
                { "gd", "6b491bd4d74072359494dc3f4f8049787e91ad9163316de5b22f779750c2fae53e84b66d23c04a47ee8cea0834b2175d95afb1231df7d51a312e6d12c3dc04f6" },
                { "gl", "5cfc3b6f8fe4df938c3c006e0e7698cd88ac613514ce08ac140f76812c7695f2b3f137cf63eee546bcd268062f1fb99d65f08cbdeda2acff7673f9148f3414f4" },
                { "gn", "03ce942c1c22e99336e9453f1970e70b88b1ba95b3ddea590b31cee975c16843234f753195d2c9d0b6580750a68076ca23df73e2fc2a46835e3f8468b39d4435" },
                { "gu-IN", "9f75e30970066d38e13185746c57c6d4dcd85f081f45a3a095abc6c37aa76b0db0956c4a09d88122813ecb3a48fb99777231633b2ed9f126ca56061eca5b68c9" },
                { "he", "6dbdf2fada99a0dc257eb7288812a72a6eec4a6b35b5ba6d6957230003113080718d405c795d50f3d8dfab4d9646e4b4adf421ec44d5ec143a21db03b685e234" },
                { "hi-IN", "9514195e08511767cffb2829ce61cfb962aaa52ec4e9569f18202e658d7dc2b8e98e5a807955483dd35d88cc27434711201806599707460a616bf79959540a62" },
                { "hr", "c13bff23cf8e761d11de027ee25b225c4713420813664a338d446959fe02586898148052d50ce0ce0aba866eeabf76b14202e537f32fd7af4c679bf199d5e8d1" },
                { "hsb", "d7705f7a47725cc95cf76a4ff2a93da05389b00d61d13b1c5c598b404cc8b85a51e5cd784a9b1182a3484840e80a062d5fdc31eca5ecd9ae8dbe5fb17789e7b8" },
                { "hu", "2a8aabdccffd50be6ebdded16cf71b8f64a474cdd204e8dc961066409bf706be88bdb3204ac73b3481aa5f8b21a8983df748f0da0dee59da4639ad58ca61c479" },
                { "hy-AM", "408b2b1c42ac8cac08ba21d759743b5b66d3536057f7a1f3c56b8b36580974a3b6d55cf76b6f820d6a437e95ec7eb2ab3d48bf8be767b45dc0aa38a1e6c6600c" },
                { "ia", "04c81353153c34941a35f5cc7d00abff96371f63680954b32078b6b97e2598f364e57015dba91f6d19a2ce4bf333b6ed03fa46d3bca5310952552c6e48a48c68" },
                { "id", "8bd42771b95f40b75646f839cc738f47842901358cc3cb376c351d80626f45198eca1d2f9845353ffba1a147febb8fd705566929fc7e12d90c595ad638316584" },
                { "is", "c35b2357d6475aef81d6a7ec183a828acf11561037dad7b38a9fd4f1449ae12de35304deb57fb26aedd41049704e3f8efbb9e6dd208d6530943726cc4bbf8141" },
                { "it", "7ee42e272043dc5e7ac48fa35a534310b36105a3ded81bc9b0dd5eded3100653e0fd613a40eade94ae878cbf7bcdccb4eb49492c62a8e6402bac71175cd97658" },
                { "ja", "ba9b775b517e2dac9035ab1af5fa6dbf22778c2bec8f2c315e2b0a9a1abf307790eb364a70b9860c5c3949a99f34f1b479993aea0967e5a4efba9a45bab09ceb" },
                { "ka", "416fd0d6093037df0229bbbd614eb0cddec75c3659ef095ce884276332e7c29f2d350c54c5a99a5b559ada2ade7f8b2a8e6153d83552c8bcb762014c89ac3983" },
                { "kab", "267c03ccf858b43e3297143c99d87d517d0281c2a948d994f160547fb5e8efbba667dd3bb8d64966d916114562ae8a14818ab021f2e2c85fa1a2018d5cc9eb0a" },
                { "kk", "36b83df75bec5be3aa50ad566b36f7e6029ddb012c359a0872cbd93d7b27bcee5f719972d23954a62794d552ce9192db3a8a525a781e9c81b02455a33dcb17f4" },
                { "km", "a33c8784fc6b5c948f052fb8e5d9ae1d505bb289207709fc823ca39c074c24b0524724026ba23037a350a2268115b00117fac770f3137c274246aa9349adc9f2" },
                { "kn", "80db7cd4b5c4ca9bb9bad5dc100bda00c06bbce4d202cdd080a3fabecd7afa6a01e1b698ebf0b900b7c3c3222560ec36def45a7a61e2f5fd6bc023ed1b10b98f" },
                { "ko", "7147b143d409ac74432a84627afe03a6117adce63461a643641ea7bff7693b649170955b5bb41692ac35b1f57319017d7994de132a71459b7560688e0656b2b9" },
                { "lij", "ee66251c254b432653308db8d843e77e32aa21c066473918a3ef85805dbd0deee2e637f72cc0b002010057f6c623510e0b8e5d9e0c800595c2f1e9ede2982b20" },
                { "lt", "1526be24024554f2774aa0ba7da91267e7fc71cbb70ed50b7fbb90e585a19deda147c08b92cbe6432ce34a473601172fd3f9d624c71835dfbb3046ce92c7ab5f" },
                { "lv", "c1196d35c90b246d1587fc6316ee880a2d32e2775a26cea45d266a28ff869f0d1ffd0afcdd773333fb083718a52a0f8291613d7b31363ebfa1e4b54139562bfa" },
                { "mk", "f527a13d6c5cbc764b9a943ee224c019073a73eca8f7e90bbab6db6868cbdd61346888fe8bf3c93a305020c44ef7775efadd83a636a003340c1611b62c52ff08" },
                { "mr", "d39fee6e300bcfbddccc6edf92f39f74c90d4441b38fa6cf1d894e6dc2c7efcad61bef7d609b8ae99481c748ddf6ce2d017eab8a92167064fec4ba02d0cae9ce" },
                { "ms", "28fa665f10f759ba3bcb69a1e71643c878d5dc0db7b01d64fd159509a63f928d22c23de1ff39a9cdee542d5635b629366244842c35b91428b3ce10e644689868" },
                { "my", "c8aea5cbae4eef4c498fd8a4abc88e94e34d3c0e7625ffb911f5bafa3bf8ee224565667de9aa3e11f8f6b8c7e1f850a823aab89f14d0b5455f509661e32de872" },
                { "nb-NO", "d4f0d64314800862b72683032dd9f601087428eed46475bff4fc122def2a98d99b21b3fddd1ea28cdf94f3d9ff3297a0309b1b4ddc6e89122326e4a49c275745" },
                { "ne-NP", "f2c05274f3154f04b87b0a852409508ce527fe7f523b9fcd295edc136c5dbd3d159bbf9749be6ae0704bad71ba534519c16523ea246872c394d2e5e2596defa9" },
                { "nl", "6aa80df89f26657df626ad98d775a1af114bcde314b156c7c23654d03576379fe9a8032bfa0a67a5de2c63de1208690b311b20e1342b26e09e0edfff033bb587" },
                { "nn-NO", "4f1afeb32d9a63104d63e4ad33cad0c8e238d62c9638e8064a7d9cacd320c78ab1ae93886d877d3aaa5e8a9c7eaed516ede92ee8e76d07c98afebe7bd141660a" },
                { "oc", "793cf725326efe7ef7d6a785bbf427f01344a37b121b6c1b6d3df435738ae919069cd1b4b4e6ec3e76e04628516fc90fde696ebf8ff2c093acbef43dc5a8ddbe" },
                { "pa-IN", "db9074cbb0b6ce0c40302d50feeba895d3d33e7440ea73d6002ad4a77142851451f2cee395d7beb93547030973b901cf0043952df7535d6d0553e95352952724" },
                { "pl", "163009389e38564207a81a58fa3162e96433511f32397395e109259edae0fd13a8d650616d7cc017d696152a4815cb03d51976e9fbefc731c397070d812f2abf" },
                { "pt-BR", "4a3d4d932482044c6c35c935678a73716723c96f3613a7ca05a80a0187162072c186ae3e091677127ef89a383e451e1f72fc6f4b4ef737093c7b2c81798cc578" },
                { "pt-PT", "570fb89bcf39b4ae2c1eb2b777f1d52c5c7db5d76557c7c35403a19e1d4ee2428752fbbd6af06bce01d4c30bdb1d9a50129c0f23bdf595817c1ffdb1da7498de" },
                { "rm", "5c5e11825060b95ff9dcc7274aa032ff3a06ac98ed0ab5f0795a1bdba71b57d0541092d6d84e9fbadf46b35f71112f250ddeb52ce4447d778d65e06f519b5c44" },
                { "ro", "5f04a9f9d099204c7414ac64815596b8224fe0a0c1810d9f76c2d813c5c9758103ac4cb319cd21b3f81b14bd225d6883a5ae22a139f29b381f46b89301c2f5fd" },
                { "ru", "d65db28186265f12bdd55a8a1b9add31c22116ab81c59512ee6e4d509c00e52cf02158e3898a41204d5d3ccaa38cac840d95d27159faf8c4892e8c085a101040" },
                { "si", "442491525261bb25e904196561fe094cea5bb1ad48aa491e37db6e0165554286a64fa8467a0e19085e30d921b7e0f93c441b502000f2ec4f3f57e4e2d85b2f6b" },
                { "sk", "677b8e6089f065f94b2f55f73f05b4f2774f932cc831e8e304ef4eb9a5ea9ce7f0ef5356ed6103be9f3a425108c17c10d7ad1f8740b1aa0dc0aaf55a1a3332ef" },
                { "sl", "945d99da74226b3c498c9827921e7df1a6888d3f0c2d19a28c7071a91177f8061d40002bc3180fea70c192196a3eb1f5bee2a9dfc38fa0717e6a5cc43802a8e7" },
                { "son", "f4ee6b2d3057f3378c4b943b40d9ab2d1df375e0f9fdf4659817ef718c02a677961eeacb234d32bf04172067ecba5c97eadceda358012b2bf058a59d3be78f73" },
                { "sq", "43aef74dc00fab47dc83849e2e7c1738ee21e685e51037269020812a70657b78d3919786dedc4ec3d83679004269edb888e371d58b39660771b5b1f3f4bc0761" },
                { "sr", "7ab69d2fb6f3143d91dfc9e662e37d729eb6cfd8caf09736bff0a8cdd023343865b274daeac77240ce0026b65883a334ab4091b87c7402748aea3aeecb6db70f" },
                { "sv-SE", "3a23d47abbd9cf684f48aa1ba8a83b245687f76f9ee007b1f31ef8ed19e7238512d56abf92f81712774c0b3225995b13bc4e81b72c97af38d7e1933fcd54e729" },
                { "szl", "0cef4052bfc481031c0b9d5fff1fea753ded64951bde793c9ed1e3eb700a2354da1296d986bd278bc3d4d0856349844fa919a392a6cee33db4bfd052c8ada286" },
                { "ta", "fd73e80334f4a92d30645868c4c54ff72acca6ec156ce83c492b4ad17a7b24f760771db0bd79f128d449d7fa88f1c3b76416155ec5a4a55335090e0850c60a00" },
                { "te", "eb390e646fbc7511a9174c4514c79eecebe8100c31f567663d21c189c20448ba59edec4aa7c3d8ac07b36eca6b3b27b5a902a18287f52ca28a8f22e9ec6697ff" },
                { "th", "a811eafff5ebf7227ab3a72ae01921939e3d4d4ff6c7322721d524bec8d1e99f6cee0fa2fd35172dab538e298bbfafa303666c0c4c510361e5a297d5bd176138" },
                { "tl", "bd22e68dc111dd11500b73157ee97d068e374861661a9e6ce4df7533a0537ddba5e4ed5d309579c5dea5fa8865ef4e562f0aba5a13cd3d3be5b585825118d7df" },
                { "tr", "0c3e9508ada41a6b69515bf27fa015eff6041c5119da2076eb7152f9f6d6a48f3e08925857031bedaa75af0a58f80eb3112db9ff9e9803172be5bef9f7cae1da" },
                { "trs", "ae1fa45cf576d6a5fbb04899ba182ceae963a27a7083206104d707303b9d51b88a3aea8b067e0cc34eea6d4f9842ada383e581c40b4fa6165a694d74d9ea2c1b" },
                { "uk", "f2a521d4058b75856e7cbf83de946c86d2180025f7ea95757f853cb971740b3737feef1b646cd010f8d48f96d19273d48465414e46f4e4ed6c3ca69ebe406816" },
                { "ur", "a5c42e2e74af9545939b37c31b4c540ae1a65cf02772f60b24bbcc4f676020bcd7e919cfa10ef5bead1948163010b0876ca3e5ab8394f20f165a72c74eb4ae9b" },
                { "uz", "169d0493bc2dbdcdf39a955dbae5e51e15f997af495a7da31c27fa39fd0457437eca880349c5616f65baa8804675d9b305d734b855e923b76aa6c46de9706601" },
                { "vi", "9dfe1bbb87bfd634febc36a56c9fda13efa7d3eea733b4aa1a5c78220a6e6d4d9100627a359ad985bc59102c251d5822723b2892338d9afc8510fecec199579c" },
                { "xh", "470ab577a8640b12d7437e2a730ec31b7dac14961564f947c42d198e4eeee0e6c6e8cd3322eaa2a72b45b261ed9c59acfd77130b7358959b43b4de6d9e11647f" },
                { "zh-CN", "32e1de7c40740974e1398c0e59a03a53627f56bffa8171abc22926c3a00aecd340d82bd5324908ef9fe6391ae03b765ba8b55b1c053d167ada769a3c7fe6d7d0" },
                { "zh-TW", "17b2251f168c0dc1b125b06673d25d9efe02217e926291c2e5261e9ad728665e5a53026042d53fb0411ac5ae12a83709f3a10a06ec7fb221680ecdc4f8f7b5e6" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/89.0/SHA512SUMS
            return new Dictionary<string, string>(96)
            {
                { "ach", "0303f0238ee721e8a33f895cff5d32a7bf0bace3ded947fb9485f41fbaa61be6f3d906e7a4fbc2af43af6c5d14e88fcf961dc9e285db5de9916d81c4a9570113" },
                { "af", "64199425d6506bbaaed2a3053ed5844cfd8d9110050b6d19f104bcf4a10ed90de6295e6e3a94dc2c12c7cd745dc7c54c684641a4d9f1c3570af2d81cf2e2b8ea" },
                { "an", "8b01d03c706d08129974c0294bf727915303e0c73f31e14577834e0a18a40dd5dcdcb236d382e55bb777aa14aebf3959ef63b4add22f6a9d6ca6a627b4319608" },
                { "ar", "f45167faaca4cc8d279681c20d80cd233fdca9fbd8b5a7ee547a4986e3c6055b17181d584b7ab5e48ae745392f243044ab9f3cc7285cfca47c22a856605ad132" },
                { "ast", "d2c9fd75a38d7d2f320bb2d992dd8aa4e5eba94f1ccc64e0874d0357c34efb3afaab4496b633a3b75628820bd3aa807804ded69c4dbb9b8a780c1a8063a49d33" },
                { "az", "4af01b21f2bdc699f1e1f083d321747aa7b8e1e7715067579d0c3d0435308d2631dcc820b08df3dd1f385fe3e1d60074263e7c9bfa84526766392c63706c9368" },
                { "be", "e1f6803974f405128158c84a315fb3f01f596c4ef4d73d288e00c2665501510704841b02d6f4126012dc52f69ff3f7a941997220dd8314304e65706d2c0b3871" },
                { "bg", "15719a591d040e14f663c9bcd8ef155979c4e2f4b062ad239d4dd41e2a3933c474b8b88626a39580b043c22100eb5433001e7aed8fccb82f774757adb65c12b5" },
                { "bn", "57eae101bb431b0605732363d1d58cf26c56337cfa22f74ed2f0c182e9a67f51d3a34c7c741847f5c890b48cdd8c0dd9e1bd04428a4617878c45544244b0dd11" },
                { "br", "596b9679a3f9984ef14c54a91a726abc70a83d1f35c731384f1fbefb2d233d6066a7dcfad091b484de90b2bae5a2388f76d62f8043821643e87680316cf56cae" },
                { "bs", "88c34e0f9a9a052f0c08191015589c4c3ce6ed474a9604b674d1a21e0466029553426fcc06426307451fec028115aeb96df290ccb7f3717e83dbb98e2121f474" },
                { "ca", "7ca7f63d2200d5676734147cc14add1589da60777bdd6b0554e81e9bf90c9dbea11014b52ac604f6b54008631ee352d1e5f4c6a3bb78209acb585cbc83efc307" },
                { "cak", "72465a3ecb79596472344547aa269b6719dfa57e0c9affcf09457ec9b64fd9f9aa0effd67548d2db4291d71f9a615ed992b768b8169d33d03f0e0bf001c21d49" },
                { "cs", "4cbf5d00d8c9136e301e3f1f5ff9c814f2ac8ce5049ffc5d6edeeceeb763d7942500cecb444e6c9ea49ee18d4af1e24cb59e4791e4af7cb3d394cff87c6ea379" },
                { "cy", "9abf48a2965bb29f350733cd5546665403d99398068ee9f5d02ab680217e609646ab3dc6944c0104395e1a0db61f539a4d8d1a0a731109bbd69f076e6039d1d0" },
                { "da", "551a2fae64b257d31afd4e69f5f05a1260876492d931da10068efaa9a8c47ccce870a9519e22fcc28c9e107b220c28d5089367a545a24853f086c10284a86530" },
                { "de", "73498b816075a08af605b22ab4baa0900d059feef01db4371526e5dc21d0bc614d600a7f44ff94af02318cef03eb0ff5ea2fc10eac294fb3a7f63fd3ab776aed" },
                { "dsb", "313a849a64374e4765a68c7eba98fddd5cb069e5c5a79f06ad9e9be23bc6d978b3b2d3dcca63abb9ad0e3e26aea1c6564918649b7594d40f18cdbe48849308e9" },
                { "el", "3e34139e1a83265875b0bda0d9ab5fb41f550bd774db1a76bbcf159d5ebcf8ba67c87687a4ee4d95bfcffbff072fd5996d41599983c767e28281ad568ebaf735" },
                { "en-CA", "f29a57e6f88783bc78c7d528ca2964bf0e78c533c1a471c767cc6dfe0abd7446b60b7740c1003789e300c491bda459e3aa1fb68770e40114c14133386dd41660" },
                { "en-GB", "9069795e2e2a0a37172a217310e88fc776c1db77a63d7947d3ffe8a568e42bd60c69dbf90c3969c8c2a8bef7c6b2e5e1a3dd49be325d7fe0f867a55976fc1988" },
                { "en-US", "c8a1ef9d78e348475b01d1d4e35838ea0282b9ded75bfc72d15f8299ebcb2d5acb0ece3c4eb25c317b67a2b1437e3bcc78f18f8fef70a2c9f2958e6d72b482cd" },
                { "eo", "8591e2cd65c4370e8d6b7b9b144378b1c51050d08a72e92b31b32eea7524cb0dcf89cc522a646dcb875907584bb666d844bbe4549ec2e141f7092dd56edc3656" },
                { "es-AR", "f7db1f5c8d906d6bc572e82db89a8ce60618df983a084f72646fd194388bc8bccfb9dba8930bb978167bb81b4ba3844d2ecb010a159f04eecc661a2c9bc0dbaf" },
                { "es-CL", "c616e056457790bda5c2e269b7a86c27ae0ce5a59888f619dfd4d1cd8bbbe4f9e53367616e910f368c5817331be77496e4838389037d12c1ff0deae307589d75" },
                { "es-ES", "ea25f51cf3040aa231e8cacc97fe2a2f2cb91cbc302d3ef8942d530406f6644e7e31ecb4de074c3714a3f69ca93f2698bcf6c198d0d59e8839b5972d74283c16" },
                { "es-MX", "28f91d878657d0fa9065862308f4c918d1375893ab63552540e8505945002cc1878d43509c8626614cfb7a89ee4a80b77c001e02995630eb95448298f685d568" },
                { "et", "941e4b024d599259e5d09c2ae85be31c1759c8cbdbc4c8187b8ec4ad063d565e15da1d1e9ad02b90cfbf03cec9ceb31fb67b1c4896afae8d147490e81aea25c6" },
                { "eu", "ab519a4bf4b03985ca1f113ef5513cb9be0d0eba94291a6a78a9f468ed31c56a122c1213dea911467ae873ab888aa8a65c76283ef99214ac39779c148ed3bbcd" },
                { "fa", "a2ec4098dbb3e406c1fc6b000a7181a5a5d3d8a42dd813386bb03dd30ae133ac0a1603c2c51f7b4902ad7e9262bed0c3d84cffbbd64efa4da732b24f6e25a9c7" },
                { "ff", "132b78c14eca450fa84b24b71556a0fb8d3e779e785984dfcac24e9e86d79a00f48cd621dc89d0d18f032c35c42d909c051563265e01da066d75ccf2391d06be" },
                { "fi", "ae421f644399609a39a4891093d2e0c1e360b0cf8bdfecf24ca2c2dc00cfc4a1359162aae193042c762e0b308dc776ea675224df721f714b61c5ab3e71790cfe" },
                { "fr", "2339ec6925e2f4fce1f2e4a17e817e8f65418d4746d59aab102f0e8c1c3063a52b992acd203a8c84c6caf4c591fe8f14244bffc6ea3ee097e6ebc5ff7be07b5f" },
                { "fy-NL", "6fc9344b9af599ce3b840292f9e55939f136c4b97f7a70c65dd2b57d30dd4081ddae864cabb82bf54c48ca7a3218fadca67ea4522a21218994e9463ff29745f3" },
                { "ga-IE", "7b371a4e82b53bf2942c722d7d2b53f92c6cc68df3d722ba9fa5fed3279c14f82acdb04ab1a82f1e8837919c5fa81d33ae40f0f1706529c521119496bc949e9f" },
                { "gd", "ac26cd3391c3273db8a5dbd91792994e4947ca43802237cccb04bf61d4afdfc5cfda6a938bdb3778ed1ab02a59d9f7d3be07f9afebe128bd7c039c15c43d99bd" },
                { "gl", "20b26b23fd101be16a09bc162e86ad725a8cbd7bb824f8611aa32f193bc0131a0e82cdc034fca77642c4d4f03860162eb4ff375d2be350da3a436fddfd4f1084" },
                { "gn", "29dd29672bb9654f119e3e71e07e0f9480f25f651c5407680a0b5fc22ffdfa426f3105ae6d808b8d4b1453e783c8d4a56da677735c9c3b090189f25a702c570e" },
                { "gu-IN", "a3af4f926f350df44fdb61d2fcb47e58f6a6ffb64d5b07365a0918dbf27826ccb7a365a0a2996beb22e7337e523b74e7aa0bac4644ccde297d671474be9a9b2c" },
                { "he", "0af71d6257ec700796a00f2a5d753494b1fe59f3e33f29781360fb4cd564123ecbb8127a80e0ade96d799bf0b90dc76d0afdd089d8c59532aaff10c9d2d18450" },
                { "hi-IN", "133d2ab3b7f9f88ad1abd93039fe8145e4dc365b08b2f2bd65581bf1240aaaebcbafc78d36a0de7b060fe95ca68c7e9e7769025b451f7edcbb794c8ccb5411fd" },
                { "hr", "e328a6980e477ab0e0d9952df073b5d8e328ffd9bb9c73c7e5489f50234e55ede87498729b5b46c14ef1a528bccb63c017a972cecb107ac542bacd05509488ef" },
                { "hsb", "e4490697b0200491ece4cb6b0cf8a65ae1e387f25ca72780e2e1d94a3dde54f85478209cf3b98d16a1f8eb704861cc5cbab01b19f9fba6c259dce09ce7f8a180" },
                { "hu", "9576c58993be349d5d5ec1a856522dc8b91c8fbd0eb3a606b3ddf932a69c00930467fc08363bcd6b9ba226dbebf786904232754c6ad58eefa25f99aa744d4f32" },
                { "hy-AM", "ff499c68d7654e0a66de9e3f9e6db639eb7bf588c6d59d1aa270b8da938a3caebe05a7fd464597be74505de2e9416875217ecee205851b1526927013f5ff4d3c" },
                { "ia", "df80da9ce2ac72304a809d7141a085d9536c8d559d6246ec82a9bc3e0612568db8d6d56b733f4726cec08d090189597e92363e1a4fd705119cc8c9cdca5c82ca" },
                { "id", "eb2abd2c1cb6348d56a607e964990b3b5abcc3b27a3c53e27f3900ab3b2a73c04f27ff1e35c7b19497c758ea7825e71b8820f93d4ed39328c2ed12fd05185ea4" },
                { "is", "0842858bb1e474f998354586732521cd6ed0a82106db751ad17168c615fd348402a56281842e08356339644c2d86d156bd105ce7d8061a6b1e4e91d3dc955394" },
                { "it", "3e9fb39e6f241c18d9c3c31b32cea45249a689e957ce8eec6f6c9da4bd4198dcc9aaad03654a028737a7fd6f3cbc677845e4d1f0b83d02ac31d83efb5f721d07" },
                { "ja", "c63ea015d514e8b0573ddb96257e4463dc4efce6b6b5c66d63844d09d89cc1aba133de5aea376e8ff3504ebff379243983d2c9f6ddd4281cd10fafbee2d0b8f4" },
                { "ka", "9f348a30c63556435982221f094a35dbcad06522ae9411f8de1b8c1c872cb5d3af1415931f4cb5c81694973d9a64dced97bd5711568a5ff8176d4a63e6a16070" },
                { "kab", "e8557770146834ed8b98876f97446ae887e1f9a63f265b1dc0402c230b12523f6df85a6e0abee2f2388ea876d86ae898188de238aa2c143205e9dfe585abee96" },
                { "kk", "de61406acbb1bd6d601903b595d1d818b7d241f1116296f4959f3443c8faf571d4c8124327076ff87999f46f9581d5d88094aadbf5b168ff8e9f1b7b9b5f0870" },
                { "km", "44dcf434aac2cf23f25011ff918d3c2b7d4fd0320124509d2ccc7f12320095322c993e9c66119c4a320c85d45ff54bc82b0f4cc476efd829decb47f24f140143" },
                { "kn", "a2d4602fc519c643a1797392c9da4ac22b75141f5376ea3b8cbee22cd99de79997851702848c3305a5c1407e11267fb08ff97373bf3977b27bb577ca9e1e3bd0" },
                { "ko", "2964d59887615028371433cb342bd746f9f9463bebc98b27c70a35bae95641484fc6fd84566d9143e0161c17b4c7877636518b47cad5255e42daf2cb7a6e2869" },
                { "lij", "3cbdfcedbce1e88cdc4531b8af226782cd41446d2ae0dd17768591342580876cba455320e1e23bbbb6efd0c0788e92713d2d4329f6b45e3351677f08b185f225" },
                { "lt", "d351f8098d38c8a760bb99b3fa534b1fb2537e5032f570966a9e0ef723f4f8b80b1c394435e5b56efc58c1a3fed464f9baf5695810f0c97085ae8029a03f9f4c" },
                { "lv", "91cba8b75abb9458a299fc6e89ede1a3e277d4a683b200a6587c24206ccf4de82e10627d1430f000fac08baeb52086a480198412b42d4c903a24618144f69201" },
                { "mk", "5bbb2b7b7cbfb80581263024bdf4f8f8fa927ec81318b71e737a5cbb66326c041a75c068d9dbae3ccd0be36f51e513b793e8987082a3db2afc28c115524d0ae6" },
                { "mr", "d81613f91433eae137b934104322f6c65ba170a383acc7bb9a5af1c6ee8beffc96ebe79f8e9efebe509240bc74b2445db1d5f1dd51826684f10e7c6be3dce14b" },
                { "ms", "0eccd03fb34f3e8e01a91aa4bb7fc215350e15e3b1584489a86e6b051117befad4619b28f49b6198193dfadd63b9ef8892489346dae058b2a0bb03960f6f6a20" },
                { "my", "d5852b73a117c3c5d0ae7d8800904d53ac42bd00ea0e8501ceba7cd2067cfcfbd0aa2df93ba3a62360c307255559b69f866cdba778a3f31d0c89958560582045" },
                { "nb-NO", "15cd7c07b17e9d0a84a2a5699043f7d775264460785bf9907cc647a975307e1b5a9937a273c24d467d1e558cd9b6cad72434262993bd74c36ee8b1c007f32204" },
                { "ne-NP", "55ef4ff300c63392b8e355eead8df6f838ea1f16a18f1b84a6f6c41efb19fba7e9cc24ae5c3da31b5987a725623fd014e872885ceb6a1a5653f22d17a47ae4e2" },
                { "nl", "fe8f8715f71eadcb4d06e6d896e4917bf25f506d8d37b27447b1eaa05f0156b315ec3792a24993a7589a90eccae9686698600c5ddab5e3fa2857f71426602425" },
                { "nn-NO", "cb89e4a3a361bf8c3ad0376419851d99093e98c6bb0c6ad3be730e0ced3274e56138d4dfd23cd7f2d6940c7bd471882fae8181521bfe91a25759c784e46bdea8" },
                { "oc", "dc748c1db0c9a99ceb33ab806b0066e1f133df294a50e2f284b950b1ba5db65c3ee735b832c65b0a5cc9a567efaf4031a25af6eb1751cd24b77ec04029c0a797" },
                { "pa-IN", "0b1108035b00f05d6ec15bb350f50d7c5d4a45120563e4d936bfd9d883e732ed7c4bed75833fcb121838d2934133624924622824c664df661124dc128491c224" },
                { "pl", "b5564e10539923066c9a6fa11ab6387675ecec802f2363a5c19c8c67cdced0bad1514fc83840f53bf932d432961c5151795388d39cb147abeca7fd0335f2ce9a" },
                { "pt-BR", "9801393ba622b4261551b685387c9adb9c1720ca1a965c5fbe702e83bae0a817ab84a3c9218de1408e1d7de760b1ba91b795cff8eaa824e87fefb638877174d5" },
                { "pt-PT", "b7e7b1c2845dc9d1393f8261a22dbb53fcf67b21399cc6406acbf2a31633c9d9542d3e37de09db1c29775b0a6c6b8c66d5e2bba2d569d87255bdc4f36d28e292" },
                { "rm", "1c96bdd6bd9ece024802fd79bedcbe63818861bc948d68462168790bacd2c3036021801de22bb9eeecf8f61ff39af00f6cedeb174ab6ce80cfc57c99bb25cb72" },
                { "ro", "927ad1b494029f45e4519aa78a051062d512aadc592cf133b962c7f500a406cb3aa4346cc6dc8e60f515b2a798344c4a0fa3b3bb0be5240532474f06b5dfaaac" },
                { "ru", "7af46b1d1a2317b8f345f5bd7935df56085947997f8fed3bf12a6ec013203377811dbf19b3aaa4eb42de92a99a5123da4897da2b2bff4a46e02fd307e7b398a7" },
                { "si", "426a3aefc7608699de474ed32d7bcd2b5e2b8b502ecff5a0af3aacff703b600f8fa797d8864008d686a721d6a8e12a7ebdb82ab346e1a9ec9daf225ef8acc104" },
                { "sk", "4001e99e0fe80bf8bb8b26240283d38ef63ceab3ac31477005d189a86b228e4b6deea47941c58c0d18fed4a0e4331cc77c07a9a9e2c1d4670b7359503440a5f5" },
                { "sl", "46fcf4f0cb7d5b8400f691a2849e4f720ac0e0ba1b27a705389057c90d4344239208fa2695efdcace26953954ff64d54101760741df6c59d35e901587e7d25d7" },
                { "son", "677e025ddc7b64f92c6b5664e13896527d5abb6169e2b0150d15530e53037585996b540efb7d73f33f0093dd4dc8278f8f23ca41bfa3ae19011eb8a1f3fe0347" },
                { "sq", "1b0577de171074318faca360f3ce8d8151ebeb1d0f13986b807812e7abca1a429f74db3d87ce7c2d190a176239879f6f5f22cee1a208e76ae9ef78aa3da8a307" },
                { "sr", "1fb8607393991535697c2b381081e6e1dc8b4f31b495f9490d4d6661165b824946af8077c6de3896f9a0967d1a0223dada4677c593c1c245320d4e81edf19f34" },
                { "sv-SE", "0323270f5c9da1d93f56a1e11c91b0853a44920872be3ae0c60a0ec324e2454c6a7a10ee8e3d71c6322fc5773a01f145f04a322e23d7b7d2c9c2990d837f65a2" },
                { "szl", "1bbb1d815d11010659b33387872b6b518180a083c70ae81f9b03c72ae5dccbf37709e565d4c18723c20901dc28f02b5b827499fae39c5b26f2d61bcbbbe6b2aa" },
                { "ta", "099f148bc1813231911de53b5267ad998923858c9c8df6d987c150ae0531670c67febefda113b4fb25819a535202ca7a06db8c00dedae27cccd4bdee1f594d47" },
                { "te", "4b55e16b728425ce0279bd3c952a62e96842ea554b100057d35b303c34eb13d9b59172a8a94f019e3096a2fdad7d4fe1c84f0f8fd37213479bd7bd13d63e908f" },
                { "th", "48a96d50c42ae9bb8dba3298bda777e214715e7a52378a8974671d26b686c688d3cd8b8dbad85a262d488e4c2f6d43e50763452b1d00ac0880dc7c5868f9cb3f" },
                { "tl", "78fd5000af5da51ba5d6e416b661321cfbb00e9f6ae14271ef85de5df90e0f3441bc235016fbb6389b2d891c6c196eab7e1aa880f7a416984991300951385b42" },
                { "tr", "abc85bb685fad7b40b0db4a9a594d39fdf9f4cbfeef3b219b9c6abd48f3740d213e127c29036e365d2c43a56b72e31887064b027bba1b4319ea278eda4d240d9" },
                { "trs", "fbd6e3dc3d964004886703c549fea3021f6fc4316dc4d9a85983a60eee3fd7e21063e4ca5e139375f5261036b4ea8cd347aba8efc320d4fec5399e4cf69e1a6d" },
                { "uk", "53b7f652d701a937dcfdab65242bc7941d70eab59180d434ae2315ff5583bd66537fd58a1052b965cb0697ba363be1c4185c919e12a2d72398fea4f9ebff42f4" },
                { "ur", "eeba78199fbb48ad9cd026b913f8179c61034fbd5ef4d972572bbe7096359a3be74025578a9efd47905ccd47324dde30e628c2c6d3fa16f2cc579059140fcc7d" },
                { "uz", "e8fa93edeef19ba293ae6c663117381efa24512be9f8094764b2a0ce1d378df5a4a6dd55603d7c5d8a54dd610390207169c38096b72d7f2b271057210c536729" },
                { "vi", "8c5fd13faa8fef1652b4841b632b0d9056941e60ba910fabdf503e959c5f4f3ed6ac7fea62eb264f95a0dea295fbae8e4443d8a7827734654ac8e65355ecb3d9" },
                { "xh", "cc252e5474639a8502f364a1fe32322d12aa9f0598d89e8a2e80b05c32f6e60adcfa486dce6ab5ec9e0f77e0c0edf38d4dce4f7e69547c779ca88617f85c8481" },
                { "zh-CN", "9991ddf368130a1bbaa0582deb052558e8b6fed1be3aaa84739a3572dd2319dae1008c267cd912a36a55d42d4784074355b8ad341f92e1444aaa47edd304b20b" },
                { "zh-TW", "a8b257a07345cc8e0be726056e5c0b388c3475818f3794c7d3afc6333ec7ad1575fc3b366402d2dfc49798f2d54885bb95db22df1f5a803c940b6256576079a7" }
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
            const string knownVersion = "89.0";
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Mozilla Firefox (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox [0-9]+\\.[0-9](\\.[0-9])? \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox [0-9]+\\.[0-9](\\.[0-9])? \\(x64 " + Regex.Escape(languageCode) + "\\)$",
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
                logger.Warn("Error while looking for newer Firefox version: " + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// Tries to get the checksums of the newer version.
        /// </summary>
        /// <returns>Returns a string array containing the checksums for 32 bit and 64 bit (in that order), if successfull.
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
            string sha512SumsContent = null;
            using (var client = new WebClient())
            {
                try
                {
                    sha512SumsContent = client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    logger.Warn("Exception occurred while checking for newer version of Firefox: " + ex.Message);
                    return null;
                }
                client.Dispose();
            } // using

            // look for line with the correct language code and version for 32 bit
            Regex reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // checksum is the first 128 characters of the match
            return new string[] { matchChecksum32Bit.Value.Substring(0, 128), matchChecksum64Bit.Value.Substring(0, 128) };
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
            logger.Debug("Searcing for newer version of Firefox...");
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
