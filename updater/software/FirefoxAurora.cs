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
        private const string currentVersion = "115.0b5";

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
            // https://ftp.mozilla.org/pub/devedition/releases/115.0b5/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "2e3e26d1c80997fefa76c80f4f5080168e1c1c41aaf397b932d76d11e6083e2f005b56461cd484ab77e4dd825694ad08eb4ee6993943b96adc6693b8ffab6571" },
                { "af", "59fdf77b3f08cce125575c8ad4415f039a1b570c984b9540d5e95fb77ab1c14bab9949c1de469038ff0b3f614abd3d52490632e41dd5576a40376676a0159837" },
                { "an", "9f49272730c13e308c6c38084cc87e2399277f1c744c3248b4ff61c1203bed9ae920085bc8039e8ff44b96c1e1bebbe84daef24f139df7798cfc9a550de5bad1" },
                { "ar", "3fe3f0c6daac43fcd00d443e836bcef8fb7f2977255ea8d050ccc9847b526278af34cc317ede0dbe16143f7ca13708420ba0db955095883144bbc2caff8fe08f" },
                { "ast", "2a6f22f0f998709e54a3d437e10e0694b4db5a38b3e984cc53f755857ef87a070b541bd46f484da7ee29b9fd3323f93796b97a7cbe40d45e630039099d47843c" },
                { "az", "2020f1a96bf834c2eb6a04da93e40f16bfb06d0f8fdb149d72243a26e802f04d3dd04f2cf82976c3a8f60c25eff9ee572e1aa7b7e9798c8bf1a0f75022a7d3e7" },
                { "be", "4317fd9530f9604cc079e17d3e81261a35a6fd5bb4dc4d59e48f5c09ecdcab0dc6d1c89264ca0b313c75903d15c96abbb9071ffb12c45e91dc03dd58f79ec3a9" },
                { "bg", "76d68a9bef680197ca3c9187fbd13e6135c4c08b63403db5f4bbb2184954fa5ac10c610f0ff36a57dfff1ec513655b3a2ec79325652bb51a3851d4962eccc2e5" },
                { "bn", "36ea3f8a0f76b893675c31ca2d696cd67c0df21d8fe7267c0cd219cfe5f9f03d14a300c09929246218583646593562c057e8caeb6f1574033a04845b685edeee" },
                { "br", "c8c2ad6d2afae92306f6171a99c82ff7fd63e447a0bda0f2b35b981b665e6fe5afd828a363fc4b35da22c8e8ec3faaa78b5ada1e1ee6298187328f1f556c7bae" },
                { "bs", "30994b6e60dd57bbe52d8ffd3f783413f9b1c4cac12a1c6d795dcef7588ef9efb8a8c7731560a699b9ab12f4f6c01399351bfad37329cf52aa4388f4ac3047c5" },
                { "ca", "6088888ec2c7d6ed8dc16afa5fd445a05f2d3b8504206b9b784c5d21165a2bab87aa22fa8cb6f6fe687434878a4a698d9c3132756059b1073194fe9092e3bcd8" },
                { "cak", "7460ab84cb7a0d883ec0bacc3b1e493a0f3cbe2211c540a8bdd4544a69606f69c254e09e45b686d2721da8456b09373c9ad25d42aaa8505d7408a90734866665" },
                { "cs", "8db541f917226b7a6104df16d88763ad78b6e5e1c4d2d7139f111cc529f9bb1c2bd297e844e12ba3e2ea1c1383026dba4ef3292bb9835302cc23bec41ed41d44" },
                { "cy", "e41081e63692d28966bbe1791d4bc156c5d09009a0d5f034a321a307c7483a389796cc8fe3d2b41219107af214ace4bfb96c1ab9e9674e18e0be65b8c376eac6" },
                { "da", "a17482da18b763b5934e54b5db552df7328a2e856e5472e4bd0bde532a2f7b00be7358e77a437e590cbebba766f36e4913443fecbf129f662b859617b51b34ea" },
                { "de", "efb1d19824cb8849020fd445588fadec174bdc9a725496f179d3bb82e753965b776bbbd98f1849e1fdc5e591a9a31263eabb1e16330436fc4527c56fc1758d8a" },
                { "dsb", "ba8c7a062ab519dad80ed0ec543d3e855f50309bfb13d7a59e9237f0d7ae92295941249883a884e214dae6f536ece8bcd55fcf7ad8bf97c4f3785aaeeb2e33dd" },
                { "el", "815a6659a1cccec958e7dc68cc8a5ff9dd55416cff1e6153444c3ce42ecac286985b3c6dec073ed8fe2aa393a0caf53d84a88d0ef9e10ff9d641716846fbc4b7" },
                { "en-CA", "93c0bdb4f6a8ac972bf8e6cd9f55142ada7ffd45abbd035eec24246b252837a0c431bbf81a58cd288398d84186a993eee13fd8fa1b00571a15aed38f935348e9" },
                { "en-GB", "b2bba0bbd4145f465d15531bf2d242314d9d9a8da3bdee51097114c36645ee70b4514bf5c9266a856922beabff81903090778e0a442f4ee97d8af853e6d599e9" },
                { "en-US", "9ac7c272c1faf45033f590d191cd45f30b5a6de0462b6bac4a71325e0a55e89d6e3e8ddf76b10070e28826a55b17d656fd77221f0815fcfc0dfdada756dab0c7" },
                { "eo", "2873f3bc0a61f84cd1ebe7681cf9e5bc12162d04f41f5dc364b38981d48bbf065c21c9b428dd4b8ad5781ced3a568d075bf997097b9d293ac1a242114c7192c8" },
                { "es-AR", "b5ab5d91c6ad2b0035ce4f8a487fd6f73cc2e50527188eb805e891e3afb54c36e5dc1280a002026fee34120d586a1e0ab0fadb4642ccab9003ec9ce939d940f5" },
                { "es-CL", "5659234aa7fe5c4e5a7303d475f02cbdf42a70b0508b70af3ffe1699420ec0fc6185087bc100259cb54998be7dded179c577d6d1d1330ef3d99b5caa05df0eb1" },
                { "es-ES", "8a45f51886b9025172d2fea5943452da03929b7b5bd145f59516d8e42f9c0b9936777a255a7e99352e50198449cdc7ac443486147edd05f98990089b9861ece6" },
                { "es-MX", "56f36aca7fac90b0efff08b3d7ba1ab10d281a8065bbaa5939af63831bbf6431737ef02f6afe84929770d100f019ddbd85fa5eeca374741c9e666b62579c7ab5" },
                { "et", "5645612ec07b27d0756b34a1324aa3e8c1adc5be8d74b443c80f5cc6290b4b733e8dfba3d53cad3050ef4163d6ee65af6c41daff76bbb32e101dfc24436b0d55" },
                { "eu", "79fdfe84abe591cb2905fe3e51bd890d5b1f5d8429ec94563602849a4f0c78f59b90b8bf00d75cb84eece3ca3819317c4080f7f6423bcf6b86846e50f221f78a" },
                { "fa", "920c2009c1aadee350a8b6dcfd0b0af8d866938aad83ea149bdf92fab9aca3627d9f1a1c386d6f0c30146acbf2ef9cd0cb6e68decb947d7c52978e9bb6e87f7f" },
                { "ff", "bac7ec69ab7a656d2d9aea3c32c75e5844146d868556558e89690192e2aec1e5959ee305fd5f19f671a721c2abc3aa767c1425dfef16c1e9bc207109001fcdfb" },
                { "fi", "2f6f87596f21aea73185fb6c2894963e38084df6f9b0db6d31252d10d32ab17e69fb43036d73f278d8e1889300983c5a238826d1c9974333463c12f97a44eeee" },
                { "fr", "0bfce6363aca32fb58c666308fd6b2c3d9161307b17d2c441c3d56f0a5747a8655a929789e0623f9ef33d2926ed59a9c85cbef0eea6a32f3b8a6b83a0afd93f5" },
                { "fur", "898a96794666b63ee93cbef2f004add005a72575d015d0e526b6844d61c3966faa367f1f5f24e109e5ce50bb0f377b2b1dbcf157dc331f2891f9b58946ee3f15" },
                { "fy-NL", "8dfc8e370807b933c5681e998aed1224138b803a54b26f65042d23f98997d831deaa4324540a64d79d460cc1409404cd530cc2cddcad2a54805f353cec7951f2" },
                { "ga-IE", "f883e5ac61767c84a2a75b2b43378614b337a7e01ec6ca1e63af5935e6926621201b6f943690150c5d4edf05d09ff1cdf2abd60609bd084dfcda11403ae59f4b" },
                { "gd", "5b35a35f0cfea00ba6190244643c0faacab06b42c14221ae3b5ea2475f52ca3464b8f48d3e953b922e33df04e08f84417ba60c93389113fd1a70a4cde003c79d" },
                { "gl", "177d52d59a71a4db447aa2497bc06ec619827f86fc9c33a3828b8d3cbe7d928c49945afaedfa592cb41307ac1cb69b55048906581f63da40088334002521fb9e" },
                { "gn", "8f045b6953a3ff447ebca7eec9f2e6ca06d2e4b0ab157957f24fd1b7bf9af15aaa17aa1f1a9506108c4863ac04019e441de468fbefc0b17ecc0504d4a96cea8e" },
                { "gu-IN", "c2c288547abb03dc717144e4278d79d9f3751349f8a4daced7612a1d175616775eab8ab372b57a346b66496a1ac6eacac83d1dd9941ad18d5c9f99e06027da28" },
                { "he", "613e094e819335f2d701177330bdeac1830a43c97493a3e12d278affd079ad321ae87c0d182ab7a7f63f7614a38aba8699dbf14282292ffc05e3d56ce31c9c09" },
                { "hi-IN", "abd2c69dfc5ad974853b703e068f656c5ab9d37663067792007fa21a0ce05f300da4a9383d831bf8636621e542bc01ac879c06f8185ea46c9d956226ca5b3896" },
                { "hr", "7e81d7ecff93df667492db05aa5f92e26c36a0175e1e122970a5cb281e5a387a24ff43198098a3dcb82e45ad6c32b8277c2e53a5549ade0e67cdc109642187fd" },
                { "hsb", "569881613c55d71b67fd2eb6bb0b0e8934fd3223714b02a9f8d26895fd9afd5c7fa9401eab491329ba4db1ea2af4f034fa1c0a067f4a4bfbb4b015982cd1d195" },
                { "hu", "9ebeac0c0c21ba9bb3e899493a412eb54b2b06e017fa84795b38f90077263c45728d863e45ce8f1e780a593b1c2a57bfff053737a863c95aa15ff4a29a00949a" },
                { "hy-AM", "2bc18829d4325ecd256f486c109c455ee398f9dd779bdb8cef846105c9c2feac97086a1d11be919ec2db62c93aaa01e9371e8b4039fa1201931b2da5b6fcefe4" },
                { "ia", "c0145e0c64e3883ad589eb04818b956d933c53028cb048334b70b821cbf709f306d4569695e4417f6186894765b0f304a6c399cde9e06a8f7cc1ea40b62be01b" },
                { "id", "5e7d657bff96d8121f55f6927d7e244e00ef242b0430301c5ffdbb0436ed0b57e0d7472df56d3bf44357fbe4b9c81c9c3e21797ecbb8878da05e72868141d727" },
                { "is", "6db5707d73de0a7df85c5b0562253bd072eead441b604025f7d4d246ce09e4f7dcd5a0465a0b3073475e1cc4de1176e17cc0541cf8bea1a16a22391b0494bb0d" },
                { "it", "a37259d570c891217bc73080e0d3e3b49e12852adbd6f73eb522c125cce37516005111fb95ee1099348343d32268a9c91ff4331ddfb17c21b878164ee2016a3b" },
                { "ja", "240a3a3fa9eb91b8dbbd11124d7f54ccb9e8e0f26f7e9e8eb8db1ca2080dc91b01072fc36d9532525cf133bbaae0b519ecca61910d8a16f7923c64503ec25e8c" },
                { "ka", "0d2ab1662e928462988f57eb6797e2966cee65fd575e4bbb10b192d58ffda09b7d817a1df6a1f8828bbc449b83b4023ca5e912b45951c296d9ec5089dc89c7a1" },
                { "kab", "9336ed806e880cb6924f97293cfec2941ff531d9bba3212c94fdb2557cfdc430ef0db5e09e23ffb7e522638dcf9c19baefe546e4d5d1b84e7a514a745817688a" },
                { "kk", "15a8609588f7d3e975526ad77d0a83b6f16af33e2ec6124972bd06927bfd98af109aae64bf983cfe9b465ce0b652a12747b1aa5c593d8eec41fb012d26e213f7" },
                { "km", "fad4da523f7a42bcd4ddd1672d7f1f876426935f947769f1df6a73abaf5b22b3ade377f89df4ec9634d173fc50cfbf8d648a5d34877c1dafc2926adb8ff27da6" },
                { "kn", "6302d3590e6dbe67f7b59457276276d0d92ea58d7f2edef7bd881a2f9d67d8481074aa06e0403d408cdcc677785f21c4a709d997885c5b2c29abac8d358c0ac6" },
                { "ko", "526eb76abd618a5736f029dc55ad9f2f0971da5705584badd25f10729463fe0ef90d171c987d1f0fee4a8d51579a296510bbbf16ecaf0ccb35efcf0c2fb9c7b7" },
                { "lij", "463463b32408ba07299697fcb515cb99546ef1e5484b8de687115c8b5bcea870e6cbbf06b370e6009ddb49c10008be30c48ae388e6c4a706762a4ee786a18dec" },
                { "lt", "02df44e8f059a628b2c53976a2594618cca5e12ee01a9a0c3d9ebf40cca2f4ff342e4010cbe6590e3aa1e394301aaa9d9a3d67ddbc299e3211ac5891ee32c40c" },
                { "lv", "a81a38783e1f7f9538a3ae57bc856e150324aeb13a7d707ebb337c731f2a7bda2f6a57320e6df2776e05e4e34b2af91cce0db40afe529687979cb633c9c3cb3f" },
                { "mk", "f57e7e8df3ce8db9964c2620368e5565f33910158d13dc7b637d271672684f79113e5739692dbccf9c269932382334a9e6735d38a0a268fda6e4278521647913" },
                { "mr", "b214f29d3584868506cf75bcbce3b554a5c2d40ab2cc2a7c7591462d9c31e6c9cb3efcc9b03a27a1b93f1e5712886ac10849c0b020605b48ca251bdfd8841dfc" },
                { "ms", "e4f081b5607abefe4bdc83437a37b1757f203ae380b9452955ad116fa394f6b950737c943c5af5000d8b99eddc9400fbe8fcba992b835f0c429f5b81acd60dc9" },
                { "my", "e02d97de3f165c9a722ac221e40f760b16015c4d51097466f2d7a47bd37352263364eb64f42e12f4be02aee603cd2b566ed60722695c1fba908e2e83c8151415" },
                { "nb-NO", "2504bf349b5217cd7aa57edf088ac69a0e7eae99f1a5f939b50ec8c01830d8e1293c62c2e5df98fd4d79d06308699c14287fa385b3e754b4833497b636887e8c" },
                { "ne-NP", "a85f5a4b380e213bc45fcd18887cbfdd8fd205530f49a09d6ae7c74729a097251d79553848b98b81b6a34fbc37dfdd63f729124876a87f0cd841797d67ee6975" },
                { "nl", "8afa75be30ac40e4c22ff5dbda578e3e6b99ca2199d875fe6b94ad8390d7c4944ed874a73f9a35ee86d7f403333549078f9db55b3ebef7e66c7e9695ea5733c0" },
                { "nn-NO", "917feef4de043806f046b6d35d559553f2a72b74132c0bb3a3194093ac4d04c792495e503952b662a5bf335f50a39b8e9f2d63e057c3c3ff3a39d1efbeb196f0" },
                { "oc", "77a81c3619e0ad278ed499804475b0edc13016da3e08493d876046d518a736f772805dfd32592601ed2d2913bc84740e510efdd3042076b893a4c2d59029f0ce" },
                { "pa-IN", "b495d461fc51271da0a365f9fce1bc143c9438f81bd7aec4890bab9d5a0534ae904835fd63b7512b0958c6dcc54972eb3c9d985014faf606ff8a7aa6849f561b" },
                { "pl", "9cd004433ef6b4391bdf0008f7250cc63169ee93af27740842939778b93464854ac8858f11e42819fe131a6ec0219d00451d4a85a9ac03386fdd2e6ddbbf0c0d" },
                { "pt-BR", "89aa106fba4f2a0c5600a24c16a7bdb6927c32191a20d89d653e07df03f52715ff65511d6da64161547f9a3f6f1f4b8ac983ea6b5abb72211f9f5bd07c236038" },
                { "pt-PT", "b03831addb0a35bfd99f273ee59717db7afc60efcbfc0123da7bf02b0f2fcfe4e0b29b2260a73dc3b9da30be35ad51235a6b3986004d083c8f2753a0744eb611" },
                { "rm", "bf07bb26e621708894931804bb4ba5e654647e0bec2b58ca3233f13b783692aad90e49dd200993e0d49177be1b02a0bcc9a6945617c00fb26539c63c6fa0cad0" },
                { "ro", "dd2013f5e33aa8e24b08a367b916ce5eaa331ca686949060245c6a9d12c432be5ccb4fd98921ead196c5acc0da5bb5ae123d30ede0c454c32d0569775f30e78c" },
                { "ru", "a4754c2e03180b4f9fc446c42b9cb5dd9af56162db682dd6a5722b4eca27d76f97b5787f14abb706162c7ccc8751b5e58317a8d5e34db4ef68d3b05792a2a588" },
                { "sc", "95ecf021250ed269ce04521b05113eefad7d2451c4cbbf468fddf40b6ec318c4b6c829347ef68c9f0102b90882f7f7326812ee3c990e01268824d681d8818782" },
                { "sco", "57e56835f632da4d500299192cbad984ae93028169982f8f9ce05dc6279c351aa56164766a99c3f57c0a626fded7693a89ef7a3878c3f6fc29538274e0cb85e3" },
                { "si", "58df23b439ba92ccf04dc135468c069a9ebaf0d705384b443aa77e1afaf7882313b54dde5324896223c3acffea9696df0c60dac1de1ba480aee5784dcd8cd50d" },
                { "sk", "5cbde1cd41ee4a98ef1419d1b0ef8c192c166941e3fa88d297accb332bf96e417212d6089d067ad5cc3773469fc0900347c696d633f4bce669099b610f12aa6f" },
                { "sl", "f77f4c0b8344da5fc02d4adb140d6aa68050d74fb5ccba2bd2d5df381247f9fabaf498f9b8137bec6f79959404afb4b0c5f0da9d380fd621a13a419b73a23842" },
                { "son", "3856df4cbe1db0bc3d013b7144dbb093e3c692c1975185243725834926b9ec7b5233da9f4769da681a6a69bf198643a86cac481240119d085782af31e3a9f568" },
                { "sq", "060cff9c1832a324feb721c49a9db1767c0a860198f2ea256dab2a18b99ef77f8c886aab6239aae7de4dd67281dad4758fbe07e637532bc8ed3786167ccdd8c3" },
                { "sr", "7878598e4c70d8fabf251039226053d794b96f481487055fa1b42888556993420120c8fd6a81fd5bf14338288de1d94d4548eda21fb74329db07870a41177dee" },
                { "sv-SE", "184369f442fe4bff11e90071760049a7861fac0ef8451ad6a0b1827aabff03f5a8bc73f3d5462321427ca120ac7a05a4e266f4afee489be3b6575329c0090a22" },
                { "szl", "00b118bf657e1087811aca2902044e01902c38daa5a2c957d57133a8c644a6293ede22cde6ba312540244b976fcf0358a462e839e583722eef518808bc2e4741" },
                { "ta", "56290e274195a239cb8d421cd0b5d4f338d1d2557b71a435bebcf8f0e41994f31d4f1f9479438e938920b4c272bf2531a34feb2d1020d8861322e780c2333343" },
                { "te", "b5fbf475b343716734b01e673d9677237e15f163de7907cd843a300b0c7667259506d55d9bccfa954bbf9594eed7989173f806f28e301b09f3bc2a7058fb62fd" },
                { "tg", "c0de07cbd43ceaf87c87ed8602aeb82f550e994634c9546ab7a15ab7549532ca77ac52d4cb9fdaffeb45132a2774906e3f47d01cb36cf9c6f5c9db851ccabfbf" },
                { "th", "65e363a423e8871e59e7e6dd53097069741650bb21fde56969695f0f628c01eb2258c82852a8853affc93e255b140543fadac233d8d51c361070979b16380234" },
                { "tl", "32f172fd6623afd62df19d5b20d3eea610bc9c8b8995d4099bf4e8572bb325ab636ffc5fecbd59a67749ec32f3223de01863cd14108c1425180ccff954f6c677" },
                { "tr", "e73af80e52229768c26579fe59a49765e288658299d68b0d1cb7aca872dff5d8ad1c3f5b0ed8a8a79742ae493426f4c8d7fefd5741223fb17e82179bd98c89f8" },
                { "trs", "f1686963df743f65eb4bf09b71323027ba7bf1dc9e83d4b8771695c76511b3ccfb7be09d4f2362bffdf49870f65acb4a5dfd07054e6221e4a0e5487dbc4272b9" },
                { "uk", "fa2aa136c36d25fdbfac261e8234fcf72c0d0cbf81339344dc5bb76eada3c02095b1b3c9c3db5422e87bd685d96272f591ff5bc51806f8a316c6d9aa957a8c43" },
                { "ur", "ce4d8f27c1a61e6324cabce0168858c7b04302f135542360caa9a967116752b48b830ac7e4eba335d7ffa24fd3e304a64d7bf2aa9f8d76eebe95d07384f2a070" },
                { "uz", "759d2e8fe7d470357b63e6024e08c35de2efd9202e76787d29076fa8e931fae106e556867f0661278f7da41160289bbb4384eee2b4e421fd07987ff838950e83" },
                { "vi", "ba37b89411c1c26bd0d2f0c8ca06c2f5ecee3eef0fb7dcfc451e7c3cfadb2e8056edf22932e972032af668a0b15ae7a6fc9249f7ef0428c1c674b91339c0b202" },
                { "xh", "7cb463b19df62885890ee6e4d7553799337e39b750f603a078cfbb29788695e004b1938ffa491e69757f57de8ffb9dd6b4f1e62bdd598cb747c504ba52e139db" },
                { "zh-CN", "b87a8f62a1ba6061ddb6488e49f25e1feab2ffc56d8cd34e349e3e1aed79e1c7701d7620d99c1738dfa3fe4ef519a59511e55f9c820f097445e6703227ab7b22" },
                { "zh-TW", "e2824f514d5839bce43d9a1166a36813f49a2a84c09cb8f9f4eba7058d9c7a0989fd30abbb2917485d24ac1338f9366577602bd8bf6c1236d7f922c17cda3974" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/115.0b5/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "4bcf77c0f371e672a958eeecede8113cc40325861b3fac2c0a2858b07406fad335a822c8f7cea2e02ba7a385f219bf18946d145cafcf4ed26bb81216210eef5c" },
                { "af", "2c5ea9028e6db2ab16f3ebff8ce98850c110233c86c71cc72be34efc5969593c3819b989e50142e45a1acc499e61048d0319f0b85157d00f8761cd9aa4dae0c4" },
                { "an", "5af2463f37b64eba199785633e6f29d14f9be4c9108fd14a28df634e66ce2998d821ecd9139cfe2b680955896e1cdf2645edde12f9b4ea3512eef0fca0b44bd2" },
                { "ar", "2f38f22399b0d9ae5164e66cc083caae1eb20e255d37aa3972ab5ca300e8e281acd67409b1032c19dcacb95a9122ad451a31d084c732f07fe59a654bfaf72dd8" },
                { "ast", "9c59534b82b814a179acd0c4d61051a650c48bddb49bb3455fb5613e6e4ee1e5fc6b738b3360c41df65652d03dc8b0e5a8a335f33d6dceb7aa2c6221b06da61f" },
                { "az", "f427de70a5627aa154e23091e1dbd89b88f0641cb5e84b645bf03a49e147ba6148aab1fc59cceb6aeae4937c06c5c536b7b2ca724f9c669932e1e0a644914348" },
                { "be", "dd3a2bec95fa4fd1ddefc3525e50276be24b56f35cc83ef4da28bba6013e2a1b257fc3a92075ab8482f54305b05f0718c6cbcb2f48eaf96ac5475afd3e29ffb3" },
                { "bg", "fc415f72427e4669d852552df78d4e91d3cfc6ba788f892097074aff788d5204f15e1f04cfc5e62b78a696e00a22e1e5d777d77833870208d808c54b0b99ce4c" },
                { "bn", "9185e69264047536c35098e97214a27c8a713398b1de371a0d054568da458f06795f8b72fea64dc97bd42d74e7b3cc347e49fd2639bbf690a4fccdced5888d89" },
                { "br", "8f1e7ad126f3c4d8bfb50ecff78fd57c714280137c947cc255e778951f8d9782ea8f77821a4ad803512bee0333d3dca08851938c2f191c16aeed215063c47d4b" },
                { "bs", "103e44ff1e67c521e815d131d42fa3f22c8f9ba5e64d121a72d201ae9c82cee18a4c79d048c8337cc1096d7ced8378de3c7f5d2a7af1d2d9bb5f0f4aeeb23d44" },
                { "ca", "2a02885fa1dedcb356ec16cab44bf75421f69484d329df9675153e3df104a719660862bdbf1fb665a453175b8f69703d68e767177cdd54d238d8905e617b6e12" },
                { "cak", "0136fb9fc00b2ba1fb0bf14aa0a24bc6a1fd62faac01cec5589ad558f1dec1af37a4c3038bec8c50a0ce0f7e8f54db998569e5b5b79828cbeefb4280209acf07" },
                { "cs", "2b129cd05b4f0506b3476e751b3a0b08d58609e7c64d9f267c49cee048ad44d8a000a1696b0a9ae6890daebabea3a24d92a03dc56a82e3635b332b59d9277124" },
                { "cy", "2998202cb69cdac68ece498d2e3efda38bbe2d7ed7ce968acba217e921abcceb672ab6c3e14f45ccb5cf42185687512274a4dab15f0394b2cd5ad4b7f06d68c2" },
                { "da", "7ce81c866d814395f2f62e4c97a88951186ce8215665290ff59879db4630ef60f6e6c8582f17760825bd11df7c6141548e928f118d84af67bead6999cc0ba4e3" },
                { "de", "64c75a88e7c9fedd47a0150b0fb002ff26a09da43a53e95a4fdd57c01e4a90aaf76117d4b8f82b3c298b493d2a5f2125efbbc0d6dba74f184e0a2b1252dcc11e" },
                { "dsb", "e0f3a18485d10b8a5968f83b1ec1c182ae9619c2f6b2c918790403c8c940deb62a7b84f3427cb0505682e951fbe8f32821e48085e08b4494ca1e8f6b66831c2d" },
                { "el", "50789c27277be480ee71d39f91dce58b4efdc56c3915c32349fadb22c2fbeab51059a46d1a27fcc5bcb55cc48905af178afb258999ff1479940d6ae61af7441f" },
                { "en-CA", "b28911d3b2c78667314b46d77230fef90a29e79326164e50e31d378b5863c13b101577893fbeb2da08defc556d30aa7f66b857bd5792ba6fec3779fd021b171a" },
                { "en-GB", "4895c2dd1e03e51b9ba81615f5428919b0959da65ad84baf848b20429ded7e6a415c306f7f7322409eabe127420222c08daf50a4aabfe13956b28e236827daa3" },
                { "en-US", "577451b1c502880696862db0178ac22540805dd4205c1e78264060d48c1638312b8a3b0211935e187bdd0211ad8b9bd9e279114bc3d4cf11f7276b06e19f01cd" },
                { "eo", "27500e72e384052b9448f692efcb04b6af6b31be07dad866cb08b321c2f8e1103de3586986112138531569adf95a61051c8cbd43c1ecc6508ce16c60bd6e03d4" },
                { "es-AR", "21b3e42fd5d4ad7bfc1117b97be983cbbc75129b3e2aecbbb38bb07ac8fb8d9badd79d3ecdf8029ee95cbef408841c29a450089094e493dcedc0a03db8a6bc39" },
                { "es-CL", "e01e1a87a8613060a4bdcc2bdea48d0aa9e3c2accf63249533c3a14205a5e4fd8eb60e918242459aff14dfb59ca832a5c5e6efabded24e7fa81ba48865ac2de4" },
                { "es-ES", "6a1d6f94511261692ae28aab56b0dac050c76b5c1b375da899936ee7b62271e41cdae3968a86feb6f75c4ed2b8d65c754da36ac707810e3b3fbe020040d6b535" },
                { "es-MX", "01207e27ddd3eb1d49d4fdaa30d9fdeb7f0f152a038c933ce89d90532f26fe457287b65d04bee9f8d1c36068b9734b54afc09000a9e3db21cbdb14e74103ab3d" },
                { "et", "28de7adc70a4d412983bff4f0f800c9badcd015451d55f5e6a4f7b68a3eb28791ffec42b8f0aa848bc5718768934df077d52b1b57f770a9d2e51f04c1f8d116e" },
                { "eu", "229067de9ff1fddbbd565d2d1b30839451387f9fad0acba884be0c6051684c15663aeebf57c3488a8054f9f86cd689afcf3fee526c62f1d0fc3c8ac0968b331f" },
                { "fa", "0d3d664c638668bcd922f6b6dffc17342f020fb9d782bf58e4310cc6664293525254287c9eb8533043de771de97a869349f4eb06e52ee587b48e1adbd54ad7be" },
                { "ff", "95a6045769dd08f711752be9cffad8fbf4f3a9d50e4d05af93c7220614e581de63834ab92f505f9916c608474081df410d7a88141221437bdbf7da0086c749f8" },
                { "fi", "e0cd376fcbeaaee714559d937ed8472134aaf52937be8a10536a1bc36f9f99cfbafe81b08a948880a184b3b055d46827308dc7b2463e6dd24777384b592ce75d" },
                { "fr", "b0ed70a165859673915ed9d87ba51168265049b13fb1a5cf3afd83000947765e2a567882caefe0da102f64a97da688af77de691e170960afacdf98220c84ec78" },
                { "fur", "bbd93b71848f04d7d7c99044876a4711de68544371cf4b986592a43480a164e38309e2d8c19bc13eee4c60bca0677c93b522315e8519dc89348b99bcaa250d2a" },
                { "fy-NL", "58f8fa47172de95d68ed961a0e7494188aeb8d9564cf431555ac8ff501b6fc12ba77a6e86a8f5e867f928d113fe72b58e982990ffb90bbd3f339996f5b4b9121" },
                { "ga-IE", "d53d100855b901ba959de80381428c1f7c701a2b851aa412f3ac7d33e45469494a2cd0d88df7aeb7d0aedcd804ae8c0e5ba01906b97738a1ce1b4c7e32991aac" },
                { "gd", "47c2ba8bbdf60fcfe42629a30b52a07e2f3e2aaf6aee4d225615cdb9e01bfa3ecf9731e3e54cf51c7a350a0ef1ab4745be07e6a2a4c9dc2914f421481d5a2253" },
                { "gl", "dd8f3026f9fd24a642892e41244518a923f8a923c70fdac39eefe53b990704a6983c5200664121ee2455a04d5b79b1c3a23382d6d0397ab5c06b18ee255c6058" },
                { "gn", "4a3fe0a77425c47567096e19c277dcd5b87119b1be570573286b10a9e908e172a2ffc65f2206fa336533532e92e88ed59491c5d553bfc51da7161c6ae0f60b61" },
                { "gu-IN", "b0895c386cd26237ba3dbbddbc09c858466baf3b231939083fab62edd82ddc2b857b9f4bb86b699a43b2ac3c6f50e65b36719a8fcec2eef41cef704ee5623de9" },
                { "he", "8a31f978b556cefd148af38ee7c7f8fd4db17094e7840f6d39517c79e0ca5a2e5350b3599b000e902963df3920a1facf43b86a930c11324e33ef5c8afe3b1c3e" },
                { "hi-IN", "9b652eea11639ca649f6a2bb3319361e14cbd679b9fc3c68e52147c8752870289d60a3c5187028e4f130b89e991dee704930f93be82db0ae4fc06ec051bba7bb" },
                { "hr", "a4c626f9ba87cd430c888302e32999325deeec506cf7f4346c4339b438d50b30a1eac9b337e3ef37a4734539a144a82a18e02b3a7abe647eff83a25615348c77" },
                { "hsb", "d8f0b6fcc6c2308463f427161698b97335ca40df43f7f62ecdf05614dd69a6d23334e5e66ebf39d67812f3d0ceaee0e982933e7d80f0d0408c8fcfc2d7c23c91" },
                { "hu", "616fb1e8eb8529ada27c7001bc31090278e42ca41eccdc7b7c9234d02912753c396294fdfadaa73ff0b8f8b6ae53ccb87e4e3cc0c7ff317f16b7ac297125e386" },
                { "hy-AM", "0a7e774504bdf05a2e37e0f80b899da86d04092e3cc2722899ad2a61d643cbf9a7a40b622f41e4bc4ec9e657992a491bbfc649eb1e31aba428a4dddc3621c8d3" },
                { "ia", "3506fbdcc2ba83df9135da3045ed3bbc12219838d55b8672a264e6f0ef5133f5c4fb273ad71e5c491ce3e36e7133a70f7f9bb8fe26e7c68fcdf59bcf97cfdd87" },
                { "id", "723f045890045831f98b78e32d7d2af96de31d1ee096f31c7f67ea11ee653436ed2b7c7852e4f99f5d469214c55567e09630e54ab68de363467007c320ee5e63" },
                { "is", "24705f3b1dcfcb786de6515d789db30106bf1fdcbaab6380ca62b2f2b0243f46b3df7dcf6cdd4caac93b59b0d870d09ffe92a80e39e01a62e01ee9a1bbe5f204" },
                { "it", "442f7f2ffb4d4a745ba9ecadcc47f1a143514610a3fa8491116bed35fc2129537ae2cf89a5d12cef49e928d678e108c71cc713bb0e99a967d543817e394f46db" },
                { "ja", "652e18fa0dd60d29a48c72a01d3e90560c912823d1403a0b383818cfde43894396c037572ec37d7a6df3b21c7b4b0833432c6acb1c047116a69288efc7d153a9" },
                { "ka", "98737b8ea9c788f8f7214d5a916be6ccf6ee3361942624d79db2ed548d69eb6b03a437ba1e84290163a8261951481c77cb59ed1adbdd2bda246363e7dd0c0f24" },
                { "kab", "79692fc090fcdc79dce24abced21e8f396365c7d85045b9d8cfd5e865145e2fbd83670a201478c9342cd0bc97d244812ac716fc89d4e8db61b4a750623020bee" },
                { "kk", "9c68152777630232812253c2e139d2fe5a4ebcc4f6f8e60a3a6e553a7033d20df64da2044cce429dbbb6c73c121c03d2a81e76d8c5eddd8b79b9cc1f4437cce0" },
                { "km", "158664cdd143e921da124be1475a65106ff214af45ad61b9c2b958e7a0be55382be09c60ef4e2a459667e9ea41ab084ed50ca87c8ee32a93f5828bd66e08d1bb" },
                { "kn", "dfa257faa04edbe0c0ac8c844906c1dd91811984063d7b92cc7a1b8054e0956bbbc8639960d39f36e80da1441a390e30f33313e0194373099c5bae7dc5424028" },
                { "ko", "67466dd061572a747d5b932c1e53009a8bbf0baa54834b7632e2041a36e2b95550684acb182085269c7dc143ca6e8fcbe419a34014865442ba382678613406e2" },
                { "lij", "935befd032df9e6a6128242b17d0605c92ea12c95d3d5ba7ae87836432f04d6fa8863c065b510bc894666a232fb28519e5f594caae3b5f94d403f84cd6c445da" },
                { "lt", "05ae3b196fe4a95af85b695c1500d4fe6ad743c5195ec4660653b051f6535694fcc594bfc1ccd562394990954cd5fef0e639f4ce7d7f324d1b876d404e7f99da" },
                { "lv", "a332cc404df19f4e0598e23fd09474a623a86beef7819b2bc7488e75782d81dc36cfb96d1d2714cd07adc6260b49f71c194e26e6e1bcb92ce32518ed7de231d7" },
                { "mk", "cb806d2d45ce4a40ff0838cd6df2460aeb64c3ca5c79b8a8476f87ca4140410cc88797923310df46c30f4136c9694b5af7c09e0e151263001fb579d550c541c7" },
                { "mr", "03380b9a576c7bcf35a8ad626bd0dc9b98cbfacfcb18085cbe0600fe74da46550975a0c8c4aeae75c91eed1c274559336b9ac452a067654eb983856588c7ea71" },
                { "ms", "a6aecf4af30dde262947cc54390e4f345e3cbec04860cae0f5ea464eb7df98bcd4f0209b6c5ebe5d2c96f6aa1d59f0a47787a88fa2b9873a4bd42dba1dbf51e7" },
                { "my", "d3d5ba68c5fdc28bfa387ffd0364982027471edb83cdd2cb1519df70a6b08c431ed6231af64fc4a4c74931e5c4ae8ab95fa13047fe3c7f7ea43e0a4938cd7642" },
                { "nb-NO", "aff963e0906ac28ff7d59766a7104c1030b17708fdf998b6f1c2621df8c059e40f175019ddc1e4d9d8cfcde9e783b529ed27a6f97dbcc0ba33319133efcb753c" },
                { "ne-NP", "163828d906d705cd42b1882a0e8c4a02c3a75322637e39fd457141263537ca3bec4e36573fba98e55043be0b54506a342314189df9dd140cc6510e59780184df" },
                { "nl", "e82315c6a37f41e41861d7621908460583854c84886b72a7b31bd1aafb4acfe29400115c7b48a6c8f1d10f801a1622c711db22ffb5f21a99ae2eebcd8acfc076" },
                { "nn-NO", "6d47337181610ca6f70beb8165916e2e5e43b20c057e0fab42251293a3ed076049441a042bc3268b3a7a8bc01a112d7c0f6fa06ad34af75ecfd8cae4a52e37df" },
                { "oc", "4b61e88668bfb69fcece9604663aac44e89f8f71642e0712b782288d541726978993f6d9ff79e8d6ba27d6dc87a12b9a2b328f7ba599687c6d65153c4f7b690a" },
                { "pa-IN", "f3b953707e7cac06031de95f72d7e843f37d4b743b85d6137e3ab454b3aa3d91ac482e1cb8a2d3667fb70da966e595908a2604338636ca17ef537a9b7050edb9" },
                { "pl", "deabcc774cf3dcfed98c370c5bf536e8562e312a63e95be05fe40fdda0bd20fa9545d630042d95241f0e565e3a5d882e3554666d3583448e51d249a86bf06927" },
                { "pt-BR", "e12d48274e6ed45097ccc8fd70df7bbcf9699fe691196b959983909cc9277d750b6c1399a16cfd9172f67d996797ed9b901f71d9bac384b65e4bf6ebae48f6d7" },
                { "pt-PT", "6fc272b1af961600afe84d1ffb9dd2c595c7ffffe94c1962cb75c357093fc3b81336e14e3964b97dae57015ca8d114de0cd2f32f3e4ab6e6570f3c5147d488f2" },
                { "rm", "95dc71c0be76b89a8597001b61b3e828cd73c38c590d2cf92ee57633050db0180e9d91cfcfce0a4c988c35f85ae10faeef11a1896a54a830bfb38ba99f3ee35c" },
                { "ro", "07ad7a98877c9aab70b8e63a9980291b8e464846b24fc083b0aac11b1ea90da24d72d848d64d9057d7b3382842f15f5ef02f4034174ff5a436ea618f33a6ee30" },
                { "ru", "52bfcd708e64f111da9ebaea29af96c987fc3afbd839f9233d0970be3ec0af19522501c0612c6c4e881f236ccbb604c8a0dc101cc931b2e292dd0b2d2c28151d" },
                { "sc", "fb1c14860a1b81ad4419edfd50af2e6bd6d5025a66ffe7d91ff38aa3b4dd7c80ee460f6dec11eb539724d9d8336ce2aefbbbd7016307ab29f99050e5220f8996" },
                { "sco", "77181b6b30f92280054869b93b5489454b75fc5b056e7119f03ed9ca7c5e13bb1e44b9db2911e6160483823c57d0282066a7c5c7b192d357d9e6222877942a2c" },
                { "si", "742ead4f0d6c1d316b1cbf713e5cbeaaef62c4bf62c651e4c92f7ed25ec5e1cbc646c88c3439d9aaf440775b8866bb703c41a8751aca1536bd2660e78539d5ba" },
                { "sk", "b9d97f0ddd2c761f91607b33edd3848b6686ba01db5343fd778586ad7776c81ebf74cae2fd3d7f4805817f8046b92e8fcb4f8429e1d89e0110c52114dc826797" },
                { "sl", "5690654d56d3da63b2e90792fa7e0ac66dcc7ccb5408044564ae8da2760ba5ff086565a40ad5f55b95627b3ec0973f9ec3b2ea61609c23000dad08fe44acd15c" },
                { "son", "0aad8626706f50f1867c0c34d8e92739f1bb5eefb828e85e64762f1d1c3f856ca37eb621b9e2741c06b588c9adee90835425a79e93e2f0d5f4272b92010a9ac5" },
                { "sq", "389930b234990738c48a08eab8d4a65915900728846cf300f655e7d3325a0fb59fb9d33cb4f14014d4781dbaf425c5a4715095129613bbb4cf1880a2f9d539cb" },
                { "sr", "732922f78cf00db1401f4d76a83bc37cec983c139b9c88e380f21c724ba86e86da1dfdb157072550f3fee548b90677a190a40cef675c1f86363dce76cd47daf0" },
                { "sv-SE", "b422707a84514e189fd75a4bb846762b5a5d115989acbbc869b4ed167f959a5a0a349dd3d7cfab619aea5db4fcabaf9ffbb2e823a86b83d56f987619d423f243" },
                { "szl", "266644c2e493c568ddbc4238f89ee370a1611beac82ce8e5c8da6ccbd54f385e1599f12d06a23f7161208bb98fd99e59a1f950cee9675219f6a25fdee0e75679" },
                { "ta", "75ece0bb6b7e437b89a0d1cd4f2d535eb122c2c501d6e9fa48509f58c5a4663b01df850d21b5674cb3830f9efe41adb1246181002d1a08e82efd874b77c3535e" },
                { "te", "6ae5acf5792f96f9de28c8c62c8c82c0af5fb574244af3e110724825942ebb3a0145f8a89f7f4048bc265c33cac2470753ab40d907dc3dfbbd6dc450af599018" },
                { "tg", "86e4065cdf31aff35920d8d7084a2fe75dcda78a84353db08638a7930d3eea40956f399f03b1ed7661aa2412415f3dca9681833b127a71af7612626a8f000d03" },
                { "th", "bfcf20b7a84530bf28a37844b5365bd128599b02863cce21c2629b8de7199a8002bac1cd603b3fd535f3310ad41c09f00a189b6fc7e964fe6538811620a83531" },
                { "tl", "80ad8a15c3a4ab8651d1de4cb12f1d2e2bfb8e6abee17d738ad0ef699612d98d6af4adeb6104b343a7de03149bfbe2e93b167e1831a77ab842e895d626609db7" },
                { "tr", "e4095185d720dd105cbf8fe6fc94df36173fd8ed22dfee2a9ec9c26cc645cd3823b4307206db90c551a54562aad469a0acc93a9b8e79c27c1cec3970313eb35a" },
                { "trs", "99283dfb8943443d2647a34f9c3e44fcce48a48f20d44c03f29b7f58c4a16d10f47cbbaa243b29e9cc4e9c0ebac0390bf19c6a729104bf1130795e4e50fb5031" },
                { "uk", "b10e9a2b80d5cf47b18c8f4f77e1836d9c59962b3b7c5a33e3960b5f8c62fc6fedd85745b84cf6fb23b54f968120d7bf1e523623d027db5ddc3fedcca34a0e93" },
                { "ur", "2df0525957844302b71fb86cfb13bd1e4385c5d99346878d2f47105bfe895fd7dc478bd3689db333de4b8d97935d84ad6b179becc58b20bcd0bef60b464de149" },
                { "uz", "0a5665ee347be8495a7b602e30be962c9e76fc349c49d4e91642304a07e6902ae5de1e302cbb71beceefc45b097777fb3468eda4cc4811259fdf07f88d70d32a" },
                { "vi", "3a6bfe2760390fba784b51545cae309560d42254f21a2d0a5bbea6146fa0ee8838a2d23482c407d71140ec665f7efc331b53874cdbd9ee46b471fe2f9e7b8292" },
                { "xh", "45e666a75aada1a452fb0f7bc103b077e47636a7ede61570ae8634326b59b201f7b49a25decf2e91a1ed003afa1721db771d37a5b6dd39387d2f7ff20a29781a" },
                { "zh-CN", "4b1bd755194485c15223780b9382dd283031148bec32b5b7d024f0ce74059e70f23f362dfcd5a98c52cb63bcf7e284b6ed779b48bd188e9e94897c9b992ff849" },
                { "zh-TW", "433ec49a4fbc4e729809879c4af14fc0b8e7f14b2b39e037d69565aa5cdf7d37e76e259005329fd717fe162882f1fb51ae5a16b333e20b4197d1c9e8503d8a60" }
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
