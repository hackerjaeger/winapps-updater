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
        private const string currentVersion = "111.0b1";

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
            // https://ftp.mozilla.org/pub/devedition/releases/111.0b1/SHA512SUMS
            return new Dictionary<string, string>(99)
            {
                { "ach", "83ba6c82d51e133c76f05692112bdb56ada42f0f920d2a9ff5d48f8adb410b62788ea5dd17c42cba6a0d0097a8ce710f542795cdfe4444b80dbe8856bf5ce364" },
                { "af", "a9dd65ac28256d578ea2b678849cecd13666b7125d4f596de94fb2e9cc7b603c58f0cbf51b6719aceb8e76c13776345144887691e2db23eb0633db19b49e4683" },
                { "an", "a26f7dc47aba16f68671c29cb3841daeafd8c74ed30f0860fc33661be689a24a391fa78fb68a49e3a9f7cde7557e30245b438e45b2edf5275d140a150a00dcc3" },
                { "ar", "5261daf139f960e0e4780fb24118d2078b36131e5fb043a41ecebe6eaf55703bc0555d3364bd664b9f9e90d70c33c0f43475c12afcf5fd866298906d5bf5f59e" },
                { "ast", "06cbae01caac9699fc2ecaed02b5cbfcc2ac3d1fe98bcf64a8852084ae222ab44b84866014e1d09ce564c03177bbf49e9b560858b5f9e32b5b9f910453494c2e" },
                { "az", "1c1397d2bfba7eef756c21ec9040055fd663d7ac3a71d23b4b17274fc180a0abdcf75dda890d49ed3dbd7605f7a6a3b036db9d4cb943c6178135abfb1c20d6db" },
                { "be", "2083b95532bace8e14fca279907aab78513fdcb19ea9f9832b3b504a8230893a2c6403c9a18eefc8f1a557ad09778b06e5b185393fb95fe5bd2ba7c92025046f" },
                { "bg", "6a599ca0b35e37618839abadb3f2590c59cdc17141bb8f4bfff0dabdd4ac9e95292ef3b189eae215b8738cd1b3cae000ba25592007e79e848eef63b1b037f8bf" },
                { "bn", "7c5884362a2c99ae4a4be88ef010539ae57dec65208eb9d515916076e1721e6790b5ca6045b428240720eb6965cc50188a351d33a023b0cac177c280e316df78" },
                { "br", "cdca15e7640c269ee05eddb34eb0d667f65f49915201455298315b243a10dad6fd49009ead3cae87c1e775313a41693b887e1a417f63452318c0baea4b25629d" },
                { "bs", "45f8428722da8e036c54525628ee99c2a66122d27953eafbe75bae0457a3043278c58868d16027f460b248d5c4c2d8933fb5a1c4ecfd5c83518f97420df28c95" },
                { "ca", "b78d5dbfb9406f768e6ad9a25cd7ecc55bfdd126d93a19fe0226bd6ca38a97e4d7586e94c9c8618baad25e2a76fc0fa1f9b2a15e787504903b92bf7c43545142" },
                { "cak", "557f0e87b29db8dfd98b72de5fd9d37d11b077645eff435f7eb79f58edb9dfb5d4154f2a20438c9a908c796f478ded701c62f8593ed067856fa5192eaf2f3da7" },
                { "cs", "e8225b08f260a0ba5280a8f43b28d6d2875c1a91d13226b6e0ee8b9dc0b06cee4300bba1e6d0a001b51f765e9be43db356bd8f0b29a8ad457add629317624556" },
                { "cy", "39fb192a28b0f5ffb0c50dcc5b189de7d934c3e5bfee023b523ab6bfb660444cb41bdddeb23d939694d2450a4ceebec280623242dc0e7483d9a7a55510d28f5b" },
                { "da", "1a1af73ac036da201dce4af6498d946874c174f7041bf2c9c027d27455163067a05eb99c364777d951b139c89558e0a85c7f73eeae2f6656b30d0b89fb602fb3" },
                { "de", "5cf977859402c518d66b96948ab52a00eedc0a382ba61ccfd4658984cd22d524a7ad1a6b77088146020297826db1b476b0eadc8fb87074170122c239d7633475" },
                { "dsb", "99d05399db96e72107f61f0ae3c281fd40386d5665685ff58781d0d80f08dfea5e1460d12b937baac408f3f4d413f87611726eb33b71f22236ef0510bd931ead" },
                { "el", "6463ac71d21676d221f8e9c0caf42c7525d208e3221a1a7eea166cb0e7344283f8592bf518f76c875e4e9bb4d9c81bfae0df592d0bdc9440645e86ab3849dfd0" },
                { "en-CA", "c01975e8c2fac37bda318061b7f45773394de5451fdd2d4c95e477cab740fc5a91063d49be3e54326fe8b663a3764d5a9d2396a1d23f8e0efaf880d534d649b2" },
                { "en-GB", "e2d1fe15c3f86129544e9c097bccec0e40860507a1295ff7ba44916aee4a04ee7b6406460d775d49f7bbed7cbb397c9a889fdda87b7f4cbd5246814ed3ad91fc" },
                { "en-US", "847ed6f3defc330ab474065b9bae911c619eb68342417f60e2c675237ffdc9128cd65f26de9d4de62de621eed5a015cf8aaffc2b51730c63e10c6fd19a0a7367" },
                { "eo", "489f1e398c55fd59b3d9225cfc9b89c92640a34f5834808c69b85c644a87cb11eb7b848ba08274d5d09f73b3205332edffd984009a8aebe304ba3b39d906cfe7" },
                { "es-AR", "bfdb652e8cc4d017ad1fa61cffae822973856161550bcfc2e7f10e9f1be2b624def6969529516b9318df5e3b9bf954c4d6c4dfd89661a54249b75937b90a4540" },
                { "es-CL", "4b0710dd01de4c7ef7c5c27ea2996b2f4df63814dc6531f7a27978b2aecdacbe15862ea0669249762ab2c9516f3e0703e99ed6c7120f42e600303a1e78e6b238" },
                { "es-ES", "e868f8b6933846e5cbf2ca3502747a74ea5221d5c8580d3b9458876879683c75fbf03016e3ee24ac81783a9c7e0d1ea865d84ee9c6f54f1ce362b64f0d24fc61" },
                { "es-MX", "d2ba6cfa3264d664011ac75090e3a25536ba2ca08b0453378fb4eb8c68e9e674a9b86dbc644a6b9f217aac691e8431ed9f14736e1e445987f7d55e1ad552958f" },
                { "et", "cba8d6f9cf2ddc405e5c768a4862fe7340d8d7045ba198d2335a89a2bc158fc4124b7e3a156dfccb9e5d6d072b210ff062221e4d83b5199e881d2b70b16efc8e" },
                { "eu", "0dafccaac7a53257d991f345111e011ad6279f06f6ab939f9d4c3e036a9ea78a5895ec095d4fb7070ca74d534e3d9db35a30960da5990cfcf8811b5173791368" },
                { "fa", "6ea33996f84819f4d9c39000d03b3c631f9ffb3aa1d78dcd161e7ba22240589c465c5c46a52d99eca3ca9d7e155e4a8e1c6408cbb84a21eb77efd5f77e672238" },
                { "ff", "bc4c9531bc09ab5192ea70d3c00cf8294dede0d527367188bde12156b9fc34bd44414629048ebf2ddad9c74903debc163294737d8cdb2137b3a85a0b2b94f392" },
                { "fi", "3912104d9dcac0b1bd4c0add3dcbdcba2209f833a01447349283cc93bf3c171847f7b3870ec6504ffcbfd7863d355507492b84fedc8d7046a3e4c78fe44ccacd" },
                { "fr", "a3a9bbba4091e9a39806847011c14ee26474e3e33120d9c73c22d18152f55a05a268bf30c46bae1dab19b39b0c30be92c2140dd510e40fff5fb0e4d5d4e4e8d0" },
                { "fur", "fa10c9c0406123635547095679587ed51687807fa8c9813cfd2653886e22a8244a6fb90721c71a3be7e24a171862740d463b686762aa8c7512867fb003de1b5a" },
                { "fy-NL", "4dca0186c44051f3ea8d086bdbb8389d2db7fe3b08b350551b0b22ff6d4047a501dab33988577e5d274a7347e3d90426fd580c23712d0eb6c031b1ad42a9088c" },
                { "ga-IE", "7558ddf453041a2f054e2a002b3df4e49733a403ce383040bb80520730849d569dbfae394d77864f9de89f4a11c12bea81c8dd188d41ebea3131942d144a073c" },
                { "gd", "db55a69e4b7f6ca9efbbe7ca1d33826cb725b29274fa148d4e2d3088816f2f5ff41edc51f386be37df55c8521452e539393aea45d93ff76f63624a6a211c0ecc" },
                { "gl", "7b38053732f78433c4990f45cb2d7d5d2f6a004160c3cda8dfd4f298ad52cbe3f009faabaeb54107e009c18b4a6e88cab4769ddbd810226b11b25837417d3cb6" },
                { "gn", "00ab104196a2af30c767bb5e1453fb11652b59397b79c2d3f2f96a0ee41f27c58ece30dcab0bce539d9a95cd99b4a4d0b4b367fd62ad3e46c8c5b09e4e02520f" },
                { "gu-IN", "99a23d80d6145d2172a3cef1e59a3ec70a3ec6731a5a281e5f0982c76c2efe5095ceee52792976947efbf6164a6f25768277f428ad5372dbbcdbd39def0da067" },
                { "he", "a877f866f8228b483746b889546f79085d50c415c547ae1f0f0865a023f0a2565a1817324525a11a21bffb73d89b7acff9d1902f8b7452e1f860741f8195e42e" },
                { "hi-IN", "0ef73b252568737bcf0f52eb1bc6abaff1e2a0cc2268f3c2a6aa27da44abd0bce5e948582f6511250ffece4f23e832481191c3dbac12e94344359b7d92fe270c" },
                { "hr", "24d5dd4f5025834c05d58758cd5aa9066291edd95c5fa876f2fb2773c41dab57d7b2f3c6b0de1ba65cb5674012bc647b25fea3f6a574ac77c9f30b77bfa60b76" },
                { "hsb", "783abaa7182092a13932b0f17998cdd2b6c0a9005375d9de234a6785aeba6fadf77688f13de02aa7bc26b7fe3efc6575370f5c9d7555e74fac8eaa5334521582" },
                { "hu", "978900bf025cdba1961359d84f4c17d7d95027e8d6f5a1f8b32fe163d582c4d6d8006ea3820a8300e15fc3a5ae6e092e04dc3f289605d1aec8cc0cf864a5a9e0" },
                { "hy-AM", "82438994df7e358c4e3002426773a7d28af7d48829763c8c8c2c575132348b330a9ea8299e5760dfcb5820f99d91122ffebb54b9ea70d01dce1fd40bf13eeac8" },
                { "ia", "5eeb2b53bc5155dce2ffbd32be47d6bd36ca6e4294229dcda56b26cfef4fa27aba34cdbbd958b31ea571407b59033b149aea53ffaf01c2a90936e206748478b4" },
                { "id", "85fc0a8e0de734477d4346eeebe044b4846781ac7003fb3c9b74c9ae887a1ee7251c035f39382e1168b8dbca343b42366a90885e1119a2b04b8642bd768a120e" },
                { "is", "078767a64d971c1fbb16c2ef3469ff37f2f5273fdd9463d01047e9814b68bfeb84527caad487d04df13c3250123cacaa409efbe981c9b16747d7feb83ff66393" },
                { "it", "20c07bdf166da973a4bcebd36cf08b1bdaa2b405ee4185b0304ef344a580305474eaf4064489782e6b8bbe206eb2faf080a3b83ba95fa983044aef00f2af7c2c" },
                { "ja", "c46e1c94211aaa3aae0d16648de133e9083f44fe11711a6c1ac4135dba94135684f5ded5231dfcb456ca8a317500826cf47cc52fbc61691f5d375b31ccdcc51f" },
                { "ka", "b60dd7800fa2faf655d78b0b6b0bfe8e48e9e462470177bce77c6dd5f4cf4a1b18dc7e13cb680f7719fb82018452cf19dce525cf2e1ae7e21fc449c20b3abd15" },
                { "kab", "0c2382ada512f4403cd48164128180e1498875872b1d3cb69aa0868009be559ada93cb98058999e9a63359ad4018bc03f15629c14044f1a7ab6c75ecc9973338" },
                { "kk", "068c68cd00260cc12f58935adcbd7737ce31dff4475425d26a08441dbfff11e94e0802393daa1f732be9e3a401127482ca86299d0fc1984288700d6e29dbe07b" },
                { "km", "ad2f22c81759bbb0549c9063f95c8099da3d5b5e8437afe2cb3d70df0d28f3e17da4d74abfddba94f597953fe4e8307085f5baf6f96f833ebc0be60d17b71c33" },
                { "kn", "e8795c58b03174f084200bcf21160c903abc191b95c7f5e882c405524dd1b53eacfea83190f102c33528e0f5762f9cdfd57d083573d81431be488ede244b01e6" },
                { "ko", "1e0659865c2c2014e51e793cc9dacfa40e75ac4ac17b4235bccd0afddab2e1e04d3320241f775da51949b1847196fa117e6188d699ee656066c71ce5eb90d5e7" },
                { "lij", "00fe79b3844f872f7ce8ef1ab8c0da4f8414fa28198d4c1d51944417f4393a58eca59c529711ef1ebc78b3219f5fca32cb9f9dc37572a48f3bb14df0caac409d" },
                { "lt", "6ef83ba1fbe75bb1e96bffcad62f1ae85fbefe3760b0d77488beba881b43ea3a9786250f9419a944056c81ec8b8b176e81e76df22a3a5ef2302d1a155805a50d" },
                { "lv", "2feeabed091bde8954f2563d18d70b986d5d9660441eda673ac13ae75dfb7be9b17f9d176faa55caa4115c7b4eb2f300ab28f98bb67fb64b98bdb15ac06ad093" },
                { "mk", "2d7cb1dfd460fa4dd1e0e43a20b528c8cf30357ec6bd9e77b775cad72b5d246bc4c835c15db8f466f3cb25f481a224cdc3a4e54664fa71a120563bcc032f7c15" },
                { "mr", "00b571058967d67d5d7cd558559410e2f4720691e9ddc68392fd624b1d86c465990052f6d8a1c69998791c2f94e3299e019d48ff30728a16705f52d61071a468" },
                { "ms", "5d0a26dd583c7b73e2ed53da94daadee708420530bf4be0f2b50e555913cec6c2de604b126ce08e9d28e4e501656469dc635443d50430d262ca9a4e90aa40111" },
                { "my", "fdb4a6a07b18a6ad4b568f491f73acfd6ce5e180cca175e071eef6637e56008b07232fa6b9e6cad751e478425e394cafa54040ddc3407f1cfd447f64133cd35a" },
                { "nb-NO", "75c542b36089f19f7e36d139aa9d70dd1b2eddf8ae8dd8e9ad3807bda24254ee802218b9f45a7bdea69d17939b2dfe2618e7e186d2c5a7a39153676a114c54d0" },
                { "ne-NP", "01a5d5726c0198c5b84a3446447b781b828b0c549243d202f3a09cb55177bc0cfb631f7290cebb16ce54236047af9ec282e6d885661cbccbf3e6ff68ab2d3613" },
                { "nl", "661f84d2ac2f013e0ac90322ae693bab19df4dbd82e1c2d2310cec5d10ca33cd9c8947aa5c7b065acb7a82b002931aa88e3dafbd01cf360039bfc489916d1414" },
                { "nn-NO", "339f14f18dd00c743293d881879886abaa4e985ba7923b8cbff021a5a7ea21bf041a989482d55ae6c89a6ede9d7564c768afc3c15a87b8d25a80978cecc1c381" },
                { "oc", "323c926a33b369394bba1c7d0cd314d5ba30ac48703002e89ef9ef35cb97c73dd59da51ae01e465cef82511e1bce3c32380a0043f22e26734e2683675f590caa" },
                { "pa-IN", "d8eee2986ea2ab9f64932d58ce125f04dcb8250505ea8667f1483db180d2279d49f24b25e66bbd20ff2f01ed972f11f503a0e6d8095cb7546903a82439faa112" },
                { "pl", "558e0987729f7f0459d5d537040aa633a4ce937ce0c53133d529aad6117b465416edbfc38eaff805d0a2a5f625f6595ded2c1075035bed2b5bd995a96abf7f8e" },
                { "pt-BR", "36623eb1bc4b9431a4e081a3338f753974289fcb6b835e6009ca07801c57fdb2d1585b7773a12f1978e19543734b07770329f8d6ea6d950d4d33acdabf74dc7f" },
                { "pt-PT", "72ecc3062d10acbaa8c9381c867a52777269069624bc905f590c0339370a16fe4ef4adeb9b3f6a094425a4ad3e4552d4e0beb58d258e8de7461837d2707fe6cd" },
                { "rm", "d3517d07290670054613fe0419ca1ee00b2baf687d6df3875b66a3834fd0fabbb130cc80a86ed05fc5f4c686da5fc88b6b4c73d89f335f6208367b1ebd7c0100" },
                { "ro", "f506eda00a884dc0b7071b84fbe270daa6fccfc956978a1c9600b9e9a7f127f5d852983284015511333f61a73cbf3a237f22ec7ddfb280c584bb65da76b2015e" },
                { "ru", "46e87f89ad38c9cbf2c904a997478ec8cd7e8ececeb41546942f1c0b4d062798abb9165f02a0507fc7415204a21e85564a339417acc3919d86e0b7a455c1d983" },
                { "sc", "d7ffab6e115b5a0a5f0defcdb738a39ca2c1f3d0b475e70bf1d0dbc1cbe5a4817186c822a45298eff4e008198f191ca3c63dec9fbcc2203e44bb3fdfcb61ccd4" },
                { "sco", "e6f07babc0760909fc1c5198440888832db659f8186a59847fc1dc7c4578ea7f8ec9ec0d13d3ebf8f9a45b8c87c15cbfa6ef95dc6a1039e16aec16f050705b67" },
                { "si", "e6cfe7db55a2442913d31551320b29204ef62e7920f880659fddb4ae19e860ed6d8e223f7c0de3ed1c90a8baeea5a743a23802ee47182f7bf24b64cb037669e0" },
                { "sk", "8a8b08e30ad030f2e31c6a7d442db785f2fce368e71aed877da49bc236b3ed37c255ee73c02c2cd426be541daa6dabc80e3b431697cad27c2c03fe2d16322b6f" },
                { "sl", "7bf0e9616d80ceae16e6f7bc64edfc868b3c0c5928a270c47edd624a2c8ede15f7f56bf76aa2ab53b768f14fc85313c5d3a83d086e6b999632cad34e537e5d88" },
                { "son", "ad861612b0aa1de9d46c580f162e03702c6948a9700ae96e7979e1fbbde77767b7be55e2d1eca8b1e8ae0d18d9cd090b9dea9d3531b6416acece69fc05a49bf0" },
                { "sq", "015e4f054285eac6f5fda928fe3182951f59579d1ba72b73713bede0ca3d22513f0ea812406311afe278cb1aa628cc1ffe43281ffb060863483668c2d706ce28" },
                { "sr", "93101b3ff9a6b96d92b1aa6a6a6c0be38b50e2b7004ae5297bebf37aef43938328314499e17c0976ce3e7d0968439590530e4ee8ac499abf64fdb07d416a3b41" },
                { "sv-SE", "b89dbe1493b9934153e942a57888269ff591ae2c84e12ec0f4dcbff9838504b8b2099e2e2997369d591c9b3db8ea15776630dd0a0dd7521fcc6eb7c1e8949880" },
                { "szl", "850cab9522f9d05d4b5813a2ba372e300d3e6b10e4b0a6bc6a3a8ca006655dcbcd85fa82c05a9523a9352ae5c3aad66a3f075640eee91235e81b81541f516820" },
                { "ta", "74795ef59f71ea186bfc684abc90ddce10757a3923566878dcaead3b19584e7129cc8d753905243ac2cbf6919f2abeea7e37ca6310dc1576def94e883c0319fe" },
                { "te", "2e105bf523b7c4a2b9309fb243e9168aa28de7df33e62e6c95c7eb4500827d759506f030101f6f2910cc978ebcdc81e0dfd6e960a9bc21e9ba60a3a201d70ee6" },
                { "th", "1452bfeb7637923d3f2af4421ae33468eeac30c3f63f690ce0167a6c4a88611ad6ad04bcc69e5af6383a0f3d58dc4f4b2388c29bdf5c342e3de4dd8d498dcd15" },
                { "tl", "0b5482a913f39c559733f80e4dc8af963e5719c06b4430b8bd0f846ed4d3c865f462764d529c57ebd25304aaae718b52eb8fbb8f40da2616b872671dee936767" },
                { "tr", "6c9f82289ea9a9ef86892bd085ce8a4be65a6c0c643d2bc6943b7366327a0e07ef4e7c51708430424e0a1157fe2213a873670fad0e353336441d083d9f5a8870" },
                { "trs", "e2ea7fa3751e8ef51a024bc12ba847cf50f836120ff32b958259aab6d27b050af03793300cdbceb936b1d8a5b7daf29ec1c8f52a6af9eb17a08716fdc5f7080b" },
                { "uk", "aac633585f79ee386709d39bd6913d98b736c75b0738d06031a5cf6da4b333e53411d161d0bed7e85c319e2169cfbad8ea839475cfb18824279429f5aeaa76a8" },
                { "ur", "a30bd6d5750842e297b2eec39c60777ce7cefe26f45a345805b9bd13ca72dd7b889df608ae0389e45494702981b0555280347b9d4671f635f980e1c48d460f28" },
                { "uz", "631ba9a2bd27166de600561b4b40d25a403ac9c160deace9ac5c8615c0c6c280ca97de409099b0407bd7054f4a8d8457cd5f91985440a864f77981ced3c3ec16" },
                { "vi", "4e7684d76d5b0ec845a0e8424e186b13663639d6f7808e2e2b2730ad46a850fcb8741f8954b8c953b97388e1edd4fe5f5d874568fc4e86883725c0ac781cb562" },
                { "xh", "e005f4640113d92d5e334d83098276d0d6ae708a5ebb7d14e0658a887c640a434c8e320d660be1e3128a6060c8704d897e4a11e3bd638e0732a90a78ce26a015" },
                { "zh-CN", "1ed06ade949a87092586a0ac3639eb0a9c7d9c4bc940b247d240c6fd41d9049ec8f61ec6fb85a52d647cdfd456b35d6af32c2b76a02eb66a10eec1e53981c44e" },
                { "zh-TW", "2ee69ead227a360dec77c1730de7fe376309b7db48ffd0e52eaf86ccdde82b8362bfb595e2216b32f73df051c815f09d76f9bd3541d3603e78ee8de9e4cf6149" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/111.0b1/SHA512SUMS
            return new Dictionary<string, string>(99)
            {
                { "ach", "ef132bab8e35f11d9594b461cf9f92819f6ead1a729125788cbed01b31d1946e8964cfb48bb3f8d13fad85989622619cb5b3ca4848bea355d909ed573ff365f4" },
                { "af", "b2ca0cf55dabc5dbb5c0c155df1fe6b77728cb90356618f162a4ec42d48fc8c868a737850f03b0d3ef049dc0b3c3928692bee5010dc2919d2f14cfb93e9eca9e" },
                { "an", "aaa238ce1f22b4fcb8e54b87d70acd8a81bf0203ce3355ec62874d84c64bbcf67c3b7a9bab629833fb57123506f633a75a2540cd336b877bbd9e4a5372a92118" },
                { "ar", "aff0cce7a61170ad28b8fb55fff8e69a1c8cccf466952acb0ff666e777803a1dc23d63f5e1f727b1512bd5501c12ac629ed4ebf6652bf71037db30e9af1aa172" },
                { "ast", "ca606a6e20c1f27a30b0991a881bc5db0e358c4e4f35a2a3524e4b26c79a77ccb3aa72ea760c8fc9637d226ce9f377e56faf0a7e21d57b1abc4bca33fdd0a547" },
                { "az", "c477cd57ac19678494c5162873509d6d987f1861b139ce09aa2927e22bcd2e56cc841e118e077ea221c0efe315a630ae7a8bca155a3aed937e0ca1e808dbed86" },
                { "be", "b17426a9e5c7f630e56bc3a1c30ef20a11ce2d926d1781f946c3face20324ccc3374911199c589a6dd82c72ce1cf78dfd576bab093c9d61c88d1675578619a89" },
                { "bg", "cf51d5f7c77ae855bfd0467fea76ba2ae2ae0e61748db3f451840a68442a6128cd469aff4ed6ee868f898f4c5169eb1cd6739ba76c84a9b06429d7076ada010f" },
                { "bn", "9d3d5ec9f882a811cf7f8d9611b05801f5a2419c5833db3ea616afec2b7559409a9c06d7ba7fc6065575e8b3a6549d152836c920e0a8f3ba74b6a05d46e79306" },
                { "br", "1d5bdeefcd376ca9c0d8f7b9ebfb66f4336753b1d0be59f28ff42c73167e640a6039195722ddc06d7cb7e9e3f85b7ca54c6df6b19bfc9fa64a8098a144dc7774" },
                { "bs", "ceff16309917a61fb9838f6cad99cdf190e2b2bd89220e2dce94a29039f2946b7e7a9f17c4b87df0075e8b953a1be655caf46b8d3884248fc5c8769dcfd0b22e" },
                { "ca", "fc30518242e0c4a03e90a306a46a92c98e33ef49377d2e834fcbdd2d7fb173e7880f170237e0d877dc70b728bce752d687b634c98eb100a5a38d23e729ceba27" },
                { "cak", "42d5e89a70d2ae2afcb17f3c6ea13bf63780314453b963730f5ee9c796f0ac56f8162ab1ddc42f5cec04748b51bd9f942b2a5840a33543cd5b920cfecccdf22d" },
                { "cs", "599cb215378a2b160f61726e69bfc7d9f0483d18035283e49d67867c0646cf70d3918a94e930703d816dfd5ac961d3904e208d1c0f921671c0bb379e242cbad4" },
                { "cy", "d03cd18aa402b3e22ec42e4d8dcbee72efc151c97c1b54fd3e9d9517b2b4ef7317b852427353c1b3876d7c02ed368feef830c0edfa4e06d032260c82f042e87c" },
                { "da", "d8e8b01b8f45f5770e5cdc27a814befb7299968f9d15f731bb8c0e5913cbc78922cf7a193807f9f48b918c30e66a08851b2e118a5d08724088847fb65cd77d6e" },
                { "de", "8141c927691320fcca88def9502467083974156a72119599fbeaf151a71f8760a0cec2246bc6c25c2a0343cc6b20610837bebbc95177b97174103ed31241ea16" },
                { "dsb", "fc7af7687c07b4da7b2970f71db8326d38a976cdadf5c55431451fc3409533f16d8bdb831f2a3987e4f78d52da137cc7dfd6076a17720b8073598d01c57df6c0" },
                { "el", "db67948fbb5c71e88731279c74d92ade5a44b01aaac544d9612e9e98a0e40dcd3bb04503c93ed5903d1e0df452de03d803f70c3a39d52b00a3f99e269ca267e2" },
                { "en-CA", "bd9778d391dd4edf3d045b0349411f784ed9561c8728fe7a7602b30cd0e98d2f4b60c430fc3cd9959c60bc6ea32fbb2028ce3b406874d9b3cf59c64d0b763bef" },
                { "en-GB", "d703b5da41d59dc31eef853f8f0e1e95b67a557b7393b09e48d15632cd5150cc9dfabb2a9086b7d84a49450068e2c37ae2dee17866374824ed775f0cb8426455" },
                { "en-US", "6264f8c1084dcb81eb0bf5c6c8c2da135c6858fb2183adcf1fbf3b063606cbe9e8936a2261da510737c082aa6c3e59d00a06f5041d1e8dd533d09de9df2c8b93" },
                { "eo", "2264d52af9d4de8fb6b757017d0ead1018c87eecdbb16a24451fbdc9b1a18a02a9fc444f8459c50f26eb799f8bd2b34714d7b59a88bc8f03380c346219555d97" },
                { "es-AR", "956feb5366ae4515dc32e9bf9ab946a61cb670851a621582c74bb27c14c9daeefa5bd375c23cc67392c0509ba85f8e4a444a591a9e0fb20c6ff4bb4baac6b614" },
                { "es-CL", "25083f1a837e5e39081ca5a95fb032e32487a12d3b585b9cf3c43b9ba7d096cf2026e1fd4c2f59a29abc3871afafdbe6960c1df77ba954c7ee2f42cc0278cd2d" },
                { "es-ES", "7863deeb374a286602e9742d0ac622410dba433d8baf2e8a1c3cf185fb1a6c77cdb336b1d308a84fe42efca77c7cf0ab6ced14aab6e4ddbfba1f4206eda32212" },
                { "es-MX", "3099441991e70248b9cb7cf440f739a57b7af8e8acaa04c7dec8187857e59f56bf008a5abd63ce7eda25a8ea6582df7ee3381c5bdd1668ee6aa55e400bef7fe9" },
                { "et", "7245ee1ef94a07bb6fd93d6c59d3f92da6bd542681f6c43131a60984f2a9a25ef0cca05ba5da6c800dcdbc4327b0db46767364188cd24ddf0c5484df64307a47" },
                { "eu", "d21a9d5d17e564ec799f27454b60d34dcbd17282030ec7863659393bc77b1d3e19123f49b7a5290c8701cce5679c21e29c2ff47f22656167629ce57a326b3215" },
                { "fa", "9460944f7dddc40fc1134827b0efc40bee90aee3fd5ddc3ad02e3eb0774990e9da019edfd9d364050cdf652d321daf7cf97d9d04525b221f23f90efe4c64c153" },
                { "ff", "3886fc6b6795b38dcdcb0108203dc66bfaf17612843e59a8b4d92757ef2d5422bc7a85e7c112e56fe3dd8e4db7f8fa42b4c335398c7a70cdcc0ae2fb786d4d4a" },
                { "fi", "bc9ab030b80a845d61e83d97266a22527da2d1cc32e1cd4e6ea2ce08f6e016b14c35e11dbdcdbb63303bd670195b6115065b32bd0523c16f4958cef0c1846f02" },
                { "fr", "73ab8e102b8c99f3401ce7134fcfc6903042187a8b300b83b0a93e5fc4216519030e9523eb1c9f0eb0ab6c3781ef8520cde1573b49e47c5df04930dcc01615c5" },
                { "fur", "b218a1f3c3c09e3079fb677515573f89c5156c81f810bbb4777ff94afd7825ba40febaba7a673880d0997624897a9dbfc58a3e74855e90e434748e64024cd0d7" },
                { "fy-NL", "7b7a3fd8d7b95343100470eacf98b71620d6b95a5efb34a4ffddfcb8f3d9d7e215ff6e6c062dd18b6d96e5694d5625b561b9a9b9f5e033854f26c9ca9adbdc6b" },
                { "ga-IE", "c61aec7eaa375f610cd667ad620962d6a8592ed50beccd635c8ca0d72ae59fef615e26f39f4d6abf8c8be4362f0a4f8ae468048bf71a0698100233f5f57c093e" },
                { "gd", "c45f171c57ba8918e41af3a0c8550281e0adb34987a09e7e23e8345703b8f61614737afc2deef120e2d4d7de6aca36b1f38e72b1ac84e687a603615519f485e5" },
                { "gl", "ed6075346721532a8efb18906e3937d344b4df568b40ef6b4dcdea00e2be76faf9c11044a17d212fc2772a02476bbd8a3641f40a6563bc5b95d277569c9ef966" },
                { "gn", "f97c3bd4c100df6912d6c26b09c571d76003743c4df03879ff5d5ee32de44ba58b00e23d93d6a35ef96181f53d78750ae2a744456d5ffc76795c3325e03a00d9" },
                { "gu-IN", "5bdbe211d3c0fefe5a2bd3a1458b004b1e75a08bf21573e1363f3f99f8fab5df73642902d1e0750523c72a97a39e15f3354ab4a51aea5994f6732b9748c10fc2" },
                { "he", "a91a2c265d414f64f017b1a74a02c61905fb665068813871f9d92dec9f0e796040d2a25b3c14d3ca486d31921f11a3e359d2c0f6ab28a6e5d340100961249f42" },
                { "hi-IN", "aeff1f9b919e71aad96c8867fc9acf06e73cf809481865622d9406733f35d3d4c99209a9cf6981291f98adf1a232b59af1a6de60f7beafbb0d8b0ff61d0c496d" },
                { "hr", "51461d42ffead4e18dd2552301751cce7562d3dda05f7e4a4ec39b2636e9af8776d29a01cbce7f6261941ff2cf7cd97d54a02eb899b404e15e0061d2a425efbd" },
                { "hsb", "abb9732833fce4725bff05808c87b72a8f85ec20722c4a0f00b1d0655ae0c8cfbd4466a31a7c5750300d85aa38a4a20a41984d32cf608125dd588bf2e1f1714d" },
                { "hu", "658c9d01b2c961101a83833cf897e88ab0ef27f1924865ec6961401cbadb9d02cde1e42e08d0c5b1355bc348be0933e767e8463769d83f24219209c9e287e398" },
                { "hy-AM", "73ee866d6fafb6b4a732d4fb8d5b5656985f1837dea0bcbeb8a3386f03922babf3a871ca833487c4083c82243db52d8d1a664fc40adcdd8b6b23a2a774ed5563" },
                { "ia", "b39a6b32dedc123b1c2c5bac422426ec7a5aff9a0b2e4fb40361876ff2df2f8bdf5d8ded548538804eb2aea9fbe8f897250fa6551cf950005c7c5be75ec8999b" },
                { "id", "b24c03ffa8c80dae0ebe93a6beac9f648bab806c251ecd1e036de23fe958b952f023dc59cbab67a6c3fbab22c59e118b6017f8e78af4cb135c21f044e73a6577" },
                { "is", "96f58769f6f769458e39ad0141e99e05d9b512c22f85607bba50bcd136329f568d19d429fa1b7a8f0ba1009e743ca8cd0332fe351a1988f22328afd52d35eea1" },
                { "it", "4d1c7f56e82f4c1d3bb205e3114c7acd4bfdfa3447ce71b4bd329df56188104baf5b148b7ba9ab8a118a3e92ae8c6837968338a7e39216d4d5a9b4c5e4d79546" },
                { "ja", "00b49dac4af1fb87b0b65eea39f8599c66c3231093c317bd50b2e2072b7fa276966482b142e532886d17bf029be0afd4a91f1dd244f68f9fd3d0c6b49b1fc312" },
                { "ka", "0cf6d8efb5c11026d70451bf5302b109bf7241589f3efb6a4027003841adce97dd39fbdeb4b7316cf8d6f4cfa59ad7354500e3a17e3467335306020ac21b54b2" },
                { "kab", "ca2aa79aac9ec9aa2680e3269d9abe17acd9883f938e4d16b85498540188f326452252b0dcaa6703f068f823c174366258fe6369e5b8fd265d354ef17db0aa22" },
                { "kk", "e339cbde9ac3581cfb39026d2851d29aa467a852fcb650f43a4f8b4a5b525bd05a2eb15df17a1b94efbd22ad2ef8dbdf174969397973a023e47820f3d32c0310" },
                { "km", "6154a4bc20f2cab763a9b1e573cf60c3def7a67b72575d1e4c2adfdf56aa832d49be72519eed5f1972d7610bc661aa39b34627c0611c37e09cb6d879507b78a3" },
                { "kn", "fee4452e08bdeaaf18294e7806581a2b87860a8ba82a94c6b53d59e52a9315dbb53f5570cbf149b135b7b2c2b68d2c1620eba6b1885ec5bf8ef848366f6f4deb" },
                { "ko", "59ee854844cabfac1d6519acb05e663446f58a368110a0b0fd64a22638683cb67f0a8161162954773fa9983a4762f02a14127fac2f1e29553a262d595fd5f785" },
                { "lij", "3eb494548790dc56d151b1f2dd499fdbe92ba03117d1c0865838c24014360077f31fe961744f35935cae078c5e83f84b2fb2b35dc3e858fb85596e6c4f51c0bd" },
                { "lt", "9d6fd82d229bd45ef88351024aaa84553a47204b1e249fdc3313902bb0ebe50767196257008859a0bc483b6d4775b36a6858ce08c0ea6dbaaa187a727d2c8ff3" },
                { "lv", "fede5c59bcb739784b4cac3b5259b3fde7da7355fc687c0a898fe16e17db4b2bfebc889b68b4426b13f110c795c458234994c15eaded2a54cc2102d968a41fc3" },
                { "mk", "2291a6e639f2532647022593c290d85830b559eb49fe8af63c991e90fd7ffeb816ad10caa094832fd7931c0f17e955ba8b41df73561d2cdd81c316da6acc84eb" },
                { "mr", "28b9c04ed57f3d91c69b2d748d18bc2e0cec41b51442dfc5aeb06fec937c408f74de00780e99f70bea553b8c359eea4458e8aeb7e2e856c2313dc9b2cc027de2" },
                { "ms", "3a6a39bfaf8152935cf13d754601de864bb9c0f4ec42020ee7a0692e636a7a97363bade8036e4fd5e814ab1138a1ff64d9a874664615dc5d5af7ee9e2f5ec42e" },
                { "my", "9f36181ab51647a5f6ee4be5b0444c5866c576e430b345e7a54dfdc0c8d8dfb5a0ece4d569416ad9bff773a997f787827d0dde5153cf429f88cadf9bcd8cc6cf" },
                { "nb-NO", "ba3a6d4985f89931c3411413bae817370732b24f48d7aa738fb19950ad740cf344fe673b9ad05cc822441db47f148f9f4d8a2b14f93193e3126064f2ec710477" },
                { "ne-NP", "88c9b26fc62db1a392612612016c3d8a769514e40b176032a2903f3ab827b3c84f7837df3725db6852b74fe954c96302646350160c7b4da5eb638a75eb26816a" },
                { "nl", "58263ab0fd7c1c1ffa79be7abc0577a06af3037dd31966e93e78a2b1b245df9c78d98c899877ad59e63baa057e636f6c2c9696683855e677de8570d520de3817" },
                { "nn-NO", "9e9d62a62cec1a7e92324693b6bfebb6497786ddf5504cf679c526365f511bb81479eb85254aaff00e1d022071271015a5750a4a9a9b6aeffb9a79bd51b1c493" },
                { "oc", "c5ee4ce36f79a659bdede78375fd33fef92704cf6880d5371a304eeb101b5da0779a9c47258d561ca37a4050ebd4a5e2da781bbef0d30b884f24e7e3f221fd29" },
                { "pa-IN", "79513dfb117796b47e2f1ef5b5e58ad6cb5d4e5a0c3c0b07b997a012498fed29841a0c263d31c10e8da543716a548455798690332c5965333c538ce86ae589b8" },
                { "pl", "f3fe37ef71a13ca34d878fee79d54741bb83d4583351da0d20910c772c3ce2f5267047af968228fdc0b6598806b1da27d9a369c46454674ff604d410408e3719" },
                { "pt-BR", "f68723724b1e63f7f258bbd9178f060a45d13abc0b915583d5861a7ba840b2cae7db64532c6d5d8fd3f7b300276116a0d385fd9a3080529b7f7e990432480874" },
                { "pt-PT", "79c2292d405904f5647fb2acb21857e65d521e5b4323a4d8545f02abdd4abd436ca4c3c2b80f5b36300e8c7054fd756e3325fcfb1fc016bf50076965dd251d22" },
                { "rm", "81b29e7502f8b0b081c417c49345d748b28897e35c0a85f35dad3274b7817f1bfbd2342bf8b30fc46dba43eabdb584602703417bd63f380e19d6ac3cdf5df5a1" },
                { "ro", "505c43c1897707cee6fd8f14750310f5fd323ab65bb23282fbe27a2da614264412dc18462a0bbbb0fe5c72874ad4e81e89099310a5c6793b68fe44d9fdddcc2a" },
                { "ru", "71986ba77bfd51400245d04613ba5ba600d3f8784cb601b72624d00ebd2451a61f2f45c03d86bf58f2c300afeac326fa506878e57efc61210d1412d7b0710c8e" },
                { "sc", "384f7a33d97adc5772789f94596c7e1617f5e5b15d91b9ee0108b66a7b5c30a0e5fd266f551bcfdb43b456b9f34c6352e3b9f38ade702e3d97512e34272f9a6f" },
                { "sco", "ddd4b5cef38fecb7514cd4a7ab6b0dc6759823e4f6223dcdbc11cf3a40b71f90440ed6051a848af80f565616cb6e58888177fb421cdc4749bccb9c268fcef8fc" },
                { "si", "92d1109fb7c8698b9d92a4b1d9b902505ef4ed0ef708bd1e62c57faf621055f1856fa9e4986f1fb6d05c765f9681fbcd5a0542bbdbf2f41cebc51d61da33efc1" },
                { "sk", "d6877b3a725177be4131b19eee748787201254ddf0f6187bbd951b4dbf4996ef011e23e775a3188d6afb8c335ea4c9cb433197a0edc8f51781d536a045ab9a71" },
                { "sl", "5170298ebcd0952a299ba4bedc290e3895bdcb1138c4e15cfe0989ba72e270000efb0fe06d9112df293fb741ef67a165091d6ca8ccc40366db21e288b4d6abcc" },
                { "son", "d7de6fb865fc952c19fe8ef3b02d5959014a85d250ea68a76627a00b03a6309500dec67559abf3c40ac35033d2cae30af01f3aa3c9197d1a19adc02054bc103a" },
                { "sq", "6b4f5ec9789b1e4a9bb0fb2360866848701f5a779492358ef2f8fb9d516e902a6aff9e47abcda3393a365f56b845cc3f7e4a1305cdaa8929d2f953951609f982" },
                { "sr", "4e8da220a02faf63bab0e52ff2e2d994c4e29196406fbf769bc112dd50882edf1648aaa89dd0b5ced536dc0c07b6a7faf68efd2d9c7f2451dbe0bdb819f8b045" },
                { "sv-SE", "7869430c37e40317ea574ae4157df0372ed1105405373a63195d0a7c9e1bed290182c8b59511ccec02efb6d6c29ab0d7391be8e9b7467cb238a2fb971aefde1d" },
                { "szl", "66087017aaec6354fc3eb2f654a42763bdd5674cbdeedb6a0dea69afcce2173dfe768c5ce16232e601309f4073b3c0dd5dbbe511071df92989f7a9feff6f6605" },
                { "ta", "09fa70a806d8617aec6130a139c2595fd0b44932d2f0403015d0d78d0943b240e49ec8c31447de3fc6a22931ecf3f19b79af2470568a58c8a2b2363d8af17d4e" },
                { "te", "397e3c0784f3fb489fd7ea83bf6b68d8d6123f54a059afe332b8a38a39097f0e4c75528891d0e759c7a978a2035aa73ad083a6f5edd8a0a9ff26e5fe457a66b1" },
                { "th", "638ae7a44995b33b24984488823ecbfdc08f935078e7592317ada2ba3e977858b876297e68a87197c91080c67cda1082ed83bd8fe52cb56ce08ab4fc139995a5" },
                { "tl", "ef4d7c0c04c85a786a8b83cbf852b801e7fa1e6f69bdb2c8cb7f2dd10e4c5d4976c57c3a1e7178a82301dfecb864a2ad80f0037ede601c3ba0a956f3c66c6fea" },
                { "tr", "70dac79828578c5a58339914a6065f29be6aa492c8c06e2f2569687065577d3bff4ad98ea13cb497783f8a4b85b7e9fd832a56cde5a79e6ec05de91ec538ff63" },
                { "trs", "56c72f7f430c4c40a31b8f329bc10570d57e4f35f87fdf550f0bd1fa84d55cb98ab390f83ce1ee79abc91b534c4cc7d5d947f78f0a76c2200d9f967121a2d633" },
                { "uk", "2e34cd111f7100080b0236895fbf03e5b1a343eb43389f0a987a43d41010f299ae729f84d93f7f344a8a3e060dcc5ba13520446da72b40586131cfe60b7b2c35" },
                { "ur", "738ae021359143154ecc14b8669ef297fcc7cdd0a86f871dde76de856dd659e41d066f0406eb0be5b5490051faa99c4b236b2b8f4afe7b7d910853906964ee32" },
                { "uz", "dc610c24c610ff472196097f401b7d9f86eaa2340103a13e4415a0eff3a5e8c9b3622798a811d6c1940f916d2b26732d542e55cfaf0462cf9c74148c0a8e4e15" },
                { "vi", "9b26d07f67a1dbf38a0edebc9d4f89a5ce40f02088307a3e5606793aa6efcce3a1987bc20495d27769d44503bdc00db36e46dd04338778b9e93ac8e7987ecc27" },
                { "xh", "92751ee0c25a9a495471f4afb9483f741f8deea8877a3cea7ee6c5d3961d3e6b77de30455223e0ec7aad9ef1dfcb6c974a5b70184c056c965a681df006f5ecdd" },
                { "zh-CN", "13d32a6ba5046dbd5c89676213c255d1ea74eae90f7f0317b0d1aa1efbd5353f04ab809ae29d289b129edc6dbfa9442d7b324f7d4b6fe9b33a8c4d96e358856f" },
                { "zh-TW", "0e58f29b37392b708effc34155fb196aac768eafccf7b5b1eb4ac7160f54bf8809bf6c420eb53e866f8cd9d8bce4e092f3dd6d4170c98c122b4d2d5c68f162be" }
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
