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
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=San Francisco, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2027, 6, 18, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "129.0b1";

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
            // These are the checksums for Windows 32-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/129.0b1/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "fbbad9e1bfeafb307444314eff71e5e4d08515d26bfd001512711ecc215549610f41a4084922122b3afa580beecad47e0073de6658628d1f2eb441b362be98aa" },
                { "af", "49e359e6c978ab1f254c04ddbfc4c9906d47723fd5385d138ffa7fd74629c39be8086fc285a9f6a9de490382dd713c2210ff0a9dc13ed431f81281c06c6ef27c" },
                { "an", "95294932189c28b12d010c953de7df2b5c9abd32940e4222a4bca2d9caa89bb825463ff2a1b641c6607d10438c69f1e97de5b1fc87d3e485f90d739eecad8829" },
                { "ar", "5660616b6355300a42a377000d916a17558e77aaa7b2ae395a013483acc8f39ae5859bf5e25ac6d65ed002abb503eed8e0c4a4d8af9126f88396914833d209ac" },
                { "ast", "559df01f28c87c74641cb7c7d0fcaa7000b9a770be4e19e21b7154e123f6b0a7e8ebd73f6236285010f7dc3b4b82929e655e087f0683fadb1c1922e46a63fb97" },
                { "az", "917c63be30398fe7ff7c8100ea5e3e388bae7e92cf204285f069fde230b7b6ffd7d44059a5d329ac8bec5c32b998acdb954b0105ae83887b637ffcbbd60dd0a9" },
                { "be", "b667e85374f0def66f6b9e12dc2dde3f22291e456ac4df39f743a43fa3da9c6a54b60b8886a918fb2d970eeb1d2bb23d283d9c57fb9a3e1bb3181b42c9aa2fa5" },
                { "bg", "1fa649b4589be723a04f6b3b32d86286325c364c4fb4dd7da20fb4598c1dc29c602cc90711f0fa3163353fac08966aba66b6cc18647b9cb8be4faa6ef0f8a334" },
                { "bn", "b694fa915f087cfb167ecbee17f4c32d93eca6e991f8c8b372f841c9972db15fb804d79f64c310381bba39920768e20ba6167ed04bf1cf493f2ae8e2fbdc325a" },
                { "br", "185d03fa656f7cdbfdaab42305eb444384727126da5125e65237e89317258658d365021bdcb10a68e12786ea57ebec543052d2db9799ff345e12304ca4d669d9" },
                { "bs", "4167a0b32756dbc163a8dc6fd52cbcd3c847b915970518c3e8d69a23a6474b19173a8a14cf1972e2ee1945c3d179c7871d01a57774fba166ddd697de63791e5b" },
                { "ca", "9804d23836b182bdf4d4ea035d6264024cd7db08530af04259163df0946e1bf63c459905aab564d8174be03766bbc00f0f70b1ea49eff748b5b6356699c7f39c" },
                { "cak", "4071c6ca5d63e22178b78f104ce366c511d72b613c62f795595feefc6d9d7a8151c5596e634d192768cb34d93c99ab2ec0bcd040380abffb8e0444e859c367a6" },
                { "cs", "63f20b7f2e5c51ad8f0808af73a4bc7d8fa29a13857e77d0431a58d15285bc4e4767e5c12ba0648f55f8a4eadec0454e455ff0e1fa4cca84a2544227d926962b" },
                { "cy", "af4bb22f9716fe89ff97b4e7f1f786003f85cc6f29cc7f8de648dfcab39f9064d4e93992c468ff484936b266ece16d4c55968b06acfb55ada8d67a6fc5af7511" },
                { "da", "4276d7b36369a198ec4afed80db808c50be17c972c462dc5fe8fd2ab9a625086ed5b0d04c0756d14ff02f09a96cd6df3b8bfaf04451ad8c8736792043a0811de" },
                { "de", "ba5e6938a54f22bcfa5b587e6f25ee10d1742cd56400d794c76109c71bd9fecddfaa4db5d45b2154115f6c7d03479406258c44e7e3fbe8be21ffbcfa275a3226" },
                { "dsb", "5c3140e9118e4e12571095eb127ce09115ea835b454362f6df489bb0300aca2b57fb009bf433f99871b5d6ed730f6a48d8984dd56f17ede718bc8665811b3ed4" },
                { "el", "0f9211073fe57e509430d8aa468f8114d617ae8e84bca0c5b8f516a12b0d5ffb8c60ec1587b7e0e44144733dc37bb2b9b63b9f4945458321656493c859ab66c4" },
                { "en-CA", "24c8a5040a2fc86abc7955b69da17966fcf5e005b0a3e88b575865321697854c6fe9515926d404b90c9afe39e9e3266a58817abe9d77c7639d4043d82c1e5b23" },
                { "en-GB", "8855ef326a0dd78dd4eafd1bd310c8af3cde14ffd8c19d4c9e7b0dec9da942edbbc3a66e151ae0bff75d50b629998a4ed9959d53fa0229f3d176d686d5401b9c" },
                { "en-US", "fbb43ba13db6482f66c68a1b6b424a201bb7ab9bc38600db7f44a23170f08cde543b4ef45d9fe16e58b3648a141c18cf9e931b7c396d63322dbb27ca8423d0f8" },
                { "eo", "e381cd5a2a44391f83a6ac38762ac78420a6d78a0da077e2dc63f3ce9da60714bd00934d03341fe4c605b514e6c60da1966b332e9ba430d15540522981702dc3" },
                { "es-AR", "9743d9ca6e24a25405df81ec70149b8f22d43f8e1b996630a4a56805a530bf67af4eb50fa55531e4c5f62ce45625180b10f5d97ef4568d01123b189760dd56c4" },
                { "es-CL", "0f7d4ecca276f111c6d94b33c9b8ba7b6cd3aef246735b86c8ed882797ef63e6e99daa5c12483e02029d489de66e676c29902da62f3096c2932022f0241480d7" },
                { "es-ES", "3219a9a3dcb0ec8b9757675999d506d6980a8928177f01d8c673946f3cfa24ec2e2361d486b9cd1ff30807df4d326e3e84cdd5015b92a2138d39b2ad17eef08b" },
                { "es-MX", "5b5ace59aeb6c6b8b8be91b1fdef27296826340faaf70baa46fde75aec64e2344770fb380c78c00993d4115934a7a26498a1e276c60f3a643cce22616cb67c60" },
                { "et", "498cb757175a23ff09d0c70f51446327da04b0d5ca4dedc1e696c69e5a6f7f6377eebc9a51247d0c945628ed19fdcdd5f92eb4e27cfe3b137072923bdf5a7019" },
                { "eu", "5d22524b3a33e879320451de4c02b38f3fe84180f28b113ad0df378e362d0093cfab29ee94067da567292006ba12a85b5e04418963fe220665d287bfcec4d6fa" },
                { "fa", "1460f753bd910480f6dd2c62fe4007858457cae09c6ab66d6a9bd55a87d723de4e1f0a73455b4143cc196dcc9eca7c0335537b9deb6627fb3eeab0117752bcd0" },
                { "ff", "640c9646888d998f06e18c695aa85999bcd1c86682a5d627fc863d310d9a58a390ecf7c19b5720eb6c071babea60a70061cc96ad455093c1648d733ae4500887" },
                { "fi", "fa0edd2ed6b53720f46abb0350df5efb1876558873e21ed77ff36ad97ab4ed64d672988e02c2ab5352e9694a11907302f8f7e4300f759616a139ef647fd471f2" },
                { "fr", "49ca3ba4124b6aa136212158ad51c3aa0a38df80033e639cc8be3ddf995e77b498a7661a9b84ee7b48bfe64f6edc85de6fc899c7b70496a39bbf3e1b782a09fb" },
                { "fur", "634aa7a56eb4e26904f1e916f76ad34c90e9b9b249b2ce7f24f114e033dbe62ac437c7712fa013de1e465f6a7c024433c916dc766d81881876be722fba722397" },
                { "fy-NL", "42f322b0674e841a854acd331ad4cef234c9f071a054dfb8d640e2d87e328e2199317c8e7e5200d7550335d43cb85dff81f0ddf92b56dbd4a3235d3ad82b1b36" },
                { "ga-IE", "b0ee363f5e705fea51ac3816a33eeaa0ef2a43e79223a903c5703dec05e2cd4b85140a09a448014d9f62ef0a91036cc337f0e736d5e2f874e3c47353ecd64e95" },
                { "gd", "4da0899484bef6e622298b267c8c483016cab85a83bc0142724c579cc7e1d0a51a084153cd9d389e5b6dbd21b7cd594f46c03c54aae03253b1fa007acbe27448" },
                { "gl", "f10e6c48dea355fbcb5b385211534dc39e3fbbaaf2d015b75762ce541cb1658d78a8a190dc761b5aa5df243c84da60145072242e760f7e66945ff75906aef681" },
                { "gn", "46a6dc6b07e429c919c5d86051cc75b99358f81a0530e0ba9fc3b3aa93e73e4c258ad07397f8d3de3b1a28c7a54845e823a65c37a2716f44ebe1915f5117d86e" },
                { "gu-IN", "daff74463a96e53f56b3175815505b8db699b40fd8094be4c7f17fe889f0d72633531b87a32fe1facbc737101cd7a1e546b169388e43a877c944549a0f1c2635" },
                { "he", "53fb1257411309bd47c45631cbf9f994cda68ca0fa16fe805ef8ccfa8de030192f1578e40f56efca2a16e33d1a4d54e3244d10272cc62cd9b3cf65fb6fe37682" },
                { "hi-IN", "93f3bb60b70948dba4bfde7f281f0cbae21f9ea7e4173b2fbf3622c02eb800a4a6565aadd5dd355d8f68f5f80b1485701bbac34c062456c9de968f1ac3b08629" },
                { "hr", "cb6c53031e9ecd4fc3f0c57c344a893083a8ef72f605221cbef4f4c1a645b015a1da6dd256fc87b0b36197af36f140981aca9f7f70690fd4f1fb96e89b11ac62" },
                { "hsb", "ea922cb9110797e6a4ec1ca5612c66b003f720758b7838e79e57eabb02398566794376f1a8586f27d499c21b979b4351bdb32d53086061b89ef25163c1792a51" },
                { "hu", "ff3c4b1df8c94ecc34f0367d06e9b92154c932ac8aee2e4e11ca76cc8e412b896d4619441485ffdfc459b3be491a8474cad5dd53d7e3fc5702498138c18d56d7" },
                { "hy-AM", "1dcae0d859d3fb3d21f3911c81398a39cca9363d00c52651af51386bf4904b0abd9681ab3cb74195558b3fc33a01f55fd3d6fb9b3a3bf77d3e26faa3aa466439" },
                { "ia", "5e685aca35855a5d778394af44bffeb2dc5efa570aa04db9d70c70b506bed012c704c954abd4196673d22d21d0eac5f9de6a0bafb92241caea7b30f1db8213dd" },
                { "id", "772ebe4955dee27839ec9e4bd02faf23dbab3ba4ec8dabaaaebe1b7e49559c2840d471b41af0deca27e01f98c31b12f99b26033c21daac35277a168d8d108db1" },
                { "is", "0873f5ea960cc427078e29c8a607beca37b64377683ddb8f4648b30b5d846ab8d019a8d2f73ebe476d4737cfdf83ac54d517eba63defc8f42ea6ed7afb057a14" },
                { "it", "a618a6ebd757d39d23362345232e7b60d609d647642ba689d3ac8a4d304d2f2546c6e6cd61026f244d61bbc34a4fcfe5c0a9aa21145334dbfa6496992efada35" },
                { "ja", "e0b88cfe6dda3c4fac11bfde6b7da2ea600149172e647ef7062e845f483af28135e88aa6522ee8005752b4e52bd8530d243d7eca442462b10f69d4ce4664b015" },
                { "ka", "be0c400cbf911dfc5abb1b1def0a7e1dea5d583bbe4b8484eb4672ffa2846d149985476496818eb280dbd4379fb7f55831157914324cd369dea7f4e6a173abfe" },
                { "kab", "dbab0ce76898d25f4ba07a2ba6991aae267b7c69e50fbfab563dac07109c9c879a179ec825978a64003e50e97091346489e4b4e925ec18b399771d39b21c2745" },
                { "kk", "e7aa7abb3c0440c9ebb10183ab300b41b9a0e1a7a3c3b32469397166d7ec45f3d18ac1d28c52eeedb11ad53eed8f293848aa96bbf18136b3e04dac763c5bb0e5" },
                { "km", "dfa7a89d432f9339e8eb12e1e61b3ffb45f4c7edbc454ba797a6497039868cc8334ae4f65c76a9f22aa3f32719222ebcb1d55119e611ec403331ee39c2fa9214" },
                { "kn", "3807cdaca77673c545a0e826eefe9f3ba02b1ad4b418f494208a0f64f643ce7a2663da92c2cc088cd84a863f30223bd30a86d86cd2b470d4015b08f0eacd7a4d" },
                { "ko", "5d95259f12c82b24e8cfec3eb348e0a778c9e7f804d3bf2e1d8da4ef8584434b423265cf436edfcb2259c12971566b78e4c1f6e51766d632044d969877c450c2" },
                { "lij", "356a224e48b458787633f49459e82ad8512c73e9a7a2f6cfd17e442cf1f74398685d4aa541a38ec47e05430440541b40b97dd5350e01c4d93d3025528fc7ba8c" },
                { "lt", "d1c3834556d694614aca69a2f530cd5d059a8932172149d177d602c1f963a9f68a31f11d174433ec84fc29b404ac5fb2263adac355a5881c6920d0fd18abf234" },
                { "lv", "091f4d8b02da92323787aa535f97cbf03d76f4202c061b7b3c7ee7e3869936a3dcac08daad89491b13157fb34d2ca7f3b3cba7f9a2725c0cadc3e5506c0d3631" },
                { "mk", "4cc2129e89df24efb697ae2ee4271563be72ee22e076ce2507f02090dc118f88d57a9ee852adfec90d8d1bfdd84932b8ac61456d6f5b9b97c0a8bf2f84dd7e49" },
                { "mr", "328109fa4f89ef8388a453f40d196ecb56bc07a7305cfff1cc6253635aba598f180f31811156144f08723a37743e3925860da686117e8316d6b83d41f457158e" },
                { "ms", "568108ba418ad2503b0f9fabb993eeac0890a930b66bf1dc2ca8794d3d8ee4e42db55c9494ade02fba11cebe8e7bcafbcca66af4e56b5bc3abcc878a662f1a9a" },
                { "my", "0ab7205676f635d374da039bb5131d0629d3722dd03f1c628df46e725ad9258af6d3a7e6084388c5b8263ff6e834c94f067e955f91bac48a10bdb6ef72d5e6b7" },
                { "nb-NO", "0abdb2b642f2af205b7cf9d79f0c21534a851c019b6865637bff2274fbf06f9b859200682599fef032cec5ad9447faea826a24a548f7efc40ec842fd54fb2c8b" },
                { "ne-NP", "46d81614d86135c930eb495681e1a259cf5b7ef7da6b1b82da744c82d0e49582aa2035b60b3910f993ce30c85a7799374b8683667f0fd1bedf678c98ddf0210f" },
                { "nl", "a310a7eb660e9875fdf0a87ad7ded676c2006dc1668947d3e77748449e7af42795478fabfaeb546e7539a6798e1b1a6a7f1e39fc254e93b385bcfdd5fa2ac1ab" },
                { "nn-NO", "2f66cd77b011efc468c562a2cb3a57b0fcf8d8269f9558644fd20acd1f00fa0d8967e27b5d83ed23504a67eefbd038840b7360403afe2a4f96440c001a7d2a7d" },
                { "oc", "36be5db426d75837d3977ba8516a87201fbff2c4a14bcc87dea8c07b3381ac59e4ec8e96be001cf4b78ba85d510a13fc13ad61c83434e646a644010bee1966ca" },
                { "pa-IN", "937251517b35bd6b5d50f4b67e83a6f84e780feedfcdf4e7b28b9bde3e76145e912853afc544cdbc8aeaa4c07d795ed758da7c37f442f96a14227f3a31802b81" },
                { "pl", "02b32d6286b095cf1e47b2e4f5bf7900cd9ee645f36a6aff22e94b632d1c7bf0f34ca67d473b69acfa13acfaf9f4a22d55fade7586914abf1efa0d4413e6734f" },
                { "pt-BR", "7a6aaaf12b94d9b69b667b460b3c5a75dc7f9c03d88c87da86f5beac8ec255381176ba8824ca5a48901b313a7c9ca29a3e4ccc404ad3723267907d15c34635f4" },
                { "pt-PT", "6174c90d81e87663e3b072375cd7cacdc1478f27c9f1d2fc7c8f1fd5af721cd940457d29f7fe6631a886f885dd7ee2f204cc5988bf24043af7635d883c1bc138" },
                { "rm", "ca51c6b69d76965f97bf2c526d5e6e3200d79f4cf501537fba568101ab8addfd77c9e5adb12996a9f940955d59bb4c9c685a26997cab1c3d31a2f24a7f8c6839" },
                { "ro", "ab851d83d86bb624c50b3c000c8facd12923fac414cdec31823d0c45a71957a21c132c7e8d19ad6894a30247206c0ca87e81cbee1429313909593067d3dcd367" },
                { "ru", "f5f59f39b86d2781a0e968837f58953bb01f5ebdd829e3da16b9cb931bb776a858c694b13c7698d8b3619d6e73f85f9ca1b0414fa250ae9b9753ebfde1416b04" },
                { "sat", "da0266186a5debfe328fcd695f1e83155e9d8131900c89d9c05635848ec118c0700709d1de009e5cbdb87ba7147250bfb44c1c67ffc33c163ad0938e74803317" },
                { "sc", "5dcb1c17e553331e31f211e157c70d3086fc878860205edf527169b6112ed67a13c42bfa8cac7a285cdefbd7ba108ae35aea57befcaadab176732fe31e507667" },
                { "sco", "605ea8e0c5b1a4f2e7a92418336ece6a2cce3ab5acdeeaab9db8e4d6e5a87453e61965d698b44617a23d8c79b4d624f4b6473ec4bcfc5715d5c8c27555825dde" },
                { "si", "2ab8b55bad0115c5291f5566b056237cc552c34e20886101a9c6cc1415180034c938ee4213abbd7cb34308c4c8d78beaa20ea63aa0fc194ac29685f7d4e4577b" },
                { "sk", "41343e0da0505f1031fde367170b4c98312c6b5f92972ad3e5721ee13a232cfa6dc6eeeaf9566986d4c8874226984c2bd5fe07dffc640fc5b4e5b5e6d7476f53" },
                { "skr", "24209116c60af8da7a68c05213eafa0be0a1f19ba78ab340ebe3b0db4c8f6eb3077b072109980c66f80748f7101fcf96129bdd91c7696be10b70818e135751e1" },
                { "sl", "96fc97589dc20127de456e0a008488f278df8dfabd1a83b6a7dc39d7c79c4e2ab5cf7407b2fe13756e4396498c029e2be098bd6618dd01d20b882c7f904ffb4f" },
                { "son", "b055846d8b286659e3c1ace7aa86edf2a7147fe888e435231a9dda00e6c9fd4fe556225a74dd37a5e06ff3b211cea50d50bdbac03872ddcb671e9a4e4243ba4a" },
                { "sq", "51ee4239589ee14c1e0a935f1bddccfd05e2e81ccb60eb70b6ba49be8f9a754731525ab98f3a0687ab7655292175cea2adce8d8031e8191e3d4e17d5b3ac0644" },
                { "sr", "d73090902617a607e2c8038b878cf400118e95da60adc5ca88911be291628223a0f7f6938ec55e3e991c9e64b06ffcd9644e7b3d2f28f94ea4a87ede0eddf098" },
                { "sv-SE", "22055d024e66943db79fd011ca887c367ccacfc66154b269cbc4f55793c5ac2df817f2bd502d30670e480b14b85a411beba3154abc9eda85fa12b9f03dcf68f7" },
                { "szl", "a3816dcc457d4970701c5de561deae558bed13d9a76bc006245a99995b9544decf62df1f88ab4753af9697ae99d48905b9f7ab6e361cd7347cd1732ca5c08f1d" },
                { "ta", "aa9f6733f5f63e7fdaa536adb7706eda0ed8536c77ef948d5885a79476632f21e18604e9fb2f53910172ca3aac24d5ee3e631276e29ce492fb300cc463ad199b" },
                { "te", "910863c8595f56175d05924aa5be43897c3b6bc6ce835185be6128c4c3e40b653993aca7e1e4c6f360b04f7d999b1e0003379ca69d28d73f52bd967ab2f8fdf9" },
                { "tg", "f384a6144f31d04d906b2a282bfac6180870cf1c33330c0ddd76598acb95895a63706751868858a0e6de8e686278a2dcab4f144d98ffa4d9dde48c7a03c83107" },
                { "th", "b896b08e2ea08453a2ecf473f7f3e870d03f3690da58ae2c19a185414b7339314b705afc9e462b6e4e4fade5a7d6b26118b6d3ecce322f05aca26478474067b7" },
                { "tl", "c573692316fdc0627891f8ffc9a7d3af24b9a17cd25b0a002962d6852764f60e43a19a5ffdb7d2b342619c0e8e953422e8d8bbd2011347add914f6074d20a042" },
                { "tr", "3c867a753e70089b305156a1d92426dcd7096565e3375c286f8f0c0046e3f32e14ea05d68c9091fb520c2ec162a30d9d44c18822d7909ba990cbfc5630ef869b" },
                { "trs", "dddfbff9f68021a41c13813e88029c4e6f1d6e1b7cbc02f5ed441ee3377d16f08c338810d03283a4e1ff18288f6a427ccf7d11b349c8ec3e8e7f2e412355c26a" },
                { "uk", "17a8d1cdd36ed5b453f70d96eaeef92ca27e787b310d868e0b9696c19f5699699df7af692f0f45b497f0273f10b0cb5804a103c0523fa4aa31f2142af341be28" },
                { "ur", "015d7fd39ef5b332065b45c360f37b54817f0602e87528f7581b523bafac9f72415fc8445b201712e08a420fd054326d54c62dd8d48ef11a2a81219708d2b1f7" },
                { "uz", "2e9ff6e6d624acb22ba335d28f81f4d34f42fd5366f0da8372ba74414ce7393296940f076f41a5ba1b3a245d8107f880c1b4dbb47f754d8fc1168f4f714a7ab9" },
                { "vi", "83776fac07cb896deaf3e0ec2ad395015cb79e47f21f1c2ab9dc0f75da89c927b8bf63f9925c53e2799c19c70bcd4bc571b6d54277ff29434c280fac1e62342e" },
                { "xh", "b86f934d138547f414f079d378c49308af170fecc5d0e5ba3c47a34afb21e6f2ef7ed69636534f9007a617b5ae786bbc7b29e01dfb08d6d92b4bad354ca00e34" },
                { "zh-CN", "dfb45df5a8b42609dcf5d099b9d7c9d9bdac3996a7fc154e7a27a64d88026e89620eceedd2c3a49f91719781729afd018fed9ecf14dbb9c433b54fbcd83af837" },
                { "zh-TW", "23b1cce8b492f2e9143c39c65315f61e71e91e9679f5da68242234acf386ffd36d3620a83514d1ccf568406cca563ef5795b78cf788202ffb252d15f4ae193fa" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/129.0b1/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "ff7f78dea59e592cf7bb453d06176304f0b2c509a8bab51445fcd26780cb26d0cd79f5505c8c288e7f24856c7ef613d79eb9b139577fadfcc049e548e89b72d4" },
                { "af", "7ed11fa8ee55bd9f807c2836fbf66ae3bf7a0cd5f2abe5d8a9594f8edbaa502681a8a0cfedeec0f8c45dbaf4b337171f789f42c01c8f4a6dfaf99ef398e87e3e" },
                { "an", "4f5d39d9df8ae89b6623756aa12d7e3d7761bb9d86987d214ec5930df674a5fa3f53e983c22469ba7a99d2b99a087bada4d1b39202df1d14ec7d80353c772f8f" },
                { "ar", "77be7e8e3bd8af9809ed16dba1b3b7c6797bcd87c41e5320a7295abd40d0276cb6588aa4f8f0f7ff4199fb07ae5dfae60e9d1046d07aa1e5416347cc4c047987" },
                { "ast", "f5a5420a1808b0bb7f18751f9aad0d069e0cc9a695bd02358c296144fb95f70ee51cc239a37c11e8153d0865b7ca5fddcc0487cb0e74d709950e4576c2edc562" },
                { "az", "3f6f7a2374c0e06cae8a078da82896161590cc5cf8e0144c09828c3878d268f232deb679c1cebc097051b70d204ec16f34fee266bb1441d11135a5746ba6f482" },
                { "be", "40f5dc3bf1b4a56b1e7e85a042e42fee9e9c9f7a68e02a7e9279f61508b0b9b89fb80c5bb72f45d80dbedd45e8a343f9761978e0c3aca2e75be5bc10813b5f68" },
                { "bg", "2b82c97b5a42afd05a54d399756930093e2c80ce728c9772de904561cae786d38dc0b48189dd170bf93d4b2aa39be5c1a8f606a80ed7aa434c32815c3e0c3f2f" },
                { "bn", "23ae371c7bfdbb69904ea98b4d5a4815dd424da57fe15a4d9ba6643e0b105b489b7eac93747ce7bbf88b4d27df3cd178e40d20e6c83218a24fbfd7380d6cc951" },
                { "br", "de7dd2649bfe56886ffa62edaa3970ab939cb3dce42d24c66be81abea04f69191718b34ccf8083f5c138f4a7eaea1e2fb048ffcfe6d7eb45bcc1caab5dd7caa2" },
                { "bs", "38f062bb29fc6f3cfccbdf0f56607f788e0fc22e66466324ff984df1f3e4acd99a17eb808892fe6c6452b6d25e23fb1a057bc6d89328395886332287878237b0" },
                { "ca", "18f26124fc7fba097309136f3ef194a5cc720649be5a8afab4552ff293d4c374aadb4ca413b5311489b5b6cc9b4b2b1fbd9ca7ea2ce1296685c115b1cf1e9e03" },
                { "cak", "3c2f08c11a9ecc0fb3b46bd20c37e911e414b928a847eb8c7c8c017ff94d786bf707f3be3cba33723b6704b40e6459f4c3bfb33cc425f4f04fcdc8894814ab2f" },
                { "cs", "a449f8493b617295f501058c4be0a3117548d0a09f0ea8708b24c33af89725da63be4d5adb32232a93fc64cef8751665ac30839118a5f72ab61666a6ab0b39b7" },
                { "cy", "961d872f78acb8e3505b2adcb9c95d085fa712b52adcb1ebacf5cfef3d49da426753ac42428d3629051df6819f7d3aaa02c93707bd0689259b7014a928ce234c" },
                { "da", "c690f0c52ade128f6c598b14f346aa86aec39e560d57c0784a3843924320d451671ac36462dba10ed95f403fda16852e38a137f86e3f09f52cd6c68001ddd859" },
                { "de", "0074d3b57ec9fe20719da31078328f46cc117ee027f401e5503fda8cd1c800ad635691e185ae466d7e1d86fffae40ab8fb7e37fb0c2f5e101dc0bc94b9be1061" },
                { "dsb", "6d9aaa1edace3097271f8f3f7614112dc790e67a4b79631b1d4c402bbe022724788ef31995ebabef6d96662ff8b623a6c2c8065ec95d7cc2feaab23fc6bc609f" },
                { "el", "9d2d00a29a795b1819aeeb3565a96af81cbf5e2bb105545645a8db33850ed619572c35a78ed48dc7a494eabf39053e8d0f9cd49eda8d97a0fea9abca3aabccc6" },
                { "en-CA", "4358c423089bf2a888810d81302f6538a4e37633924a5c05fb026248be53b96a0bc5f0c5a496562107a4bd5ca04d144567db9cb3b75454261fe798f0b083fc2f" },
                { "en-GB", "7ef0e4f900c6b8ab05c215ec838852db01760d403a73701d51b3eb240ada68536bfba64342eef69f4399b24126a9a199dcfd5797b0be5c568c4358c28b743625" },
                { "en-US", "c0461ab846c9fc7a20c90374e7f145a585a934f214e6def36178740bee151815a32217d152928a7900dcdebf82a9f48a646e4d48c886720d293dc0ff6503d21c" },
                { "eo", "53aab704267b9012f3d1230e713f702b49d4e054540779bb835156232f9040826d1841d9c35f13e5cc0a754bcc33d6716395336c2f68524eb721970337b0374e" },
                { "es-AR", "8d444852ed578c89a75ce2d7190f9157f8d393ebdae9f116d4bd63b170717292b86c298ed25eda9882327062ffb7c731a80e1e7ec0facfde31ea62310c0a2d87" },
                { "es-CL", "9c1ebdfd227f0bf6367c3b6b6c705f0b1eb0641a5c9ba7839164daaa1db6949acad70df335b7a0df1f43e5d8cd1642a2598771cd88bc0b72f51799e2821672ad" },
                { "es-ES", "4a270b5980b35708011c32a7cb8ecc9990a50590ca16d937fdd326142e3c160cef9bba1f46c55754d2bf2ca852453a791f2a95e07741643017a1ef8202ea424f" },
                { "es-MX", "3548b0b5c3c62e507f305ce483f463befbd63a30d4f9243de8bb059e607e88348edf832c0ce77535baafbe428fd06f82b4120feab0ed9628ff02cf5e9f6c50b3" },
                { "et", "7c2b3bbd04bbbc620bb691747949c002cd94a0b9ed5fbf4c70abd88dfb26b389d406a1dcb16369a7e22855cbbde38275b855c377fbbdaf029333b249713a9cb7" },
                { "eu", "add71bec6de6360368bf87bea12bbae2dace1b16d670d9746c0908aebf79a67a529460a7aabe16f8acf7f55b189612ddceb870b9ad11c10db4e03cdf63300940" },
                { "fa", "8406b604fc8358d6d2880a841aa53f4fe0cdb32cbbc0cfd0df1ac463382e1eeda7bf81457bad9d1efa2529a8157fd6574a7cd97846586c76e43ccde53b33be27" },
                { "ff", "b3c581c186368409544a78b582288704beecdb751057e04ac346c480dffdd6a9e960e6f2f129ec773584530370eadc3c26a1954835c1a993ba6dc236f4ef6b26" },
                { "fi", "855f1ed586c056e9c559abb05654f1cbcf9abb72c7666cf58e19af0581b2f9d9691db6b29170ff86de793a0fa5ed4edb75c937a682dd8eec18d567eb0a852209" },
                { "fr", "aaaaec153b395ccbba6232c6e3c39bd45e96cd31bc2363519a151d4ea73cf8ff8f086f01583a44abef57dd9b466106c5691f45a6e62a9430e35bbda1595c9e93" },
                { "fur", "9a34a123f8fe7fb5b73cb91b579e57edae7337496cea79235368d831ac16114b05d69468e450de85995b4091a98f1329413ecca318d3e33d4e7d24493f4aa1a9" },
                { "fy-NL", "21a9254ec7c4c8a817f8860391480280d7e1f3ef32b3ab4735d3e30d13fbc6272cd520b90e5ecece5dc76ff2bc14c0a7d01a00e550292b6e293cd1f194e8e538" },
                { "ga-IE", "c8e1b7cb4adedd99557f92c9a26971c042a3b607dae2623bd794a87fc91b9bcc24b7520939239c18ee3e4d50b0867060048e0efa98df7c1e7aec6c4da3c42f74" },
                { "gd", "52d8a592e148ec257048fbbe78b28ef3cd45506b84d36b2538f7037d8901d2d09a7de93303c66e259200b036711a3d207d435f244c4af985ffe3357f323766b0" },
                { "gl", "1e3bd68f16b6dff5bc35171aa979e10206f26d5013884f774dfc9a4e068df45500598ed763da73a87f5e01d4a79aa2b95e8e10e5775258bb551fd41fe5c0025b" },
                { "gn", "9ae65987b90363d2e93be69f0641a979ea755125fe55f860ced4fbabb588b8f4ed02bb154a051360e2a1516f51569b3e11661fd072cb396cb7794b24fb5e0566" },
                { "gu-IN", "77972f1d11c59de0143eb52ad92170226b7bd3ced012b8a43a209816a8383dff79ee7507b7468de8788b72a2007d283957ce6dd35f0e32860014761580ad33cd" },
                { "he", "db9d1b6b4832f888e8305e9c2b65c91b8dd13f5372bf28ef34a6e5ff38ae01f40665cb8c390454180cb071fa2af856c1d349df036b32c040090f5b608a24e26b" },
                { "hi-IN", "0fae7c7a4d8ba3105b3b14dd1d5c1fd7ffa898a74dd734ae4afddd27390a4fb83061992fd59bfb98b45c91013005b4dcfa4e221ef10c4b029675a57bad1712e2" },
                { "hr", "64b2795ceb21044a0977307e1509f4b0686fb6d8189819c535a4ad1359d1e15863ab78c821b3260b40b927b9ad206c6eb87d6ff3409931081ddbd88702485fce" },
                { "hsb", "82d9a672778ecf26d3caed2f6b7289809811cc1f70a8394417ae15292fce864214826a5aa914dddfa16f9999365a8908366802af1512d5a601ceea3d69203cd7" },
                { "hu", "0c58f672794c3a3a3c1e01daf3b52d1b858ad88810456e3c1f1344c6e04df5aabb9b42846a2e0a3f2c85c7229eb2cf1158c06fd8aeaf5927f41d42ad1acbda76" },
                { "hy-AM", "9181a11623adcbf358964afb207888836f759fd261eb4d1fba635e16aac57aaa6a63c24cfd8bb96cec7036412d90d4c66077a800dd06a97ebecade32c20e1b13" },
                { "ia", "24c61ce2943325700bf9d7e3412dd0d9590c18983577510b3a2f4422093fea07bd6ae83b8f8d96c4027db4ee7e9b390095f014e90dbd252dd4c3a28abc783318" },
                { "id", "a85b4af502cd5fc06549c2247126a9d15d663fcf41557f1a0af40d81b2f52d81a7a8957dd5b7498f1c38a03260305233caf20d5882891fe793fcce5a87bccd9a" },
                { "is", "62651c76690eca8134708b78dfc3bc2f462332782d345036dec374c5a74a78a5ded77450458b7fc93669e84fb20dd8b25b4479dc7b8ea27011cac4b9e20d6901" },
                { "it", "84aa037276bd9bbd51e7e93bbf1374c7ec74ce52adc15b016dc31c6f25296f39fb5b8b6eb3283b648bf58c4e2fd2606fd27558aa0a976f96998ff8aa2bcc2905" },
                { "ja", "cf00fd6f94a5dbd26ab9e5301e8af46bd759adac99c31b1c406894325fcaf31d31dc1bb3926e3497667ad4affecf980a0b99b614ca96152174bcda288240915a" },
                { "ka", "164de14744400554515b2a294f41a9f824c1ee076e8e56726bf2a192a13cd35bb99dd7a5481330f0b9db82443a025049b2b9825881b9f30e71db540bae68b299" },
                { "kab", "d7f42483e8246039f1c17fce4ba5b92b27a03743fb4674c29d510bd5b44ce9c09f23d346ba80b2de5e984065a4358c53ed9bdaeffe3e5d2ce4a4cca6a9f3b381" },
                { "kk", "134af70d391b7c70a7e42b0b1ee81d6c2514220d4d851d4d074672303d5d19d43e626c9937c96940f175939fe31be669cc4786f37f333d3e890d45daaef8f160" },
                { "km", "15ac2e9fe181cbc7c6989738a87a564540d837943e0a7373208c0a3a44ec371ea41fd2474c857dda020d120cffd2099f1a398de00f60ede74ba05fa2ac95edec" },
                { "kn", "c5cd6a5282677e78ee21fa116f1c479ace5624cdf189843e391a50c631cec63877c3a71e999d3f9001bd433af6bd8a6a8cee2ea75a00078836de50688fa33112" },
                { "ko", "f9e5c468fe46e27f16dcb2ec56937ba9b2fc3379c178b93a4ef141077a887b92e7163f2574afade487e285fef5f841d82f5c6af29b0ab4956462422d9454dc25" },
                { "lij", "128f770ef38490017885076329f87d999389e3f5f6a8911f511eadea3e0c25d8cabd32b9b225f33f77830ad79af179439f45c4877774e0acda676726f49c9e65" },
                { "lt", "dc2750aea6160e245d7da6222a45f025cd3bdc44dc97f1c3d42f03719329d80b37e9d74d10d387b3febde77cade800527a02af2395acbb205ee76981f20121f2" },
                { "lv", "4c45978bacf19a56b24d1d0625099429ec95454304577cd9c879f5e78e5ebc8b2da85b0427c3141f3a3be26f6996c6223888c6e784556fa6db0275e38b36d2f0" },
                { "mk", "ef0377edde5f646acda5d9c49c8b2a47d338f097cf7606c728d985b3f37117bb31d413e9ab3386a5e297d966d88e7d27e01079f875945eaf8d71374b773a7023" },
                { "mr", "06738eeb55d717bc82cb9777569da77a82fd81f6ef360516582b35a39d53d3b4b6cd7a1768a47b99d1cff70d84e53442536f26dd4578d5304ee3b8d6c10a6321" },
                { "ms", "bca0c77f5d88241e02e92924ca38fec55a84d4289ed84fadd4da20f41b7099f68a8025d345534d9072f4f7cc7d17148000f0a30f91a6f2e9a5b90836ca86cfd4" },
                { "my", "4ec5fc3ebc1ec6f6b514c34c24b48a816c689536cdf239d56fe85776772a1ba74b0835f4e03e9e3b5653e2d33013d1a98748e98c61118e3ba1086c2e7d46f19d" },
                { "nb-NO", "d60ecdf0901fecc87ab4166ccfe0a620582ac04bbabfdc222f6e8e15df011e3dc50e8698204d2345453134f2883a4af8a336ad1f1a925fd54afc3a6a212c3e71" },
                { "ne-NP", "67f8522e5f714f832038041e72843afdb8faac8a6fe137e0f2075b370a6ca31212a93a4aaf3747de87b3d6c39a2ab77ab151f38d97b999253e6b9533ea591c82" },
                { "nl", "4f0d9b3df70e266d5a02303b7702a43bda29434b7ad58d0643f83a9382ef767a7f9345c9f44fa08476eae700c9a1fa085576473dbc90d0821adb727b7aabd79d" },
                { "nn-NO", "cccad89ec2b7367e8a9a9a9c92a78e8cb4180ff13f8702f444b1dfee41d732f411d456a38bd5a13d6795c76899959cc4bfb4ef0432b1795133c86ddce5792155" },
                { "oc", "603eccc50e9e0e5406546a2f9c2915a0839411d69e1ccc48380bf3922e2521a23e34d17a371277f32190a863e790e0faaf7d5dafdf472907f4e6a6eada89c47c" },
                { "pa-IN", "aea94de412c68e30138cc759a893e87ada992ac05ab6edf85259e86fccb3a4ed0d18a2e689ad8747ed11c4d8cd288b948af491c632c64041e1e66081f8b4bea1" },
                { "pl", "0c619319a61d98ddcd1a9a3e43818d7865d1b0ade3d0512ab37e69d7ebf0076ef405deb653d483a84a835dd00c4ec9ae0cad4d051ab7e5d4a501886a81113213" },
                { "pt-BR", "69901d4dbb41a5b1989fd5104c770e47d5164c2ef85b301d577d8793642224ffd303358e465c9712e01d0c547767239297afb6a71eacebab8af8700ce773d634" },
                { "pt-PT", "e701219feab3fb1963776ac12a4e2f9903dae777920d61d629daf45ac65370a358a4025afe20460bbecc79206a55636a77acbdf136ba8cc4794336234edc43b1" },
                { "rm", "30d770ebaad27a59237dfe6265fcfa6206b2d17049b38c316e63e3b04bd63588607be013efaedc89de8751bd05d1333916de1adfcd6815a019ce5f2c35b913b5" },
                { "ro", "b439a5819d6f67d7858a73d6b92a1870203f65b73ad426165881a6ea2eeebac889be7ef70f28914c8b32a63210b4f825f4c3457083d9796482ecc9ceb3536924" },
                { "ru", "b1d577cdc908ffce4f31f2256a3ce2affaa4bf272326f48df1aed304116b20a012c741885fc46fc0d2f67e37a958cc38b9c59a236b7c12729b9885dcf7ef1089" },
                { "sat", "1954eb033d55539fb8296e928034376dc52201254c50997597fcca49bb34b58c7f0614bddc4ca585dd1a1d093b860f86033830ac19013c49678c5482d06690f8" },
                { "sc", "38c37f3252f5b6e93b98576e472a2c1c23f069109bd7bb49ee3c71d011dfa0cf0cd1c1a2fa47425c38937af653cafe0331345991c55fbc40e8dfc1e4f6563964" },
                { "sco", "098097a5d4a4997bfd9be083ba2931a6b92e22539d84661267229bc2cc8d0305fc893f4390c2afe494735d309640b59ae92d562c658102e07e461d440e134fb1" },
                { "si", "d239eea0bfd567e345eed7e5559d2c05a3a27ddb01916481ca7881525c759af3fc10c57643a9e8a8df2d01fc2a1a85b7fccf1adea495c612f5ba26ffe19fddaf" },
                { "sk", "68f2f386748f678f2d9522c20325a8923da5f9e4a043de7ae1cb9ee99812fa4a4ab718b7ce1f9c3830ed9c082e00923b90c7b34532cc548ce57da0048047ddb3" },
                { "skr", "c46eb5963561376a3d62d6911868588e46b9364ff8a8b64aa9095ef7dc5e51c0721ec62234bf6af171f2b45d2b811a732cf7b00eadc8e280412a375a7235264b" },
                { "sl", "aeb753ea3ee4b84c8e17221952c09f9310f071034ea437b1b1bac58597e88a3135d915f69f6a1ee79b15d1fded33c7d15c4c1fe710ce6f486c2353b3bbf7f485" },
                { "son", "3de59e995cf86fffc550aeb25ce7b64dfeb0c4da1c51bf57129f4e4f2407c0aa6bc09b14256bda1aab61df0a4109640b9555b44997bbf7b031099aa0f8de3d61" },
                { "sq", "7098587bedfb2f09d2971959bbab05d31df9b3ce9a2204ec4ddd53d014a0f34dc2196347187edd7f4d3b0f7eb8db885c993cd87b85b353dea758f5accb153d66" },
                { "sr", "1708a00d0ca9be0e55635abd83a5291b60551cfa059a3e4722f9139fdfa8d7f35ddf7f9c52f6a68ed2f57577380281e43e9f73cca1d833759c43e412283032cd" },
                { "sv-SE", "787f3349c39e783b115536d0812609d5eeb1545cac234c24b0057ebd923fb01f89f3adae273164e326a3faee3f7283aab8b3eb43935b63297cd28bc772f6ee26" },
                { "szl", "360b3cdcb19d9b82d77ff984344ff8a5cc23b9d5477ce52f09b8f417e2039d31d1e2fc9e83afde31688cab6a97726c85921bfb7d83ded54cf9340d3d9b107391" },
                { "ta", "4226ff116703b5e944c81581c916616824593ee0cceb235e200eb83ddba2524b93c72a995647962f6cf6980fa1a3cdbf68bcdadeb99212c9e542ca9bb08438c9" },
                { "te", "7b492590dffa343f9e241bbeb38d05126536ffb4ceecb77b94aeb3674bcc04a57acbf1b37eb938e54cdf3421cc1e0f3fe90b30e3418bda9add9a72440e83389e" },
                { "tg", "e8f913cf7110cc59abb1dede44bcfcdd21b43d9cb1a449da20c23bbe30ec7d0da99ebac98f71a9a3a2313e958e2205dbcd85823423c133dfe868c21487a7145d" },
                { "th", "96873f15e4476a86fd71db6214d0f737a46aead7d67c4c830410f58e5ecc415c234cefc27137f01bdfffccf17e03282ff59b9f50e80b07a929fa42276388404a" },
                { "tl", "19b4b4c7134cd5ea3c1e83b9226f6611d3c196cc0fd3bf014bd7453fd3dd24f3dd2eb05aaca6bcccd577906968236de8b5190273bcd2c6f97c7b448e0dd5cd67" },
                { "tr", "465ad5d6d3762ceaeb9b978ce0f4087992289593d1368f451c2ae64ea0d583409339d36054e15799737ec129e2aa82520e5d7829981b8c0bb2eb3e354598180d" },
                { "trs", "1171d8065dcae569e0e922844a537070bba40dfc0b1157a6f849b7252bb81f3811339cb9b0da4a351f30069f120a00821dd82031e30fd2969451ed5fda06ee0f" },
                { "uk", "ee3d5c6131c0050efe038e14ec0b918142b475574499ed347894e4c26c5e9282bbd4cfc25c8a13a21d97d425473e35c3d7810981ff1a1bdf0d0be3e392ae5df2" },
                { "ur", "8c446b0e5c676ce214315978d570bff5c29df22165bfb49888a46fc9bf154fba89aab58ec1c1fb609d4153436fba9ffffaec156e10b3b352dd5ea0ed4591d5d0" },
                { "uz", "c398d2857021f66708079289eb2bdc6015c1751895714de30172b220a645cca823af20f37235742a9efa3465a7f225091bd7f7578692cfa70db51411b9b6824d" },
                { "vi", "a3a14af17872635111aa7a8c21d092d7f0f6ef998ddf8d32c730cb2c558212a803b08693fe66ea2c143dee23f7f3352fda7dd5fbf026ae91992b5a01e60bef91" },
                { "xh", "5a04f21b50122e513289842500241af68d9313a8f551de6dc97b290ae7326e70375656b6b25d2cea19524f705616a5627e03556bcc15024f08f06ca2988d3105" },
                { "zh-CN", "15cf31abba9ae58929c57474dcd6cff6ebfb5f6290872f9b8f72b28c2143ef26fe6d8024b4243203f4757d05130549fa9a2efb1ab97b6c90182df118be837e52" },
                { "zh-TW", "1e800391e9aa6d95cd3cb94b37f6ae16d3f3886d628b71dc5ea5da033baa13c994bd8c1b7898e27cb513b9d7053909596934e49c41d4a1a18225a236d916c793" }
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
                // 32-bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64-bit installer
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
        /// <returns>Returns a string array containing the checksums for 32-bit and 64-bit (in that order), if successful.
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
                    // look for lines with language code and version for 32-bit
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
                    // look for line with the correct language code and version for 64-bit
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
        /// checksum for the 32-bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64-bit installer
        /// </summary>
        private readonly string checksum64Bit;


        /// <summary>
        /// static variable that contains the text from the checksums file
        /// </summary>
        private static string checksumsText = null;

        /// <summary>
        /// dictionary of known checksums for 32-bit versions (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs32 = null;

        /// <summary>
        /// dictionary of known checksums for 64-bit version (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs64 = null;
    } // class
} // namespace
