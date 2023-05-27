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
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
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
        private static readonly DateTime certificateExpiration = new(2024, 6, 20, 0, 0, 0, DateTimeKind.Utc);


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
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var d32 = knownChecksums32Bit();
            var d64 = knownChecksums64Bit();
            if (!d32.ContainsKey(languageCode) || !d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
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
            // https://ftp.mozilla.org/pub/thunderbird/releases/102.11.2/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "75b80d62b96a7900a6e07fe3d5a7eefcc57a6f09dcfd0eb373331650c3cb000fe92fc794c23c44b298abf46e4d1c7930b5c13b70b7193f7ae3eb5fc84d065f95" },
                { "ar", "5743c583a42e62f4bd6f15db7a920a21500d92c3ad6430d10c7af51984488e73f77ffec73225defdcee2f85ebff183521e0cfb5c7fa2cf0b12be9fde530a8eec" },
                { "ast", "d1d70f5a8886df747386bf6778d825e4fa6ecf77af940cc3354fc544f2d00e901181c022cd4c6b19937ae4df0e708b4f3e978d9e57d1b6faa236171bc4e269ad" },
                { "be", "e64924213c617a1fb9747c1c26ab7f58a9daee58e184265cd8a4cefa792098da4708c1e66cddd94a6ada786048574d05a3bb543707aa79840f743a9a2822ac54" },
                { "bg", "3f51126fdbda1a6cb84e0c500f6d5e539b80d7ed76f24084359c79ceddbe70ed9f1126903380fe11a77e1589555db1e0ba301fa4d194adc8c7c7ce1b3ca1bc10" },
                { "br", "77581ab73506e478f5e33f9650984e8913a2deaebc8d17c89a75612245346e3b4925674dd1513191956269a69d0624bfd81a2c52df5e271d55dcf547833affb4" },
                { "ca", "1309d058c76161161db05dd8e79b4b30f014fe083155746e24e7ddf298c799e5d67af226f85e1f8c106a931770e37cdb62d57ee237222245293f097d3b54a59f" },
                { "cak", "84ea59f13e72d3adbff4f6d1bfa432b36e372c10f04a01c7083eaa3796a745aef303d8d8d2b9b9c7151be32ccf171991c95993c43c8acfec5778f856116e84c4" },
                { "cs", "ca7068717293883d356b1e3767ae1af657c41907ea066dd529d3e9db79fd8419fae53f376f8615794478ae89ba250218cb1922465a2b070e7baa4a4e4f115b79" },
                { "cy", "3608e29700b79a07dcd5a29b42a3a8a9cb0996e42628ad14e84567468ada67e678f8e58f79f23f259f86dc43c02b7625f62554ce036855d2bb4ee11456a4ca65" },
                { "da", "a0526be2963fd9d0b3208f753ab11a709be23d24f00978b6ade6f4630e21ab89401c3bcfced31b0bd138798835d1a800f96203ed728de1b16bf630f0c609b52b" },
                { "de", "88c050bad357ebb6467ec994464ce717c4b226e23aad3c3276f04c2a7ae84d3daed6d149aa70cfce40f2ce03dfb949e6f13cb540a6bab7927cc4921a20dc3d23" },
                { "dsb", "32b4c252ffa9237f9dbc5777bcf8c0035960d9d0e80d0b773bfecc0cd65ab9630f00a7c43382b6369a28059084c455d34cb827b63058681a198cb0f4d20697a6" },
                { "el", "f659c1dcfbbdd078d130383e451e0e79075335c40e2f7a31d14f5f29621fb9314129cb319863d027254d308ada0bdcee45bebf7b5152354ebaa8b50a179b606a" },
                { "en-CA", "24e39968d6a548bcfd9e323ba6a58c24e0e601f5abcc7c706f183079a408e12acbb79bf28cbcfc99a57fa6e10d308f8c9316a577b567da5c371208b7266ef5ef" },
                { "en-GB", "202598ce18ad47f7630116782283779361c58a7626b489d9efb126a7f0ba0aa89c1e33e6496e31e25b4bc10f54961d6afe6d41193f7b52f54e09adff90fc732b" },
                { "en-US", "5a4cbc05c900e6fdaeab4165340aff2bffd28ede7bd00e17962a4c797c245098d4c6c810d5f5f4df8eca296ffe4dd7f6489cb9272a2ebd6352262439f4fd8d62" },
                { "es-AR", "490a2acf8ab26d564fb0a24e98127d9e8268fe94d20f632ea207a07911d4d63fe22cf20ca53896a65e2421491e84e207c37facf9e400b268cea2edb5adb1c106" },
                { "es-ES", "2689769e00313789104c17baf971c5962d109877b779efbf44a96d1c7fc111d6a57ad127f8279a91abb3dca73b00429ee77aad4bad98dda359b6dfeb91977379" },
                { "es-MX", "b15eb619853bf8ce75dfc16ace575b0ab8e58f9147cc6dc1818c4eacbfb979a0d51f0703bef17c51f42eaafa14a639114c8601a5344c4cdb65aa51791b8275eb" },
                { "et", "c83fc7f181d807f2f70d8cb68e852033502e3fe409d7e0a4888db542e7bc50325757b38450d6bc7c09706a272a764c348fa35ee7b23d7eb9ea8e8b85e5eb7700" },
                { "eu", "b793abe0216e4f8795490b9a1518ba4005b5de0558b60ae682dcae0f22c211b35413386d7af5f1acc03ebc5e2913b92e0e95d0464aa6ac873e1ebbb5c6a2c317" },
                { "fi", "cffb217cc5baaf54f43679a1c822957f336a40a186a20aed923992c243d8bdf5449294a0b789277b5a3a2c3930f5a9d93ce4243329993701da466eec4777442a" },
                { "fr", "dbd205ad7e4e145afd5a0e7f64933b2a31512c26e8e362440e4af4b3099b987c044fd10d2017f82c40b2234aa9892af6100dac9e2b5190ea5186a9ea3baa68ff" },
                { "fy-NL", "21b5c9ca5cc50248d4340b5fb7aed4540c5e5847c6ed1f36dea5e074e93cfe08f1defc91e3e68f798ee9e77d29a728cb84cea0c5c2c8097e042916bcd1465983" },
                { "ga-IE", "f10fcbb2df4211ab7575a5deb8e7eefec0a663a06ef9335d2057273e3822eec87fa2de9bb6a88dd791a50b025806008e3a7cde54ab610df2c54bce8b7bd430ca" },
                { "gd", "a02f16f23fb0aa8a5244d673d53b76cec77247e484d21d4c43cde5d3e0eb21cf5154accbac9df8744905348611301121ac908e4ee6a082540a9155d0e8b073d7" },
                { "gl", "e873d68418042cd786fce815888bd46b627765582ece0738684b46881038ac3e30cdb475259d5ae7e5175f014340f34c1aa6d0f0b5585b4797d1bad26583b354" },
                { "he", "655d632b8374267d30eec55c8fa274a34007ff5fbf7562f762e0322702ee7575cc17802e3aee21d7dcf21817da28055e91ed42dd76046bb07396291731a15323" },
                { "hr", "f39b993b5dc8ad7e315b9d8a7d6d64e3072595faca70fd790f7b85045c1b45306aee5e04fc3ad42cd54e419d85762624e04dacc0ff529dd43e0a5844e29cff6c" },
                { "hsb", "916d4ce969c2ffbea3225f48388ed8beaaa2a7b31061f6b475f9bb6ec5bc7167210d27ded1bcd3a7e57c81c8442931c851751150c7d62a94a8758c618ca8d77d" },
                { "hu", "e93e2b3258ee5c8b4d104be2bd9e726029579decfa3dbfb3d5d926b86f1b1be3ce2e49e141fd178d847d2b3de4429877943c7e782ede21e82571b9736798e916" },
                { "hy-AM", "dce4a5ff203c850aa62be34d0680a8c37adcc2bcb2c485191a30ed53c473592b9ce2e7e6c2f2e7d85191019942be4a05b0255ce9cbac1705b7afd1d6a5f37640" },
                { "id", "415d6f34d582c6357ac30ad86fb0f62a3955a16ab26b6e626bbfb188f451a28542e3aa8647868f0e7a5acff93811a5943f13bf9f795dd0c324b6455d82b46ca9" },
                { "is", "df0a5bde531a737e0885e770d39643fbb8da454a1316e0b5b1bc51fd9ddddc2a2de12c8a8e9fb89285221be76257aece569eeecbd8b786d212949a1917968833" },
                { "it", "e1bd4670041550316ff9f6fa46c0207f0f1d395db33dc9ade7a76e05ddd73a499dbedf61446a00ad77f38321d4b29d3e03e5019e05d55f68992cb542c33e59d7" },
                { "ja", "a6c71e0017d1c9466906b4c7b7ee1724c8586a65947bafc12df48d7504735736e7e68d16c7a3a26c41fd85bb7f2533f517b36de5e53b34ba29bcec7ced4400d1" },
                { "ka", "6ffc211732597964041d77a37e6f28413d3b3871eff111620387d960f3abee1057c12274795afe2d09d62be5d968ab05e829582bfba57094c53528334f509b63" },
                { "kab", "5c6537e7d5692cdde90fe969d4df5c839ee686544b56cc1e8b89627c184a07fae2fb66787ba722f3658fdcc4770cc6b6accdbd15a9c8fef29c56bd3854a15634" },
                { "kk", "0f9e2c5adb565af72f1b8416ca9a02b99de7b9804b0a48b6bfc0ecff639366be59437d9aa6fcb1fdc743193beaee8617a39cc6eeacc4fe0b41c5011aca2d8351" },
                { "ko", "dc8af29e4a17c5ddc5431bf4d7663edae89f6f6b9688d79429a8a874dae0bbc5e7f33cf6ad033542df5718b71c5c0b45dff78535a0cb2923d443b8056d470238" },
                { "lt", "43ee8e63a11105f89fd11d14d6889c40e7492cef1ac62f585dc1ed8eeb63eaff459cbb30552d08d9ba80198330386a7e5384748f752772d52b5e5fcd3f076c06" },
                { "lv", "c38da566c80fc83699ee0e90d18c1a811149f0f8f45240dd9669505fe431e6b32cae48d8ecde76fec39dc74b794bac2e84b62b3932f3a815945942846f29a304" },
                { "ms", "b17cbf0290329e717d3044aca1d683b07b073ba5661576e0d0cd6001de241cd4e44f40ad798df58c44c76e0c3005733e0383f9b4be76d3bb10975e72b4f1e6f8" },
                { "nb-NO", "093d96559bd3017f01d8652de4c96e887f3ef87c014f52501857771d6cc1bccdbf878313278fff530916f29140b63a4f71b4ed2ea8a5e8580538759e2ade7dd2" },
                { "nl", "deab53a7abe372a33a6acb1dbe00167ffac6b327ad176f3d83d7f1ccae3eb0ed81c4d9f19547abe1e11f89c1008031200b6e6c88d243d76c069eaabf7afc4443" },
                { "nn-NO", "256ceebbe62c2354ba82928632ba63461dd27d40aa836deb066371610e194bcc5026a433f38650808d89ff3addaf873dbce7205195e73906bf2434c6f7573955" },
                { "pa-IN", "181c35f0a2e23db1fe7b6982ce3f4a75578db527e18910c5ad18fe13565be0ee2a6cc4d398f6230297dd2159317523cf7eaade006649818b36ff8eceed819cca" },
                { "pl", "f039519718a3a5856a07ad6ae16c0df9fe049f31e626bd25afb432c01d0c9fb71382cac2c7555a81e7dcb09bc6bcf517806af74c2ab024cb10f8dbc18b8f0fe8" },
                { "pt-BR", "8e359cb2f569e084b9c314c5d32d8597b3b5c41ff104f40a7b57b100cc3fce72c2b9aa7a20efffb7fba742388b124b3dba54dc9ff0006cde70886877b2cab988" },
                { "pt-PT", "e9863f5e3738ffc0b4ad89923a7706e86fedaa2679faf2890eca405e2b93a97d9bc457aaae114f861d6e31612cd6ebcf17bebe21ccdff3b0743b3a2c6788597a" },
                { "rm", "238dbfc0746ff68bd80edab8b69906a26c9111eb34bdd0e0814002aacf91acc990563672ae527a0cc4dd4bafcefa3b20151dceed39447a29d78201fb86a650a6" },
                { "ro", "d98a1eb2533278e7c3f218282559b7a7d4cc79b5d906b028f00b5d99562d7abc208b7cabb986c935b863916dfbb39dd69dba989b89746ce9af66417593dd68c3" },
                { "ru", "15ea0e5cd3f84faaaa69faa84835ce645667b9ae05d3751a94bcceb13901d3704681679c559f760d18cc515cfdbc6e9718b1d30fb155c9e71f1ee5210fbdd96c" },
                { "sk", "98f7292c1809c8491631d3bd56b6f7de2a07b62d17428a91a762b9395eb85193e35cb56499e4c9bf377b5ac4a51ba84ed3edbb023f6006fb8b67a95b4e5d2266" },
                { "sl", "733f5db0e0cbb2b4e18546cc9de817073071d59de20d559ab6353c618cb317233e2473c456c85660c30a826fa66210f932bd677fbc339112da9ef7aab5ac88cf" },
                { "sq", "05a91ce4f5322ae18160655b2b3f5d67526b5b1e04a9666e87377e046382389bba7b71244ac7e843ceada9c38cf6db22f9a6119cadc00e23afb71f2e7ed7d332" },
                { "sr", "53ab08b2d0c3df7a467a29c9676a5a983c8910938da0bc2d46d15441c0cc8aab6269d15e2cba9a34fd77f68ab483cf896f62e2bb800d88d66fde831b946dacf1" },
                { "sv-SE", "b4af85b8185e0d2b40b700ea0f670085bd12022278371eb76c499b103e2a290da25b1eb9c1d41c2f420b311004f8d869ecff9b314e97932940bfc91ab42a2abf" },
                { "th", "b44a93968de5534e1e1454819c1f849af961cd065f44d3ba6dbe90bc8face7121f6561c972be4de19f15bd1679f30ad94583a1accc557153de191bf4b13eac7e" },
                { "tr", "44e3cdb485c629ba2a8e9f6b7b3aa6362c7af253be7fa6d7f3214658ee89f1dd00bc664ab0e10990de3bed1a3ed4d89482fe39e2a9b2e7aa48597b0ebf1bae5e" },
                { "uk", "f53a8c06df4148d9b6f5428c318d9d4a341a088aea8ab779bf4398fe96ef1aff446d4dfebf932212d10fc89ab94953b91bafffe4596b28e9149c8b2e655658ee" },
                { "uz", "7eb9905482041ba1a4b76b8e856bbe52b3e624d20dc85d59e14984a01ae48f3db9f07460758a835e6d0c1cd1116c21c19e31fac4d09f9b44bb1fc2d112c57471" },
                { "vi", "fb1f803b3bc69126d3f88ea5874104d2e384e349442db7984d6f6b00b5d7562aaf17734d69e7df58cc0d6200932fadf48a93564ad4c187f9bc3000876a4c6c8d" },
                { "zh-CN", "37346e7c447b0ab86dd739a4da917bc6cc098eceb36fd48056003870ed4f76406dacb810a134ff633e15d44b62beb798fe3f23209f69dd5fcc3daad6ae62aecd" },
                { "zh-TW", "4b36f47a0753614943a912c528ead35ec68608e16169987387fe17f5599e0ebeb7aa9a988cfeb7565a6bed93bcbb7849dc48e3f8ed4cc69c0c6018d5a3df9bca" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/102.11.2/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "de4ff5582539797dd88712718f583606a37c6dda75bad28c7df3daddfcb4f32ac9fd8c94e27cc5c30cf42c7664a0a52f33012b368b826723df05a3ff4ed40c9d" },
                { "ar", "de49b75384fe16dc3b0e5806667d79143ee39e93af242a9aed12b0d83a299e475ebef0adcd5483a4b30b7d4711d411ae36c1aaf80ef78aa53be4150f04372c6d" },
                { "ast", "5dfe83bcb8399661b3dd4f86dd95234ab4af5e6eb5ab5f97a4b42368ab7ed9d73f95b5f8df4d8038093b018bdbb17bebf4c618dcae326014b6342892897e36eb" },
                { "be", "2cd9e6db9b8816fa85dc8011824579bcff360ce9ff2d4521a5329142474b72cbb31f414583b7e5b437cd65159ff94a563e15d950f848bca4cf9c5e92aa329fd6" },
                { "bg", "9a5dd9cc2c4e172fe5dd86515fc6d64117a2475d5609533ba6a23a97306c5a638269cc5795f58eb4c71c59da06305e002775835cbe94b3358f04c0acb5caa888" },
                { "br", "3e973bf51af5c7de007bd8589a21a6ba09dfae765027393e755afb25c777b9321533b5e3f47bd4cdbd6d1fa597517f2ce3f5475441a11c15f95d7e20b5d0039a" },
                { "ca", "41165230e53c759087a6da21973d509e2b3ff74f12b3e613de791c26a4364f46a8d38cb144ecbd9e2f61b245e770a60510f1c006c79a946225d149beb3ac36c4" },
                { "cak", "4bac1060578b11de3800956062f2e4ad420cdcf9a0c158a156703e1767609533d71259f079047f83720eefabbd0c2e8565fa6a26804009ea723ab5f8094c1145" },
                { "cs", "57e9ba5652b0213dc67638bcc022e3a9bda85657b273679adb015b97551a95a3c12b54f1152ac38c4b73d9bee8e8d31cd50671b4d82bb6517a07359908f921c0" },
                { "cy", "640e82e3a9a05ee59b21a526095f86f689b2e46000571ea2272691b2cb2d680bb7201ba53a24290f59178e1d4ccb2025a71e50c7836dd0b4d133c2ab2ba076e6" },
                { "da", "6087502e30fe6148f228cef9b9bdf05d626a10310637b69c9cec73cbd9ff6f38d08642b8bb75e069dafe0654e291dd2d7a92915258c5e4995e3bbd1800f0b140" },
                { "de", "4c72e80a33bfa2e4cd224b19b1e10ab71e34ec6383a68d0371f35be3a1e30ed8eeefd2865113e57bc28677eb087af229dd0fe46117abc3987eac7bf00083bca0" },
                { "dsb", "b85c4583c66b22e1542a834a1b42c18b60e2088a62be88018727d4d69c1aabbb059fd268bbb8be4a41c0bf85f7357c45481b87ea972f40819b201f12311d16ce" },
                { "el", "da3bbf7bec0a04fa92e6e79a6040738d0e844fbca142e604376b905edaefdacf61f9e9703793390e349a847203d37e7afd4519f6d6ffe66a7747085acac36634" },
                { "en-CA", "2e90de9c925e42620f2b01e33edf130d17b0131676d87a193ab8df184c6f9c5c732b5cbba9ac255b11f2fb6376f6cee8f4932454e8e1b5ff3d35b27dd2ff03f2" },
                { "en-GB", "da3c9476eb29bd2fd42a25941ce4259528fe2adac038066b71b3a49036ff5d24daebb7c5fde4b8121ac3c29a0d704dff21ae88b3f7c4c110d53d31e4d1a3ee29" },
                { "en-US", "a31cdda1ede2324aa8dbeb24d5e00ddd8f616b8b3e529cfe56094d438413df38c127d7149d22f59db37afac59df36b461177d3de385a733714654d7298d6695a" },
                { "es-AR", "237c306520a1d52f3353b370a7222ebc898fe42733dcbd6744d990ba9e378821f35053832f46b915b59583a7427b3ba69e59ffbda7d1f3b3ba353ae3658bbefe" },
                { "es-ES", "e9494bc313d018c0b9e51a88a7d05a7e7cbc83a6f8891b173b7ee16405405a36b0b9d706348cf2214e189cb0e5c617e84ad89b40b1dde9df1621120dde9d7f70" },
                { "es-MX", "f3222659eeb1f3f0cd77d3d4df95e97b139cfae834e6bf9a7e9f3c29d49ce875d8b9aea5fe00fe5ff436ee9b908fea9e24d1ba921b4bb2f864defb909c80a8a9" },
                { "et", "915e7f8a934adc37c05fe6b14224a9dba184ff90df5cd673511e8e78fedb7c8467234f8f4cd9ebd5f38150bd2716167013b35059cc2742153702c1b481be09e6" },
                { "eu", "f62035e25c2d673408d18d6d09d470a9af3ca6d5cbf8764a9fc950e47aeb090163d7ff9a528b286bcf11ba0f6bccfc5fdfc25e378bb819e428575b6131153cef" },
                { "fi", "2b92062420d3c443257fec5ebe610accebf805dd34f6b27cc1b6e24cfb168d6d53fe1cac8d2fa7e4476367b64ae475e2d302933a054986532e86b78b7176d7cf" },
                { "fr", "1c2d5cbef05d17624bf8d975158248fc1817ebe0bc29276216d8e9665ef25b5fd7aecfcfe85a72e00987fdb34c0e71f7946a9c2bce927b30ced3f6a83f4ec340" },
                { "fy-NL", "ff2648ad04f71598952d8e5b879d2e47349cdaef2766a934a8e379c47bb7852b85b800891076165962f535cc007c5e5f3f396d642474fdd236ffbe74252e8a0f" },
                { "ga-IE", "e2f0beb88ede24acdd1677c1eb0fd43eb29351fa60e2f1ee83583827114805fa06216f2f2c991529057723076bdac6d4a1da29f4bdef5ba25d03705fa6508020" },
                { "gd", "c8a3760c9c5c20ab1ce3b57f3b814eccd5d55d7f9fed322bdb004fc70304b033415ed38bb2311b56ac6735ad21f14a1ebeffe48e6aab960b62254d94b1e5d4d0" },
                { "gl", "86df452c8ea2582c8da16405cd3ec6701dfaf74c2c8614c14467ae6f69b9fc2453d7d2a3a27cfcf97c7880b7fd3b807105a908ea7b83713fabab230c3bf1b6a8" },
                { "he", "02c4fbe4ec36538167ea87ac377596dc1ce6606b69f8965c28ca374f4ba942e2464ff01a9036373d7e9478025adf8f251df4a10b3854ba4e2b4fe391e9bb43d8" },
                { "hr", "27c95c7f13975b796c83420d5c483ea2a25b4d455bd918eb98aa538568b897389d386047014db3a3e98d3b81080303608d6d8c1a409a3a1ed9767ca8926f7b8a" },
                { "hsb", "3f54eb036c4612782f94ce18a65dd9e5f2644705b2b89642477ac2bde72c261c04a8dada366ce40edb4aba7af6371be31d34a6c85c57f15254a7e492e9373f83" },
                { "hu", "a0aa234bd4e0351d0a3b3d9c3aa46462e73b36290018fc774a09b4d56d76edbcd8d50feee5eb455839d13488c5ec9cd872eb6b311521ce5f42c22a19eba7012c" },
                { "hy-AM", "3ca765e3900b2505c1b74b91552d74fa9a550258e05206849011a65247e69ec30dec46c15a89dbb4e02a9ca68ca672033e0546d23c4870385b77fe573ba095c8" },
                { "id", "fc0eee5c2199a078ed2a14fb8c5e4e21b19b7e57f9d2b64ea0eb6230990daed2de67f6a06ed2e7ed437978f58756e6fa33cd01f1a46071fff4c46e66c4cabe5a" },
                { "is", "4abc0318c65971a185858b360a3ff51611a9d298bb3c59b718104ab69985f44b2980a1cb5c697e3a052320e8d91ebf8bb903e0ffd193cbe5299360bffff3f420" },
                { "it", "06c8eeb823b3ee353735b96d54f1f7ad395b885d7e3c233fb1238e4bfdb0218840f9eddd7ce147089c6d4e150b87a0d26a4db8b82ef52e73190ae26001b897b5" },
                { "ja", "f9685c1fbec6714a0d4c332188078652242f680d8bec8e829165936b8a8c7cb0d3aaee105486c64187e99c4131388bcf095f5b64c603294af72a20d823ea7f03" },
                { "ka", "c0166fdb7e63238b86a959d23cd5dacc7b8c1a9fafd48221ae386219857228724a20223560cbc80b3c203131b981960d6b005ee1d1584722c6cf4cbdf0346078" },
                { "kab", "dc4e13cffd8e5f08bdd8130fc9de695e654bbb05dc23925e0738295855d556811eb172f6ebdcf3ba064e28549e6a3bc592be8fa8698dc17fd7f2eee385264395" },
                { "kk", "d9e317aba44c1925322e2bb76352a0811917c505df314eb01cbfec368255e521a9d201abac5522edd683cb00718887169674853d034e4f7be4277ecdd3eddceb" },
                { "ko", "fd7a08c5db5db4fb5492b64621c5c1ec36126cb68c9ffb20c5d68d4c1e675e55536dea46880eec81c677354065f22163d2e279c97cb19f03c4fd67499e9445ca" },
                { "lt", "8d54e981022fd3e96f0a969fc80434cc8c1dadf449c620366a774485584993030716d7dd996edb88173130e4ff97e82e28bd639905d7cc05eaf9ec4e602ed3b1" },
                { "lv", "7d38458dfdbd325af240622133ef59f771b3feb6d64b8fc5317000ded4106b66c7dba6ff110de7151763a2958dc32705d80a233fe1d6df0351ac6177782eef2e" },
                { "ms", "9103ab2a8119f59efa962da6e56021be40451ca870b05d1ef58bbe601c609eee8fe6825b00412b3a7f234e2de311c5904b46764ac301b9b28c34b23874af7ed2" },
                { "nb-NO", "6bb1762f7b5797e6210c1cef7cb2f56c70275300fe000a233aa9a72ddb596870d6206b297d52ab514fa7544dad95fdfbbc69afea82d5b1ed6572b4edfcf3dd64" },
                { "nl", "1a60abd0c8c742110caab7f01c03411c55710df26d0709f1c6d395eed2e8e6735b75f22932a86e0928a1dc5bcc52a29169143ea65d65d70f90ce1dde2870343e" },
                { "nn-NO", "15b3c9d7d29008edaf59ead91a1bc5557979fbcf7d8f2c35b6edfacb4ec05b4c421047cfbfc03813f1e939e890ca5003fbdc743850750ff771f3d91e507e4d78" },
                { "pa-IN", "9fb6a31d3d5751dda46b3de1b7b893064069aff75a94680890c362404ab6768b9fb67e509470478a52ddaea8383759452f97c44b5d51d988ab8adbca588d6d01" },
                { "pl", "5252571f8f6ed2838e70559f32832cfa05fe2ae589fd64912e84fb16366ba9642cc7717696812d38ba09c5cf01db77342642a3aecbd97087854486bac3dd464d" },
                { "pt-BR", "0b978af9ef14eb3ced1ac5672c4925921550a726df39c2fd30ec896d0fe0afe07db1a3ed6b71e0e8bbac982ec7a111625ba42c2b822731e58e6e521f070baa5c" },
                { "pt-PT", "8874ecc75246a1acafa210147ccf6f11475600e39c759bab113c8299530806d0eda1dd4855b594db5974627ee61cbce74cd8bebc50b93222b6d85a05794bb18e" },
                { "rm", "32cda1194ec9f498884252d755a531236c6c7e94d5dae5571175d17dd97df1e0fae67ab6189141c1697e14e303d9e687b25a9e99c7ff412706b862828734e339" },
                { "ro", "e4213a92855746c541754aab9116377ab582253c65f67e72559df220d1b5b5ae118c4324abd715b6d3f4177624dfd4bad57bf65cab77b211b944cbc8d9141809" },
                { "ru", "42c2bf8fa0102ff1f41918f3a9cc3193758c3de04cb412796a2ee12d5d350e1d0399544da1a91438a156bc6b96ed0c338a1651ebb68e86646d324a5a622fd243" },
                { "sk", "4d7397aa61094cb4f85895d65d1dfbfcf9fdde035b95388e991ee92e6c7b5a95fe3ace37f37fba3e9ab0f6e038f6920c38a3525ed33558171c68abe3b4aae756" },
                { "sl", "0fcbafb601dcbe1e1bc9daa054b9bd5cf3797b743e6d5f3da004f299e37fd6a5f9eadef5fdb81fac09d6fae136de35507a4f599bc07475d81bbbc0284b0f0e16" },
                { "sq", "5e69b89ece00d81230f160e560abcdb80b2145cf2a18a5530ea1f453bf33dbe85d353977a1db431e69a4ef5997aab5f461d4cc343b9cf97f38cba4491b62c14d" },
                { "sr", "1039e4e6d33dd73fc0c6023e8868dc4b2d2790c6c5fe004d6ae628b9f6b736e7ec09eefd5c35e9a226ff49b2162af7489ba7e140402a172780a7fd92b0953318" },
                { "sv-SE", "54c5bf909f666593e10151fb201bfe444f85236683580e76f34ed48995e6ac9b5fe49e65fa95bf858e004532a9de8010f124997877dc30fedd8cee168f453d9b" },
                { "th", "9a7b9318983e7a8d8e9f194f23b526b283e2631fd955d1a85698a6d8250e30445e561d91de2de1276340843a95ce4d2221ce721864cbc6232dc3fd4525c2cc2b" },
                { "tr", "3cff7b0bff8d7cb055518a4364aab1da51b05fe0b73f5ac6d9494ca0745269dfe0df0eccd13696305d79e3f06a59036946f6f30a7b5ea1192c8f091f3e0b6eac" },
                { "uk", "dc37f4091ccb7100f9dbedf1ddde8cbcd806c00db551b6fbb4889ba3107283f6357155772ec98e33eb0f043b19915bb4b63c80873057fbcea0e23405f3f68d14" },
                { "uz", "d982d46649899fbb7f3b5da439cc7f55d398f0bf54b3147ea71071e6ed3559cfe201863ec8c02146456e258253dd84ced24fadef841769688fe1f062d57adaa0" },
                { "vi", "2dcb763a64475e6586cfa013d24afc1f1de92deb201aaebc823ba98e25750198d6ba82484b14189dd076bb962b28cd5bd94bec6e701236afd256a8cf15610624" },
                { "zh-CN", "4fec8eecbc17c5470ea946846e779f9052d5077888a58e060bcaff13d3f07e3b0648d99203a789df0951c3223e563445c9f205a1f7e682e0363369e10046e1fd" },
                { "zh-TW", "12caf900749b6b906f7699779d9fca48fc859af2209d047371930821b2b58909565fce5040c755c51a06617ebdf36e89f9577c253222d30085dc95ab406fe707" }
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
            const string version = "102.11.2";
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
                task = null;
                var reVersion = new Regex("[0-9]+\\.[0-9]+(\\.[0-9]+)?");
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
                logger.Warn("Exception occurred while checking for newer version of Thunderbird: " + ex.Message);
                return null;
            }
            // look for line with the correct language code and version
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // Checksums are the first 128 characters of each match.
            return new string[2] {
                matchChecksum32Bit.Value[..128],
                matchChecksum64Bit.Value[..128]
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
