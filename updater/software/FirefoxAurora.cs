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
        private const string currentVersion = "109.0b7";

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
            // https://ftp.mozilla.org/pub/devedition/releases/109.0b7/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "6d83862f1da48f92cd7d617b3a4f9aa2ee9fff5d5677c4d563952646a198008d09b7dd28752d98beb872bea6a60ed3c797f70c8ee1718589db757b7a3da34f97" },
                { "af", "f0a009bc3a126ae037ad5e86d1644675b3c35473abcc998d6a9d57573a723cc0d68961ef01e8a7dab9abf956be6193bdd9aac7d09a6f8503c6ac32a8a87db0fd" },
                { "an", "7da74cce30c1245587af74bd2ad1fb7b9f679f405aa739161082072ad3b7827d2be3b4e40e95bc07a25feb041de2f888b944b8e54c53cbf64c17fe1e868ce25e" },
                { "ar", "35f3cb71eb6068d1d2e4e6cdfeca5c889f8117898d34da052af645d72b48d2a4855754911745bde3fbd47674f3844c2ef143350f40e0832e0bc916b127ea6c41" },
                { "ast", "341d89cbf49e1be41f1adb90c8d4a110220da09ff1ad2628ce168c703637b4d12c4126fd2b028f4f200b485f2e4ba4a9298055f81466b097b70b1c7356f71ec4" },
                { "az", "42bb5206a3e932cf36273393de703c0fd5bbc4aa6709ddbc5b38d27028ab53110667800517cc30ed26fab5782ca3f5e42ccd6693e3d2149377fe8b23a9420231" },
                { "be", "fc9f0b33daceb17f00fc014149b38e3234f901ddc90660bf346b9cb495337c694e6512e775a71437a32332156814a845f73337e6b26f3645c3ccd2cee381450b" },
                { "bg", "21a2312ddf4bebef61538ec9e09654aafcc9f25ea91fb57aa89ac06cca992ea270efcb1bc47e825ecc0bb51f1d3d7dd4517a26d892ed2c528a604d5c06512e72" },
                { "bn", "9f99e101543c89e0ae0772d75f6e6c4e0e4c7736b8b8b40137e7cf9b871cfc9159734411fb48822054c7a2e5eb4ff06c076645611e47f37c2acc09df425c7fba" },
                { "br", "39e21c902227ef5f3032d7f2d4431264787d5f1b4ae9a8cb58762436504a81b255a6792c6906ba7f62cf32f504ec667328145050fd670c82b2c6fab1beaa7de9" },
                { "bs", "5d4e2c321a1e2c56347e05f2d19d8f7afb89b07cfb8d6cc3b796f1668291e2c8958331f228f4ca98dcc428bb0dc2de16ff136450395138670f858407794af91f" },
                { "ca", "d3aabb9423abe71c08a745f42e00594451614441a5c47bd82d43ddf07de0ce7a93d5470a64a47e67beda2454e2c1e5c75c28adaa859c89103ff946785ad553cd" },
                { "cak", "1be5a276a51b662b8d68f990bb3e151e77b173c70b149ddce2672c53a4aaf9538b9c4e55d145b26c29f525ec576b56e01821f8b21d468933b89049136b68f85f" },
                { "cs", "16f94d75a2d01dda16ab753479d64a78b3427cfbf0b8b76b20a0a42d97513067f6f405998fbc50327546c6512e28dd8c26a72e97f5af84479538ae6c6ad80772" },
                { "cy", "f34fef0fc0a72a33fbd0a671f902373d47291821b88f2e9a274370848fb3c1943e06b1b3514a6571b16b31498e2eca5c5e3b36d526065c765efc97fdadbae5f7" },
                { "da", "178fa0a0f7420e45b906e8630b4399dfac78d46052f3fa13abc5740d50f01b3799fdf16711798b6fdd75fce22e13c51faa8b01b2accdf8863f5ab229a1b5bfac" },
                { "de", "5537faa886a2603d950eac9016a9cd52168abc2a7ddf5e3c5e945ad95fb3992c38347245b2ff24ed4d9c09b4ebe3a85a5b697299ba65990f90718dd1b17d7af3" },
                { "dsb", "44f45f94af2c722484873201adf8da9719238a34bd78d86c0bcb520138e5aa5c97776099639af95618aa98f708240ede4bba954d3b8e050545275e66054cc1b1" },
                { "el", "f329c9f6c28e1a848251f15690049965dbac5d822076c9ad147e5c2fc51fbcf007e339deb080f659b94e989011248d0fe65bdb3af9679bc8d77704e3a4076228" },
                { "en-CA", "2d3b5723019e227c97b0b61224fd58f0d654c01bb83da2c86d5b6095b27fbdaa75ebddc9197b93ed16ea2ad7cad73fe14405ba05dbadceb3dd976d381e2c9356" },
                { "en-GB", "b800a95c39fca64953c6201c9e1b7c56d62a1b1d307f32c3a62b70b38f5a6aec63a33305254b8f86f25fe22ee4de8c9a05e6c6dbdd0c3cee9cacbda64fe8d852" },
                { "en-US", "df5753fb1af545e5a0a5a4903c9d44c80be84021a0f47b52d1faad54d6453baa2cc8dcb35e37cb0767343d63b47950f9995e4607ec21a7a5ddfc852f0435b4de" },
                { "eo", "70478a26bf9dd16474ca5711cb83eda5890f8b7eed9c533c8a991e97751c3d3937b5463084867874e59ec080f9913179e9e3ada71b3479bb6d3e1a9c7524e77b" },
                { "es-AR", "c5136eec75090ff2e912a03492fa0adb79247574ab5348b4d152eec3c2fc9fc568131dd3ec3e922b414d6da2464bc8fd4c5245ca799c0b40e7e5de8e36aad614" },
                { "es-CL", "ae12d778591c675a5c319cfb819f674f28a4f92d9abdbf5e7989edf225db6a2aa5ae45ceccd54e0faf212353c25fc6466232ab0ba04e88fed830cb7ee1438dc1" },
                { "es-ES", "304c0f3c40f452ca34f9a03f2bf0c9128c670740b30e6787c29f0781a75b5efcb58f2d14bbec41f61865c31e9709eae6596dbc419851e77f1daa4837a5562f88" },
                { "es-MX", "166f98c39ea23a06417acfc9f05a369561d5f8c4d2496fe7c67b656d9015a0b0d4de689539e781bb5c823810304779799cefc5e386499dbefff6c4ea451dfb85" },
                { "et", "2ff1e36604d965c568ac835ea95a86e0f4ec550140985fc30f1e082040f578b7ee69d14723a06454e01626eb934b21c30e19cc8ded2d47669a60c67ad91f97b9" },
                { "eu", "a1e2328aa35aba2525621384c707342c66bff6d094f47a694d71ca9446d6f90b431d07d479d331012caa9e67d551b5f03e069dc796d385f3bfd94ea9ba4c61b7" },
                { "fa", "aabc998ee5addf51bda86ba1f0198db0778efccd6d8f2012c98bcea8f0b661dca9374786a414cfb215b416b054987e336dfebebf1bc03d02fed34e9eecf7fce6" },
                { "ff", "b6c5630b04027f357525fb8451f59dd61a93d88d8781cdb7af0f6385b7c3d04863885fc72cae307a54d717bcee955c3159cbf1e43430c2a80dbbbf2e5372d160" },
                { "fi", "34f3d7487b9969a464111c1df59d16fc752af45f2cca09b9953efb0593ac13938f6314b8e7098d8d19ffd3d13107abb2c562fbf23c8e349181a8eb010c294711" },
                { "fr", "9c26300af58c4de3eec77ae7eb8b3b5ffa29151c645e13287f7d4ff0e531bf812d7da9900b77d6b6522f835f3ac95e96763fe72bc0a73c9d55f3b1f08e1f8625" },
                { "fy-NL", "9d8ef280576fef9203f6a9c772e34a126ff7abfd980da04b3a6b255f4a344495bf4b2d4dac41f943672a8d9b9ad0de6f6c23fff3a9a80c39ecd0efe04fb71fc1" },
                { "ga-IE", "783e6c118c7f6c0a4ad449124a42bebf7f8ac44f44d9a571b34b982ec4013bce9a73bdb26790d5a8e7547b24a9a9b4dfdaf4219eb126745d814192b0f9f855f3" },
                { "gd", "6894d162ced3f14372bae942bd2585e672632d886c74821ccd90025e3742b5db9db875ec7b0ff37069b20f57e852a9abce774e49c7b43f3037b2dae02894cb18" },
                { "gl", "b1748018e5af3db12a505f67063e31c14d2261d08888c85f308b2b567b18a1dedf6926a3bbb49bb752e5189ef6e9979e1ab0c4643fe596882b18326d9233219f" },
                { "gn", "829c341c445dbc7b9f4f36654e7be2b2c327b465459815cfece2925c1a1c4485ebdaaeaf520f1314a5ffae7e40721a24c2bd4241da8dbf69f554efeb3d34b485" },
                { "gu-IN", "56664e80afeef45c1670fe19b6197b77d3eb103abf79f5963871096b7b4e8baa86d26249f20350a9554c48913e9301eec1d84ef9ce2751b5897317231dafd401" },
                { "he", "4ea164aa663e2c1c1e3bba41c17f6a6215ab30f8fd90f19ad5be6d49c8463e94e21792288f69cfede876bc641b9ed5260f5e294b1de3b8e74eb6f6aec6be1422" },
                { "hi-IN", "1f338a0e558be630eddd934cd3e48b9c21de865e0f3db3302ce42ac04f6357d9e73319762dcc62fa15ecc50585c38c14e9bd1125b801c65a6d46590b5a7a25f6" },
                { "hr", "bce6ea978805a61311a7c7b1ae6b38bdcf38dfbb6c1ffde9cfaad0f6772393e8ece0c06e72e1e1ec74ed2c7732014e287317c3945e648006e19c4f220a40cffe" },
                { "hsb", "76135718c376c4eea0af0f2a260787dcdb3807e670751e0c04bb0c389e3675b5e907343cbebeb0483552c37310d3ffe2ef426ea0fe6d5e79734694c51e5ec702" },
                { "hu", "754f266febd9786215ef7f054f1bb87410a8cbf4442f126538a3cd80ead33026e92ffc5b9686fc4e9e0a746b89bfe46cae33178c1a4b5879d250fb6f0ef71686" },
                { "hy-AM", "08ea13f0c36d3b8400f1f778014ed8df72bee842585f20c4752ac92bccdffd62122908e9f0c8bfc13e0815c2d37818e8469f6f6695afdb0550c03c82c35b4fdf" },
                { "ia", "0fe0aa77e595827aa50a1c3243bc2b88d79c3b06e5546dcb7297833b0744a1dbd1adc6ed908e39859d6381e8bebd501ef51e65d7da988ff8cc11382e15e5ab81" },
                { "id", "e8539f4fb3a4990656cb9b2c856c93c55c0390729650aa085b33476f6c1e18961cabe1e0102b81897559c3ae9c251c50063579df08039a71e133b4cc2e10ddb2" },
                { "is", "8acd4e9ab46c8597284532c5f6f6559b299087cf117fb724dde1c70d5ed4809939d8f8d07f8e5307f091c42ec3d5214a2e698e8753f5e97caafb22d836b1317f" },
                { "it", "42d48e7ac2493e5fa30d5fb4d6f341b1c67aaa03f16668e7ffb6d35d27a06184a4597e1332377e2c4c27d982907cb69c51b5b6817d5cbcb87aef910e99e4661c" },
                { "ja", "6ef0ea5d0f7c0233ba3f0e8fdd1adf61e1ff0280650c13fb76b89dc63d07ae7ce748cc4d046c086aadd86543b8323b86182a346ba4686e2b057911984c7d0fce" },
                { "ka", "e97c50af1dc7e60e70ab88cb658d9294c460f805436fad5014dfc10752b5d54e983b09f134036bbe064abe82d426db31a293b0d9a74664576a0b68ce20e778b9" },
                { "kab", "8f99ebffeda2c0dd36b2de0081e4b73dd0ae646b48c4c23f864490a4f035c51a9a16f5251d4ed90207e99c6cb99635675636b90f69fdd9e852e5d223394967e0" },
                { "kk", "f43b0c87c4c17b598b182ea36e81cafd973cc210f14e45cbe6107933e5c239c2a57f559a96ff38ca50794400e3cf7d2187de56591a5cc711d7f9774f638d3b2f" },
                { "km", "550b78fe0eed2179145b55ce000a2808d402750e068e47a3a5edaf1b0550b636c4e4303290fc54b272f19ba7b6e8ebe858ec70a95da19a2775ff8fc553b6bc6c" },
                { "kn", "99a7d7796bd293bdf5b476353ba052a483874276ac5255d5fa3edde1bb0b6b4510dd83790dc399eefbe807fc6f421d94b8549bbf6f39d10a2cfd2270fc474bae" },
                { "ko", "642c49b1adec19a6b37f3117a93a4376e9f6ffc7e4d53177c5b3dae577b4a2877da7fc2eb6b07f107337f718b44ff334442940092cee44d561c81926247856b6" },
                { "lij", "6d5c1f4eb9714842b76d3a3ae3a4a088d626e3ce7e8b6b5f47ad4f4c452600676e32c98368cb300cce867499e758bc237b1a1c4ce9c32458b03e1cc5c1242054" },
                { "lt", "fea8bb2db1e37152252fccde6bbabeb0a2befd6fc1c58197b0dd2e19ea459892c8e7a73c903869cb395ebd96249546b715e0c405825c17a3c0fa8f55665c6afe" },
                { "lv", "c392d49f34ae417462422c71c55f9224e0691bd5da9f150cc9f653d4f7ab2ed37e8cc6cd52d9b4750d348882bd89ae978fdabede9be88f96bc7e05a578afa872" },
                { "mk", "e3ea377cb971a9dd7a55d21c24d3c0c675aea345d3f61cfe1217c9f376b9201d0fc91af79bb261f66dea907ba4aef44fafef3f79286c6fdfc576413c92568dce" },
                { "mr", "22fc2ff1f33bf78d3878d9fbdb9c0be91f36e1d6a1ae42aa5c2db794010ede29d437266b1ad81a75ac02efacd308f26651bf1edf6526a73f58343ce7c08acf9c" },
                { "ms", "9f2abd7f4d2c6e175d12e8404914de2d92f1c5a03e8596cb361d6ddda7d46f251922cf6f11fa8742dcc7490ab0079527e4d6693a30005676e78734448d69db65" },
                { "my", "bdb7c98cbbfa7055845c6aaaea21a9938cd1c4af2022eb1d9e349767c56d3b5a80192ad112d0a5a27a8849d722064c9e9ff9fea86f4559b1cca961bc3bdb045b" },
                { "nb-NO", "1c7d4aed0cc32d8ebcab84fa4c91b43ecc0f8a39e61d65686908a985e9b1d0c9b956a2339362c953f670408c13f5afc0d02b85943f737f75d837f9f64e642075" },
                { "ne-NP", "a29486448d900258dd3d9f99ee7b01e844eb21fd76275f543db47429fabbed2f11daea2f5b54312fb902ca1be0577b206cfbcc2c46ca3ab1aaf1568c839bba95" },
                { "nl", "b79f5a2f4e73a9e5e8595a65ad020aa69f54067f470f4952f0512d62a7a35890e2d433f326e552fc1a891f930cd4cb2e6a0b8bfd8880d17629d084b2d4eb67e8" },
                { "nn-NO", "3bf2ab79b2786bc27bef85da32423bccc0940861c3b757c915b71e268cc34165728cf751aa36a2b0a4b2e6ed8dc03f941a9687e7867efb3a8a571e335d51184d" },
                { "oc", "678be12d93f56724c61c4aa3b142fe5168d0f8909a0a061de691b2e8f14b6a98465429df916f71b98442cecff7af83e2026e28e63362e15368bc4288204943a2" },
                { "pa-IN", "ae327f91b5a100bf1b129a8646a39ace5dd247af03c910cc305cb5971e7b7a2a992cc5114748dfb494fc738db76a2a221867660eee7dad65541dce7a830eac33" },
                { "pl", "4e76b90678ddaea7eeca0fe2c5e99cf3ecf43d96065cfaec684c242defbe361adee3f2ef5dc79c689647781ad9623ff90c626f1fac78eb218a0e4a621e48d3f2" },
                { "pt-BR", "47d784b1f8fc107a911102fa1722f375559437207913c4dc2bfd16c1ff75b0fae56f041934932522232607180b6644490cdbf1b8e32274121c3a5638c214bb51" },
                { "pt-PT", "ff8dcbdbd1ad486366979d1d2a056971bdb9d1c905fad9ad0e04a925f689382e47fd18e3af9f0da02ec5d0e706de8cf9b907dbc1a6735db78b8b65f284354537" },
                { "rm", "9f1080d12512fcd1379aa9356b93733bdef64b86ecdd7fa54d2b17e4b23ac49c7b0b91ca3da255bde41eeeccde187fc33366737d908e1f1632d5f93d0f10523b" },
                { "ro", "da420bb23cc81a492768511ea90f2598df1f95acf9bfa8e919bf901b666c36da829e4f2a9c6be59b03ac7b80d5773175c16ee4f830ffe6f16d444699e6496051" },
                { "ru", "8d241b86a0f531b6bd6a389ee728ddc701159df4d2f1c574baf443838edc80c57d2c7b79e2a183e6b6478b3e820b568676ce1d51c4472ee9877bc38d3107a89e" },
                { "sco", "5c871aafd332d62c1b9dff0d22201973b2e51f5292eab2876f46a003b096e92a15e032dac4cf0c7bbc49b21f217e634ae599a07b0abae70c7af0a8f060274e37" },
                { "si", "9f1eb8ca986526ca68f8240fbe28c6554716550c6ce42ae6d0a7523acf2935d4ce9a1425a39af60d334645adb5dcb901fd6ff98bea1d69ccd85cfc21c72eb3c2" },
                { "sk", "2bd3098a746626e33552be6871c2373dd401818bb52beee0ed3494d5c1a29a0ec6cc9ec91641dd6f486e1e7cf2a14df6c2235361953082de716ac41fc2dbd9ed" },
                { "sl", "871dda104ef42bcdcc39ba322218e154e47ca55d3253643c608b5350beb49b13b848cb357199b8e8f9efde9fb7ba89863701f4e52bb15a971005bfe9b2954e31" },
                { "son", "a2570d0d368a5067104180c145613134dcaa8b546f96e9f6f8f5439a1fa4bc737b11f362edc8a663de29a66b1a75c6dad422150e4989c808a77c0faa62f77a10" },
                { "sq", "c7fd293eb49becb90d94d9d3b3e3f513f7ed419670de702aeffc39654622ee7617624c7945c95fa26e76426b45771bf672516f1139c3e94ebfd2966fec3fa902" },
                { "sr", "063fe6e5abdc590b57e4cef62e44e394681708c4ba71839fb15155ee8f7c0651d987864d484f29bcd4be17099250366e8dc48c18997e61fc986b807ddce5bed0" },
                { "sv-SE", "609924b191ced5cfdd78599f72b22e7ddfa217c0c8bfedbebd7fa3edd7b23019934e45354e16f9d2131b28948067d3ae458542b61efa3c15d31384677212febb" },
                { "szl", "8f5fc1bb96b19b4785964bb4a030d4eaafa58c5f8913ef408a0cd162e0708d3ec84672ed0799169daa4cc256f437dc50024f1e551dca0a4232b12b7819350c0d" },
                { "ta", "1a9b62fc6b65c600e43da480bc9f9bef790e9a586cfd8e4abe10db86cf1d41901c6ed269e99ad8c14111b593c7353a778cb72ca310f00cbd63f9a6241e2267c7" },
                { "te", "6633f5854999f6268dd9f14e73bd0cc362800d53a5d274430401a211b3b222872cff856d139e5bf6668ceb28ca48b106cdec6d5f71f5dcb22c1dd69061ac0009" },
                { "th", "e6151a51aca33a2163f4e8525c78881af1db29cbeb203439de7101df187ff0b37f49e3245896fb0a92ea02d8a6a41774ddf6c257425f528621db9ea6504e3cee" },
                { "tl", "ba43bf578b5d5b49edafc9b98d7b40aea086ffdadd9694aecc8251963dc2ffd0fada09e8aefc74a8876efbfd2537cdd62f9c136d1f5e4fc8afdba872b7eb57c7" },
                { "tr", "aed1d6257b526be6e4ab05ef8085f1e76d7613e5731ac978deb0973904d26ddd9cb0f6a14a98c74c1d2bf4bd10affd9b2195a85476bd23b6ad1a86bf509380d4" },
                { "trs", "de54d285a29fed84af3479bbcf7579104b47dbcf9d2c921b86a55e0bbda10c82f706405e452fbf5b62f319110d189f885800351e97cd59f6dfb9139a3fa663e4" },
                { "uk", "4f5f6dee95782c03a47bf80a9b9e768e04f21ef2cae12058218658f503dcde31d9b469b24f3049a24bbe5c5f5f927eb71936712e62ec7b8fc54f58cb5327091b" },
                { "ur", "a9847168c36fe2e6b1e351ea375d4a141ac8086ecbd21c05eef0b8b3cc58b2784610a09c38f284e0dcca795c485f3b428d9a392821074df41203360e3e067186" },
                { "uz", "706f560f1fb6b57d32d4b1e0e34296ca547a660d8332f451fbe11980efee3d72a015b7a381dad2c64f7f21fff91e2ce2b31c01b127d112753ed8a8c0b57f0097" },
                { "vi", "da6f9f61d512a0928d381c0429ff0d84c2cb9a1805b1d02d39c95cb99d957ba24962800c6b3a47844de2dd288e08a22fde6f15d364bd65ee875dd0930b1adc64" },
                { "xh", "648c4fe4377ae58e431dd0aa70cdd478cfaa4bdd74d17e6a65a066a281e9f16dab7662280362144628ba32c085d5dd1c573377d6e36e442ba0360cf61177611f" },
                { "zh-CN", "99ffda1fe1af9df5c7618fe1581ca1537a06b3dfd83e3861eb04ded79e5e86214c41316ca4b93c53421392c1feb0347f73b247979689587b85fade3b1dd8b4c9" },
                { "zh-TW", "1e1a865dacbc7a75307e947de33e846c643fef413bdc585ec47eeffd3dc68d81a871bbdde3ed5e5e177325de2819482410c06b6fa34456f97b497577fee0eb53" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/109.0b7/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "5c49ea4d15bf158e805a79eff2b79067ee5ac1960f0625aee02d0ea1b5ed1c063fe3b49060ffe07603ed4fe9d0e1a0eb6353d5a42c05f9f53158718160567b15" },
                { "af", "dcd11c46c74bd6d18ddef93311fc4d9c9031b890368c38ce310a82d9a568ed3158d0dbe06c10d6e19dfe8d5a40a2e3c40aa935cf0abe1f8b509459aaf7e2e6e9" },
                { "an", "8ac20f26970e7f81c3e1d0a3fe4fdce84ab98ac70ef39523418bd2c7fe5136dc99903a9a7bca21e6d6b12a2152d72930b2d3496d175b3e5d29ced13dc5623997" },
                { "ar", "1e4672d37804ba40a90269f2b6f619f5464a24beb7e7303d1318dceabbf46bf7015208512d7685bf762b74a98f013603fbed2f704b1a8c7e72629ca236c5aae2" },
                { "ast", "93814ec5e7f184a67bd1f1eb9e06c47e3d436ba6d1ba35edf1aecb3e3b4361bc3d36c81cdf357a30ba6934ed653b2a08d47310e02437f1678cf6cc0d2a482f36" },
                { "az", "ba4514e6dcd7f2e6662d7a530d8efe56fbad19c8e25225bef7f514058b58921b189f751bc3113c8d0ec2b0d8dd867214598892b634a173de8d943a72a94311fa" },
                { "be", "b9cce488cd17d95159213133ad481b6239134fa4f586c131003599e16692a5bf1550d762345256e0331830391295fb81eda13a8ae5226acd10dad680ac6ab435" },
                { "bg", "908e3f97f9dc01d70ac8eff60bee5e3060fbd43e86f033358703b0e81cd4dd35d2ff3fe898ace8972eb97ee0eb7651eaf750fc0b5d8c22ab0576037be9fcd875" },
                { "bn", "9495f5f78f46b4312c0958346379055803c9883b154079b0ca3fe5a1f8d95ac3a46964da63435256b20e91aaef05cca06132b2dfa4fedaf051e2ba30e8915aa3" },
                { "br", "370a228de66ae465f5bc831372bec3055cca60fac99f4b55b080781a2da4377b11c223df381902471e7004a2f9cdf1e530e562d3cdd4f73c66cbf1cca9d728fd" },
                { "bs", "4122b8a6d44c0b638f1fd51020e5c1aedc6ed65fe879678c54a23f91a1ca55ebb53e7241edd1a975a54996aaaee2109ba52fb2ca0062bb7a9a6f5fbab2e52014" },
                { "ca", "7d6714af594a5fba4875ce6f7217531e07401eca72210e696a520d75abcbd753c80b123b7732f875778b0d6754984d7a5bee4a14adbbe0c6fd9813d90dd8db56" },
                { "cak", "c16099f4f1e38ad5eb170b0faa64ebce2afe94967ee3aef86e063e14b9ebffcdc0a60f039b71e7da1aea83016961668e0f690c2030ec72c2766271f0d4751c75" },
                { "cs", "e4cd61a639aa34ee7bf45f7e6f651daaf5bf22e51a7bfa3b6f2a363116010ecb0e135ed3ec5ed56d69460d3046df5d48f444ff2e12a1ed2b597042992cee8012" },
                { "cy", "8ff1843b6f23eefc391b3302acc74276d53911e7627f53a2ba9ea088e98badc91b25dc0b693266960e11fe6df3b9b02ebb55db8c8a205a1a1551c481fbe71688" },
                { "da", "451b7a0fa559d749bbc59fbeabee166abe7a1a9497ad5ba1ce32e230cfd4de3456f6c612f633e6fb967ff8202e7f7c11bcc5445be4fa70d1c0f3da5be3e315cf" },
                { "de", "3141174658b599d7f8aad5fec51944429f914e82d7fdd026b2dd132be7cfd7d46f56cbfb9bd9728ee6a52a7de9939ca69e893442f5072a756b7ce114faa881a7" },
                { "dsb", "b3cc78b68c3ebddb9ba0821b30b6391c8ab68f90369c99e33f3d67798827908202f049b6ee0d1beefb4a2cd4e6394bbc4bde0a53abb97cbd86dbb5c1e538b9b3" },
                { "el", "9be63b928ca45603c5ca9b0f34773fac96a5c95205978074b8e9a8ad514de7e7ac8fab91f18c5b318ed46da41c7a1b9182c7da2d21aea98d3a2d31eaa27af05c" },
                { "en-CA", "cd50b992253460a917203f058afe4e34fd8e0bae565dcdf4d82653b4fd752d793a5d0f07cbff058cb8032089326639678509880a90a1983a042e171e38ddfbcb" },
                { "en-GB", "7c15486de61cf97efc2bc424d950304e21cd5923a1b9ea5030483d2a3440617e72d3f201c8d36e274cabb2526c6208a25f1a1491c89229723370f7fdad563768" },
                { "en-US", "4b22e501548bb802db9785a14f1dd98f24ad45f25e2b87d1a0ed2f859d0fde4e9ce1b2af2d73219fdcd31cc009af37519f3fa27562916411a929134495b9b786" },
                { "eo", "7b520c24ca1016dd1aaf14c68ac1c393f8683283bf7e0610d3c33f58afdbabdc08b91dc4aeba16ef9dc83748920c8aaf343cc2036525b042d374f8912eb10cc2" },
                { "es-AR", "3276b1fd8c1d6c23f80fdf5af5f7eef22da377de0e89de67745710501793da9d94b824ebec5e4525a8f2af32dcd555a4654f9e0c1d772763bcde185fda73b496" },
                { "es-CL", "648e7fe1e1bedd9af1b62fac8b37ef871aa1481b2cca4bda019a66d3a78f6dc280a1ce8f5903515c7c4d5c5b9c26f63dc38cbe0b7952fe57cc0ea132329cc3a8" },
                { "es-ES", "920d427dacc5f5a997c1f1a2c86d24aed43f680c2b76b548a99c73fe8cfd343f2128539ef777e93dab6716d49fba9a76c027cf356600c175f198c88191e95415" },
                { "es-MX", "74fc6460a29097e6380fcc72032294fdec9a46a0886ae6371fb976a7632b11748b09dbc053b13ca100dad4f4ebeb3b9fbfe9d6886a80e942918a37cb38e618ee" },
                { "et", "9e7b339fa407af00ac65e8d0de7874a9dea860aeb6f5254c462da110bee2a4e1a2ebb2c23474af121257f93ab29bcac98371cb1922ab34c99d32d305ff5e5593" },
                { "eu", "7264e83732e1398561016463ef182544f062d6d0b55e7063a255946e277c81e2a5ae376eaded2dd46468a7a228f897a338292367b8848def9de33e9add73e111" },
                { "fa", "9322ccdf64e9036ed31b40685431b5f3ee1eade5c7fbba0908c7419f4bf55c1c16ad371a1c55b2efa94839ac198ef8045043586f6204eee892a40923cbb0b1fb" },
                { "ff", "eda4f6684eec14330ba912727848d3acd94463eb56f6a3a2f603477a8e9c8416976a4b7986e3e9fcda573a92f408310d8b220421348ed09987f9f86e02a1804e" },
                { "fi", "47ec6cf9b78bcd779d6e05fbd568842ec0b57a37bade9dcb741975f02020c09d897f835ceae4c34a57ed8c3b24ba98b1d8a3396171f6e212bb23b38f725c1796" },
                { "fr", "5be69ec46b543752f1b71d8b0fd402112f01fc686c8b474c117ed4655bd837e8381268dc8e137af26586e2cec784da5a6744ccb623e63bbfc6a4b9b7e36ca70e" },
                { "fy-NL", "e0955de74f684e91e6092b8160f76b7f2af4f45b1aeea9802d89286eacde1260252e6115161c712bb38105b52146aad62f16371bbe796a9370cea3a4bdf9ae6a" },
                { "ga-IE", "3e3c4fdfbe36a6be5fe9a7a05420fd1ac29ad3eda14a4d701c7bf8830a73bf3f722a1cc8c4cff2fb60f87d31990b89d16d37639b23541f167c9e690beb9c6035" },
                { "gd", "f58a37b36547e13b51dc7d682d43d8286ca2a92abcb128e42ffd49c2331c01fb362154b4148a7578df800df54d182e85d8a391485a8148e06f1de92861f34da8" },
                { "gl", "1a142a5820765d7e7c9e2686db2628aadc140d263d2876a5ae91696105c6d6434bce50eb316223a654c904c5fd61f9fba5fbcef011e338335ea7dc049758837b" },
                { "gn", "8ecc221d11c27a0dbd9f7f3def32c590d02bcd87e0bfab17e491b19ec448b667a85b74599d8a909170f1a7574c631a995cc5c3d90ccadd7ece5b6a06d7a4187b" },
                { "gu-IN", "d10e8ce723796adc0bc96ff52fcdb2437b7ff6eaabb3447b9186edaa0f6623907b116bdd0c874a65a4a0511e49cf161d7dbcd63fbbb7c983771ac3411c6533d1" },
                { "he", "e86d1332112ab214f8726bc698b0103a2b4838d8dd1d798120e7926b18afff93efb35f080aee0ad63c2699d50c9426e4606ec9cc8f28a99c882fcb9d7ab39688" },
                { "hi-IN", "8f4502e189cce61fb079dfd57b72cdea902c50d968a5a13185c6fca6401058b4953a8926b53b8068075ea711a2ae9d9a9dde7fca72eb4976bc48208f3adbba97" },
                { "hr", "02cd645c6b66bc8b9a3e82d66260d2f50f1bc97f013c4175551842904a6c058c802509f729aa3efd7368640623f47ab956ff9c660bbe6c3996d6d036468a9c8c" },
                { "hsb", "deaaa85eddca72fd075f08a188966460664344b70c66f94aa8d2cf9781f148a60f47fcc7380b0595e9456ef38115a888ca4db5a427fbe781617c8659c20e51e5" },
                { "hu", "18ddbc9f2c4aceb3a3ac9bb0881dc95ee8e994f60b8eaf06ad1e488f584f4959d60c751d3460eeb80b8e1cb8ac3f11d187c604df5b90a104aa4e3fb9e05cb556" },
                { "hy-AM", "4e1856fb0633342cc784ac552b1a3315c5a327e1aabb2a64c2bf796b92ab616d8b0cff43739fcb2054ac029b3b189fb54a3efbfbdac85a9a90e7409d1e0b62bc" },
                { "ia", "44a7f5567286de24c7d7d23361a57452a215d51482d755237b130e912e0ac074a2d4ac553af312f2302a23c05f188d3d38d63d57fb5d5e85a7dc1617a125c811" },
                { "id", "0bd43aad5e7fe42424a010ddb2cc2cf714c0f632361669102f289d28300ecf07ef0a37638a5a6d89fcda81060452c63d9b944c59f3e2f8c4347ff7a105439b14" },
                { "is", "3e3c835c67807edb677f9f53707d5ea2e13fdc070b72a01a239a2bd44cdbe713ca5ddf89914d2bfacb0c04ed4275e8f3922d7a16fb7f2249296a367c65bcbdcf" },
                { "it", "e18149ce40d3e6946971fe19fa129505bd68f71d0e14f36be5ac680db4ba98b4dc2ab9ff7c2cac02f69c2a638c8b59b3f43437726c64dbe40fbac0486d743c01" },
                { "ja", "4aae58dc507d878d7f81a9714000704718d6f772fb4ef521c1e4a3bbd3a4c83a4403bb95c338a654f4af71cc34b2f59481a995da5f6bf509c9c731197dcaba3b" },
                { "ka", "0df4fbc20dff067254558526120cf1174910c5e99819b55eeacf82ede931f49146603bd502e778f579e09f0687faca952e19905cf15c096ca2c4c4967377edcd" },
                { "kab", "ec8bfc8df689df94e3e48ae9a936a6930169e69934a507c7f9ce0f532c862fdec02bae7e0820489fce003195640351955f4d387ab5e1d2ac1696474797f955b1" },
                { "kk", "175a8126c10a2a6c0a5c520b61eb39b5bda40957b7f16efe5b419989ae2ad6a711526a6828b455bc9066dd20c3a23bd52f760dec8fe464220b87173568ecad49" },
                { "km", "1e207ad66e98d9cebeca8bd471d3a0e618a08eedaaa7803710ffd1a688e522bfcf08223609727d2f12afe64619b0f38a2efecb08426ecdf9b48390e7a758ca23" },
                { "kn", "40694ecd79fc7aebff54b91cdf3f51df28eb34c8b76fc5210542babd36d676ada21c81136f1348d5985aa553d938e49513b4a6189d709c427d5d4331ed5851a6" },
                { "ko", "8cfdacb9c8e6ea85f95f70af27b79cd14c70d2b078de41e4fb504f0c2f97ac98542269c8a45a8d5a2d9ecd4d78997cdf85ff71c3f1abdc73ed7bb1dd455be270" },
                { "lij", "68d6d201b93b1d3954096564150176073b93a9aa16b1ecd7b19a72bd65d72e7507dec0b26a5accb00868540cef4e379012d2af19b1b1bcee8bb0467592cc1623" },
                { "lt", "88625cff65949b41b9dc9361fa2cafb65310bfd14ea1345144140b1ecf9363bb4dc85c2af369676d3039495e920547c3ba4a19711b4b39ceb26ec3fa500fd7fa" },
                { "lv", "f4ce4beb81e549620e4b7e449d196bf7a6ab65d48ca00693fdb4858fe8f8ebda44592680b1a042962f84d469b5bf584c30eddd992850241474cef0766fdc385e" },
                { "mk", "fc69137272a90569f78451d65fa532e577a57ff2e8a2ab3679ca7fda88b7dfd965dc2ad4c825a5d1e16c94fcc34c0db179663a0214dff3fdfc2f62ea35b89e55" },
                { "mr", "9248afa0b71427cf63ce796fe7f27b4c6027b9ed912167400fb3aad115e89311da5badfa18835e2b4963d34132b961623b4560953c19bb34e23088c91e2064f0" },
                { "ms", "51d87bec518dd1bcdc5bcbe263d67928f00ecd43cc304e439dc8032b671c520c4359d1fcaa86f1a821b6e621c28d9184cdda4daa6e6106dfdde609865b55ad14" },
                { "my", "827d68ee9179b1f6196e5b3ed1ffff48a1eb57bbeb8737e966979ead62002170db71e27c06f6a610193c391bbe85bba31181e4afcec0e78e3cfb35671bbf2f24" },
                { "nb-NO", "ae8bee8a21ebf5c1fd92bc07f472145cea260c04a48c7b29d7bca9adc8ebabb332d41ff6bfe2a9029b0a173d95d3e3b5b22c509da9c397b3e316a4ea61b320fe" },
                { "ne-NP", "ab2deb2b6c7f29e473fb2d30677868d65d5660e0f3d8fe55ad6ebd8317a513c6bc6f241b6591bea02cb105d23197c347fde7f01606ac650a960665ab36c5399b" },
                { "nl", "32c7f2b9742edbd9b3ee0a5578b508f371f9f04c8fbe52f6b953c8f139fd9dd1a5f6f5fd43b5ab0b6426eb302c0e6dccd065f950eaeef7536f1ecf48cae4b3c2" },
                { "nn-NO", "79baa460b574520a902ce9bcca6858a3452bb93dde08f3f244887ca8129a4ffca545eb30e137745921cddcca8584f6d67e49c70bb8df875621eb5ee812366f93" },
                { "oc", "fad21f5cf44a5cf5fec94e54b0ff638cb91d8450bc6191ffaf0a4d28cf9074e2c33c88883977b953987f57e6e3a1adcf23d6a7efa3fef823f5a268a5abff0b27" },
                { "pa-IN", "0434951e68c1830f28f0b63cf67e75ff0003f418cea14eda2851922cdd222b80880bef76233c7cf7d23bf3de0392ffa4fa338a6ece555d114744051213571b7d" },
                { "pl", "a37ef2a28ab8c3939360b1fe491dcb810f50f6d3030a458837ffd3c459c2a5dda7b5287a1964851c29ac1065532c223c4cc0c91034372c3d1009164be7bb68f6" },
                { "pt-BR", "9402a494810348753f7841945cf79aa47387372f347bfe1fd98b8e19841c2570727c1c1896c749046f01d5c5bbfaae05caed922ab0c8554aa1686f6553166cd8" },
                { "pt-PT", "867ccd8086af2a45d4c5422541dc7449d4a158d6ae488e03ebb594455987942ce8caf1832192e016ea8a5ef0d482cf3b4ec2466e03ebe62cf4c3ff324ba11f4d" },
                { "rm", "85107c37e752c0b15e9f48320b9c3e70e83c8e7a183d90681e8527098b62c362b804bd45fc8bb065147cb9c783d7932bc1a8bc4faa4e0df7783d48608321187d" },
                { "ro", "22bb70c1fde71d6381bf3852ceb41062e5e5d04955a08f73d3ca4e2ad154cbcf11b18fb7af5bcea0880b6650e36b4b4b1a9a15a35b5b143cc4fe09aae21dcaef" },
                { "ru", "0618b254ca60983b94d182c0cdf375c77babd01946a9812ae30c2ff00353ac0f525b40b7678176aa5de0bb41ccfe52b053f3277b111391b90a4bcd99cdb1f342" },
                { "sco", "7e6d74d76b2789187c24b334235ab5ad7404383c2479f404c87806a663a41065ab48ce70268a92109bdf13a0ba025dcc00ecc157a2e56ec5d91d479acf6edda2" },
                { "si", "ee721df65e0eb347092e5993df7cafdbbea0419d8b0a365e1bfb2dbe88325d5051375c2d620d7110a2d9b3f975c6ac0834239a457a452e324ce30002336c60f8" },
                { "sk", "4d95fe57183c36e116d636d3c752c02d8eda0ebd21b6898b77409fc887cc4b1f6647f7e23cdc6a5b5ebaf2bf92dafb386ca727163ae960026136d7338c74f361" },
                { "sl", "c160d8c16eba41da030dfa7153ffb293b4ec8ebc5a2f4f67e722fd4556d5d84d8b4cd4e652ee1dfd2fa2b30ebe3268863e697296343b8f496c151013a3eea5b7" },
                { "son", "81466656cae69596472c8043ba31ce20c2f8f43205b175e8a8062cc273b8e32bd63d38a59597c014b5108b235e14a120cad670f7d6d2301e7715090dd4338946" },
                { "sq", "0aae0708d34f2e7c56630e0fdf04a2e0068264221b837897001da1ccc460d7994f73e3f332018b6cdeed545bc9f57f03f5bd5a8e0ab3b8c618c1a071b4a6640c" },
                { "sr", "1f671337c920965540ed3726ca0896da4186ef23f1a006d332c05f8d47c83c2a4c82b6d243951d866b2e96ff9f081bdaa44bb6e3f8db0b6315425ca15bc2a0af" },
                { "sv-SE", "566004f5ffe0a95b49c71e56ea074ab282b520a697887b8247b4ccfb87a17a9db0d73742fd82c48649841ea825e734d5ade89354b5b3e411db83c2e45fb4cf45" },
                { "szl", "4c6be5a6188809b3a0592aee1bbbd646a585043ae7349d0f118b8db17d013ededb1f284110c087a7f340a89ba7b75f5b00bf5e994cc92c15d13d57b1f0e35153" },
                { "ta", "8fde2eb0dd9488e6a30847f8e22ac743e851a23fc6fa35a5a8d3872d93ff33a86055ed275a2f8254aed38d4b44fdc76e2304f51bea4cd07ce72794cb500d0d38" },
                { "te", "bef17bc991ce35aa9786605b8e92c50f666cb45c908b2d26fcefbb4f5f21cebf638c862bf4af45169422eb5ec2f8f2cbd279710bb2e44566e70196591c235922" },
                { "th", "49333e211ed0d9193e3c082f16364f033ca88b824997b27a7dc9e487b11ca83d92152518b285492ab9af211970b7e4c981c22ebdfe83b04016186e527bb6e46d" },
                { "tl", "3fbb41c3b8be1ca671ec26d9efb333973c12593e95c19008d815dfaf52ccb206da43db97139f613e13ad5698e5a7503dc841761d04697db05fb3c752cad354ca" },
                { "tr", "c6a241f99c641961a06fabbb5ced327e9b0aeb5ba53b916415927fc891b54173865df1fe0eb1b22a73e61a85c89c59a2b26b9e69b498512acfddc33eb3c8f61a" },
                { "trs", "8ada0fc8a9a74d5437ce38f8022cf681b0f07e37bbefd40fd213e5bac8444f59806e9142eb823ea903aff8714a2684d370164ad34f9941b9c93c6cb52bd5a15e" },
                { "uk", "ba836f357d996638ba275c954782183ee581b934b45e559cdafb595bb7b9bd17c3a5c38ca8ea8f003f14496f1625019b835a3ac81dfee2f1a0d566d961c00912" },
                { "ur", "5bf60b4716cde88a1151f6e044c06c2d17cfd21ddbaa0495e7b0f39b90428fe56befeafcadf7fd9bf341aeab1c382475f8314267b8112f249cdd30affe9d9b25" },
                { "uz", "e4ccc1b7d9fa054dff9ebbc9436b2db2a7c2edbd96e781cbdb9a85f9665315bad1937cf43d32de19d8627562a2f2d50599275153e7b2335e35d3d059becf6de5" },
                { "vi", "f8160adb47aefc3f8913ed452c8dc64797f01f8b737ab1fcf43ae135c1305843b6a81d958be9f5ec819393b851ea76b24748cc7edb3e3b757fd1144b955aebd1" },
                { "xh", "67f51a65171931f2ee579d5f01aedcbdc8eb88ef6a7bd8976cfeb489e7710484b96d57614950be2fdba89c2306d93541209625798e6adfab285fffa8acd3e527" },
                { "zh-CN", "f343cc5da7271a50fe781aa2fcacd8748cc0da44ae796eba6b9dc7f05252f69ae094c51b8788214feaedbd53d83c8b3402f3c3be55911b7906a9d927fed81692" },
                { "zh-TW", "ee107d635f5a4cbe9c2ecf5c4b2a41cdda99c89a8e99899165e2193897345d00fb989eecfe0715ad09c7222d8b6c682595f51214ae2a5d0fd148f626c9075ba6" }
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
